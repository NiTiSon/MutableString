using CommunityToolkit.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NiTiS;

/// <summary>
/// Represents a mutable string of characters.
/// </summary>
[DebuggerDisplay("{ToString()}")]
public sealed partial class MutableString : IEnumerable<char>, IEnumerable, IEquatable<MutableString?>, IEquatable<string?>
{
	private const int DefaultCapacity = 32;

	private char[] buffer;
	private int length;

	/// <summary>
	/// Internal buffer capacity of current <see cref="MutableString"/>.
	/// </summary>
	public int Capacity => buffer.Length;

	/// <summary>
	/// Length of current <see cref="MutableString"/>.
	/// </summary>
	public int Length
	{
		get => length;
		//set
		//{

		//}
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

	//[IndexerName("Chars")]
	//public char this[int index]
	//{
	//	get
	//	{
	//		return buffer[index];
	//	}
	//	set
	//	{
	//		buffer[index] = value;
	//	}
	//}

	/// <summary>
	/// Create new <see cref="MutableString"/> instance with same content.
	/// </summary>
	/// <returns>Copy of current instance.</returns>
	public MutableString Duplicate()
	{
		return new MutableString(this.buffer, 0, this.length);
	}

	// TODO:
	// Replace
	// Shift/Rotate?
	// IndexOf/IndexOfAny
	// StartsWith/EndsWith

	/// <summary>
	/// Appends the string representation of a specified <see cref="char"/> object to this <see cref="MutableString"/> instance.
	/// </summary>
	/// <param name="value"></param>
	/// <returns>A reference to this instance after the append operation is completed.</returns>
	public MutableString Append(char value)
	{
		return Insert(length, value);
	}

	/// <summary>
	/// Appends the string representation of a specified read-only character span to this <see cref="MutableString"/> instance.
	/// </summary>
	/// <param name="value"></param>
	/// <returns>A reference to this instance after the append operation is completed.</returns>
	public MutableString Append(ReadOnlySpan<char> value)
	{
		return Insert(length, value);
	}

	/// <summary>
	/// Insert character in <see cref="MutableString"/> at <paramref name="index"/>.
	/// If required, the string capacity may be increased.
	/// </summary>
	/// <param name="index">Index to insert character.</param>
	/// <param name="value">Character to insert.</param>
	/// <returns>A reference to this instance after the append operation is completed.</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> are out of string bounds.</exception>
	public MutableString Insert(int index, char value)
	{
#if NET8_0_OR_GREATER
		ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)index, (uint)length);
#else
		Guard.IsLessThanOrEqualTo((uint)index, (uint)length);
#endif

		if (length == Capacity)
		{
			GrowForInsertion(index, 1);
		}
		else if (index < length)
		{
			Array.Copy(buffer, index, buffer, index + 1, length - index);
		}

		buffer[index] = value;
		length++;

		return this;
	}

	/// <summary>
	/// Insert string in <see cref="MutableString"/> at <paramref name="index"/>.
	/// If required, the string capacity may be increased.
	/// </summary>
	/// <param name="index">Index to insert string.</param>
	/// <param name="value">String to insert.</param>
	/// <returns>A reference to this instance after the append operation is completed.</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> are out of string bounds.</exception>
	public MutableString Insert(int index, ReadOnlySpan<char> value)
	{
#if NET8_0_OR_GREATER
		ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)index, (uint)length);
#else
		Guard.IsLessThanOrEqualTo((uint)index, (uint)length);
