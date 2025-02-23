using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
// ReSharper disable LocalVariableHidesMember
// ReSharper disable ParameterHidesMember

namespace NiTiS;

/// <summary>
/// Represents a mutable string of characters.
/// </summary>
[DebuggerDisplay("{ToString()}")]
[DebuggerTypeProxy(typeof(MutableStringDebugView))]
public sealed partial class MutableString : IEquatable<MutableString?>, IEquatable<string?>, ICloneable
{
	private const int DefaultCapacity = 32;

	private char[] buffer;
	private int length;

	/// <summary>
	/// Internal buffer capacity of current <see cref="MutableString"/>.
	/// </summary>
	public int Capacity => buffer.Length;

	/// <summary>
	/// Get or set current <see cref="MutableString"/> length.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Value is greater than string buffer.
	/// </exception>
	public int Length
	{
		get => length;
		set
		{
			Guard.IsLessThanOrEqualTo(value, buffer.Length);

			length = value;
		}
	}

	/// <summary>
	/// <see langword="true"/> when <see cref="Length"/> is 0; otherwise, <see langword="false"/>.
	/// </summary>
	public bool IsEmpty => length == 0;

	/// <summary>
	/// <see langword="true"/> if string consists exclusively of white-space characters or empty; otherwise, <see langword="false"/>.
	/// </summary>
	public bool IsWhiteSpace
	{
		get
		{
			for (int i = 0; i < length; i++)
			{
				if (!char.IsWhiteSpace(buffer[i]))
				{
					return false;
				}
			}

			return true;
		}
	}

	/// <summary>
	/// Initialize empty <see cref="MutableString"/> instance.
	/// </summary>
	public MutableString()
	{
		buffer = new char[DefaultCapacity];
		length = 0;
	}

	/// <summary>
	/// Initialize empty <see cref="MutableString"/> instance with specified <paramref name="capacity"/>.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is negative or zero.</exception>
	public MutableString(int capacity)
	{
#if NET8_0_OR_GREATER
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);
#else
		if (capacity <= 0)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(nameof(capacity));
		}
#endif

		buffer = new char[capacity];
		length = 0;
	}

	/// <summary>
	/// Initialize new <see cref="MutableString"/> instance with specified value.
	/// </summary>
	/// <param name="value">Initial value of new <see cref="MutableString"/>.</param>
	public MutableString(ReadOnlySpan<char> value)
	{
		buffer = new char[length = value.Length];
		value.CopyTo(buffer);
	}

	/// <summary>
	/// Initialize new <see cref="MutableString"/> instance with specified value from array.
	/// </summary>
	/// <param name="value">Initial value of new <see cref="MutableString"/>.</param>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
	public MutableString(char[] value)
	{
#if NET8_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(value);
#else
		Guard.IsNotNull(value);
#endif

		buffer = new char[length = value.Length];

		value.CopyTo(buffer, length);
	}

	/// <summary>
	/// Initialize new <see cref="MutableString"/> instance with specified value from array
	/// with offset and specified length.
	/// </summary>
	/// <param name="value">Initial value of new <see cref="MutableString"/>.</param>
	/// <param name="startIndex">Offset of <paramref name="value"/> to copy.</param>
	/// <param name="length">Length of content to copy.</param>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is out of <paramref name="value"/> bounds.</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is out of <paramref name="value"/> bounds.</exception>
	public MutableString(char[] value, int startIndex, int length)
	{
#if NET8_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(value);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		ArgumentOutOfRangeException.ThrowIfNegative(startIndex);
#else
		Guard.IsNotNull(value);
		Guard.IsGreaterThanOrEqualTo(length, 0);
		Guard.IsGreaterThanOrEqualTo(startIndex, 0);
#endif

		if (startIndex + length > value.Length)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(nameof(length));
		}

		this.length = length;
		this.buffer = new char[length];
		value.CopyTo(this.buffer, startIndex);
	}

	/// <summary>
	/// Get or set character at <paramref name="index"/> of this string.
	/// </summary>
	/// <param name="index">Index to get or set character.</param>
	/// <returns>Character at <paramref name="index"/> within this string.</returns>
	[IndexerName("Chars")]
	public char this[int index]
	{
		get
		{
#if NET8_0_OR_GREATER
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)length);
			ArgumentOutOfRangeException.ThrowIfNegative(index);
#else
			Guard.IsLessThan((uint)index, (uint)length);
			Guard.IsGreaterThanOrEqualTo(index, 0);
#endif
			return buffer[index];
		}
		set
		{
#if NET8_0_OR_GREATER
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)length);
			ArgumentOutOfRangeException.ThrowIfNegative(index);
#else
			Guard.IsLessThan((uint)index, (uint)length);
			Guard.IsGreaterThanOrEqualTo(index, 0);