#endif

		int count = value.Length;

		if (count == 0) return this;

		if (Capacity - length < count)
		{
			GrowForInsertion(index, count);
		}
		else if (index < length)
		{
			Array.Copy(buffer, index, buffer, index + count, length - index);
		}

		value.CopyTo(buffer.AsSpan()[index..]);

		length += count;

		return this;
	}

	/// <summary>
	/// Returns a value indicating whether a specified character occurs within this string.
	/// </summary>
	/// <param name="value">The char to seek.</param>
	/// <returns><see langword="true"/> if character contains within string; otherwise, <see langword="false"/>.</returns>
	public bool Contains(char value)
	{
		ref char buffer = ref MemoryMarshal.GetArrayDataReference(this.buffer);
		int length = this.length;
		
		for (int i = 0; i < length; i++)
		{
			if (Unsafe.Add(ref buffer, i) == value) return true;
		}

		return false;
	}

	/// <summary>
	/// Reverse character order of this string.
	/// </summary>
	/// <returns>A reference to this instance after the append operation is completed.</returns>
	public MutableString Reverse()
	{
		Array.Reverse(buffer, 0, length);

		return this;
	}

	/// <summary>
	/// Searches for the first occurrence of a specified character within the current string.
	/// </summary>
	/// <param name="value">The character to locate within the string.</param>
	/// <returns>
	/// The zero-based index of the first occurrence of the specified character if found; otherwise, -1.
	/// </returns>
	public int IndexOf(char value)
	{
		// Array.IndexOf is faster ~1.5 times than usual for loop
		return Array.IndexOf(this.buffer, value, 0);
	}

	/// <summary>
	/// Searches for the first occurrence of a specified character within the current string from specified <paramref name="index"/>.
	/// </summary>
	/// <param name="value">The character to locate within the string.</param>
	/// <param name="index">The zero-based starting index of the search.</param>
	/// <returns>
	/// The zero-based index of the first occurrence of the specified character if found; otherwise, -1.
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="index"/> is outside the range of valid indexes for the string.
	/// </exception>
	public int IndexOf(char value, int index)
	{
		return Array.IndexOf(this.buffer, value, index, this.length - index); 
	}

	/// <summary>
	/// Searches for the first occurrence of a specified character within the specified section of current string.
	/// </summary>
	/// <param name="value">The character to locate within the string.</param>
	/// <param name="index">The zero-based starting index of the search.</param>
	/// <param name="count">The number of elements in the string to search.</param>
	/// <returns>
	/// The zero-based index of the first occurrence of the specified character if found; otherwise, -1.
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="index"/> is outside the range of valid indexes for the string.
	/// -or-
	/// <paramref name="count"/> is less than 0.
	/// -or-
	/// <paramref name="index"/> and <paramref name="count"/> do not specify a valid section in the string.
	/// </exception>
	public int IndexOf(char value, int index, int count)
	{
		return Array.IndexOf(this.buffer, value, index, count);
	}

	/// <summary>
	/// Searches for the first occurrence of a specified substring within the current string.
	/// </summary>
	/// <param name="value">The substring to locate within the string.</param>
	/// <returns>
	/// The zero-based index of the first occurrence of the specified substring if found; otherwise, -1.
	/// If <paramref name="value"/> is an empty span, 0 is returned.
	/// </returns>
	public int IndexOf(ReadOnlySpan<char> value)
	{
		if (value.Length == 0) return 0; // Empty string can be found anywhere, even in other empty string

		ref char buffer = ref MemoryMarshal.GetArrayDataReference(this.buffer);
		ref char other = ref MemoryMarshal.GetReference(value);
		int length = this.length;

		for (int i = 0; i <= length - value.Length; i++)
		{
			if (MemoryMarshal.CreateSpan(ref buffer, length).Slice(i, value.Length).SequenceEqual(value))
			{
				return i;
			}
		}

		return -1;
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
		if (this.length != other!.length) return false;

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
		if (other!.Length != this.length) return false;

		ref char otherReference = ref MemoryMarshal.GetReference(other.AsSpan());
		ref char thisReference = ref MemoryMarshal.GetArrayDataReference(this.buffer); // Must be ref readonly, but Unsafe.Add allows only refs
		int length = this.length;

		for (int i = 0; i < length; i++)
		{
			if (Unsafe.Add(ref otherReference, i) != Unsafe.Add(ref thisReference, i)) return false;
		}

		return true;
	}
	
	/// <inheritdoc/>
	public IEnumerator<char> GetEnumerator()
	{
		return new Enumerator(this);
	}

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	/// <summary>
	/// Copy content of <see cref="MutableString"/> to new immutable <see cref="string"/>.
	/// </summary>
	/// <returns>A new <see cref="string"/> instance with same content.</returns>
	public override string ToString()
	{
		return new string(buffer, 0, length);
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		return this.ToString().GetHashCode();
	}

#endregion
}