#endif
			buffer[index] = value;
		}
	}

	/// <summary>
	/// Create new <see cref="MutableString"/> instance with same content.
	/// </summary>
	/// <returns>Copy of current instance.</returns>
	public MutableString Duplicate()
	{
		return new MutableString(this.buffer, 0, this.length);
	}

	/// <inheritdoc/>
	object ICloneable.Clone()
	{
		return Duplicate();
	}

	/// <summary>
	/// Creates a new span over current string.
	/// </summary>
	/// <remarks>
	/// The string length shall not be modified during span lifetime!
	/// </remarks>
	/// <returns>Span instance pointing on internal buffer.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Span<char> AsSpan()
	{
		ref char buffer = ref MemoryMarshal.GetArrayDataReference(this.buffer);
		return MemoryMarshal.CreateSpan(ref buffer, this.length);
	}

	/// <summary>
	/// Copies the current string to the specified destination array.
	/// </summary>
	/// <param name="destination">The array to which the string will be copied.</param>
	/// <exception cref="ArgumentNullException"><paramref name="destination"/> is null.</exception>
	/// <exception cref="ArgumentException">
	/// <paramref name="destination"/> array is not long enough.
	/// </exception>
	public void CopyTo(char[] destination)
	{
		Array.Copy(this.buffer, 0, destination, 0, this.length);
	}

	/// <summary>
	/// Copies the current string to the specified destination array at specified <paramref name="index"/>.
	/// </summary>
	/// <param name="destination">The array to which the string will be copied.</param>
	/// <param name="index">The zero-based index in the array at which storing begins.</param>
	/// <exception cref="ArgumentNullException"><paramref name="destination"/> is null.</exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="index"/> is less than the lower bound of the first dimension of destinationArray.
	/// </exception>
	/// <exception cref="ArgumentException">
	/// Region from <paramref name="index"/> to the end of <paramref name="destination"/> is not long enough.
	/// </exception>
	public void CopyTo(char[] destination, int index)
	{
		Array.Copy(this.buffer, 0, destination, index, this.length - index);
	}

	/// <summary>
	/// Copies the current string to the specified destination array at specified <paramref name="index"/>.
	/// </summary>
	/// <param name="destination">The array to which the string will be copied.</param>
	/// <param name="index">The zero-based index in the array at which storing begins.</param>
	/// <param name="count">The number of characters to copy.</param>
	/// <exception cref="ArgumentNullException"><paramref name="destination"/> is null.</exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="index"/> is less than the lower bound of the first dimension of destinationArray.
	/// -or-
	/// <paramref name="count"/> is less than zero.
	/// </exception>
	/// <exception cref="ArgumentException">
	/// <paramref name="count"/> is greater than the number of elements from <paramref name="index"/> to the end of <paramref name="destination"/>.
	/// </exception>
	public void CopyTo(char[] destination, int index, int count)
	{
		if (count > this.length)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(nameof(count), "Number of copying characters is greater than the string length.");
		}

		Array.Copy(this.buffer, 0, destination, index, count);
	}

	/// <summary>
	/// Copies the current string to the specified destination span.
	/// </summary>
	/// <param name="destination">The span to which the string will be copied.</param>
	/// <exception cref="ArgumentException">
	/// The string length is greater than the available <paramref name="destination"/> memory.
	/// </exception>
	public void CopyTo(Span<char> destination)
	{
		AsSpan().CopyTo(destination);
	}

	/// <summary>
	/// Copies the contents of this string into the destination span.
	/// </summary>
	/// <param name="destination">The span into which to copy this string's contents.</param>
	/// <returns><see langword="true"/> if the data was copied; <see langword="false"/> if the destination was too short to fit the contents of the string.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryCopyTo(Span<char> destination)
	{
		if ((uint)Length <= (uint)destination.Length)
		{
			buffer.CopyTo(destination);
			return true;
		}

		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int GetNewCapacity(int requiredCapacity)
	{
		int newCapacity = buffer.Length == 0 ? DefaultCapacity : 2 * buffer.Length;

		if ((uint)newCapacity > Array.MaxLength) newCapacity = Array.MaxLength;

		if (newCapacity < requiredCapacity) newCapacity = requiredCapacity;

		return newCapacity;
	}

	private void GrowForInsertion(int indexToInsert, int insertionWidth)
	{
		int requiredCapacity = checked(length + insertionWidth);
		int newCapacity = GetNewCapacity(requiredCapacity);

		char[] newBuffer = new char[newCapacity];
		if (indexToInsert != 0)
		{
			Array.Copy(buffer, newBuffer, length: indexToInsert);
		}

		if (length != indexToInsert)
		{
			Array.Copy(buffer, indexToInsert, newBuffer, indexToInsert + insertionWidth, length - indexToInsert);
		}

		buffer = newBuffer;
	}

	#region Standard methods

	/// <inheritdoc/>
	public override bool Equals(object? obj)
	{
		return obj is MutableString other && this.Equals(other)
			|| obj is string str && this.Equals(str);
	}

	/// <summary>
	/// Returns a value indicating whether the characters in this instance
	/// are equal to the characters in a specified string.
	/// </summary>
	/// <param name="other">The string to compare with current instance.</param>
	/// <returns><see langword="true"/> if <paramref name="other"/> content are same; otherwise, <see langword="false"/>.</returns>
	public bool Equals(MutableString? other)
	{
#if NET8_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(other);
#else
		Guard.IsNotNull(other);
#endif

		if (ReferenceEquals(this, other)) return true;
		if (this.length != other.length) return false;

		char[] first = other.buffer;
		char[] second = other.buffer;
		int length = this.length;

		for (int i = 0; i < length; i++)
		{
			if (first[i] != second[i]) return false;
		}

		return true;
	}

	/// <summary>
	/// Returns a value indicating whether the characters in this instance
	/// are equal to the characters in a specified string.
	/// </summary>
	/// <param name="other">The string to compare with current instance.</param>
	/// <returns><see langword="true"/> if <paramref name="other"/> content are same; otherwise, <see langword="false"/>.</returns>
	public bool Equals(string? other)
	{
#if NET8_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(other);
#else
		Guard.IsNotNull(other);
#endif
		return this.Equals(other.AsSpan());
	}

	/// <summary>
	/// Returns a value indicating whether the characters in this instance
	/// are equal to the characters in a specified span.
	/// </summary>
	/// <param name="other">The character span to compare with current instance.</param>
	/// <returns><see langword="true"/> if <paramref name="other"/> content are same; otherwise, <see langword="false"/>.</returns>
	public bool Equals(ReadOnlySpan<char> other)
	{
		if (other.Length != this.length) return false;

		ref char otherReference = ref MemoryMarshal.GetReference(other);
		ref char thisReference = ref MemoryMarshal.GetArrayDataReference(this.buffer); // Must be `ref readonly`, but Unsafe.Add allows only refs
		// ReSharper disable once LocalVariableHidesMember
		int length = this.length;

		for (int i = 0; i < length; i++)
		{
			if (Unsafe.Add(ref otherReference, i) != Unsafe.Add(ref thisReference, i)) return false;
		}

		return true;
	}

	/// <summary>
	/// Copy content of <see cref="MutableString"/> to new immutable <see cref="string"/>.
	/// </summary>
	/// <returns>A new <see cref="string"/> instance with same content.</returns>
	public override string ToString()
	{
		return new string(this.buffer, 0, this.length);
	}

	/// <summary>
	/// Copy content of <see cref="MutableString"/> to new immutable <see cref="string"/> starting at specified <paramref name="index"/>.
	/// </summary>
	/// <param name="index">The zero-based starting index of the string creation.</param>
	/// <returns>A new <see cref="string"/> instance with substring of current mutable string.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="index"/> is negative.
	/// -or-
	/// <paramref name="index"/> is out of string bounds.
	/// </exception>
	public string ToString(int index)
	{
#if NET8_0_OR_GREATER
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, this.length);
#else
		Guard.IsGreaterThanOrEqualTo(index, 0);
		Guard.IsLessThan(index, this.length);
#endif

		return new string(this.buffer, index, this.length - index);
	}

	/// <summary>
	/// Copy content of <see cref="MutableString"/> to new immutable <see cref="string"/> starting at specified <paramref name="index"/> with specified <paramref name="length"/>.
	/// </summary>
	/// <param name="index">The zero-based starting index of the string creation.</param>
	/// <param name="length">The number of characters to copy from the current string.</param>
	/// <returns>A new <see cref="string"/> instance with substring of current mutable string.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="index"/> is negative.
	/// -or-
	/// <paramref name="index"/> is out of string bounds.
	/// -or-
	/// <paramref name="length"/> is negative.
	/// </exception>
	/// <exception cref="ArgumentException">
	/// Substring at specified <paramref name="index"/> with specified <paramref name="length"/> is not valid within this string.
	/// </exception>
	public string ToString(int index, int length)
	{
#if NET8_0_OR_GREATER
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, this.length);
#else
		Guard.IsGreaterThanOrEqualTo(index, 0);
		Guard.IsGreaterThanOrEqualTo(length, 0);
		Guard.IsLessThan(index, this.length);
#endif
		if (length > this.length - index)
		{
			ThrowHelper.ThrowArgumentException(nameof(length));
		}

		return new string(this.buffer, index, length);
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		return this.ToString().GetHashCode();
	}

	#endregion

	/// <summary>
	/// Returns a reference to the first character of the internal buffer.
	/// This method is intended to support .NET compilers and is not intended to be called by user code.
	/// </summary>
	/// <returns>A reference to the first character of the internal buffer.</returns>
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public ref char GetPinnableReference()
	{
		return ref AsSpan().GetPinnableReference();
	}

	/// <summary>
	/// Defines an implicit conversion of read-only span of characters to mutable string.
	/// </summary>
	/// <param name="str">A span to implicitly convert.</param>
	public static implicit operator MutableString(ReadOnlySpan<char> str)
	{
		return new MutableString(str);
	}

	/// <summary>
	/// Defines an implicit conversion of immutable string to mutable string.
	/// </summary>
	/// <param name="str">A string to implicitly convert.</param>
	public static implicit operator MutableString(string str)
	{
		return new MutableString(str);
	}
}
