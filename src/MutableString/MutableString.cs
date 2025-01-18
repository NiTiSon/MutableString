using CommunityToolkit.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NiTiS;

/// <summary>
/// Represents a mutable string of characters.
/// </summary>
[DebuggerDisplay("{ToString()}")]
[DebuggerTypeProxy(typeof(MutableStringDebugView))]
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
	public bool IsEmpty
	{
		get => length == 0;
	}

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

	/// <summary>
	/// Determines whether this string instance starts with the specified character.
	/// </summary>
	/// <param name="value">The character to compare.</param>
	/// <returns><see langword="true"/> if <paramref name="value"/> matches the beginning of this string; otherwise, <see langword="false"/>.</returns>
	public bool StartsWith(char value)
	{
		return this.buffer.Length > 0 && this.buffer[0] == value;
	}

	/// <summary>
	/// Determines whether the beginning of this string instance matches the specified span.
	/// </summary>
	/// <param name="value">The span to compare.</param>
	/// <returns><see langword="true"/> if <paramref name="value"/> matches the beginning of this string; otherwise, <see langword="false"/>.</returns>
	public bool StartsWith(ReadOnlySpan<char> value)
	{
		if (value.Length <= this.length)
		{
			return IndexOf(value, 0, value.Length) == 0;
		}

		return false;
	}

	/// <summary>
	/// Determines whether the end of this string instance matches the specified character.
	/// </summary>
	/// <param name="value">The character to compare to the character at the end of this instance.</param>
	/// <returns><see langword="true"/> if <paramref name="value"/> matches the end of this instance; otherwise, <see langword="false"/>.</returns>
	public bool EndsWith(char value)
	{
		return this.buffer.Length > 0 && this.buffer[^1] == value;
	}


	/// <summary>
	/// Determines whether the end of this string instance matches the specified span.
	/// </summary>
	/// <param name="value">The span to compare to the substring at the end of this instance.</param>
	/// <returns><see langword="true"/> if <paramref name="value"/> matches the end of this instance; otherwise, <see langword="false"/>.</returns>
	public bool EndsWith(ReadOnlySpan<char> value)
	{
		if (value.Length <= this.length)
		{
			int requiredIndex = this.length - value.Length;
			return IndexOf(value, requiredIndex, value.Length) == requiredIndex;
		}

		return false;
	}

	/// <summary>
	/// Appends the string representation of a specified <see cref="char"/> object to this <see cref="MutableString"/> instance.
	/// </summary>
	/// <param name="value">Character to append to this string.</param>
	/// <returns>A reference to this instance after the append operation is completed.</returns>
	public MutableString Append(char value)
	{
		return Insert(length, value);
	}

	/// <summary>
	/// Appends the string representation of a specified read-only character span to this <see cref="MutableString"/> instance.
	/// </summary>
	/// <param name="value">String span to append to this string.</param>
	/// <returns>A reference to this instance after the append operation is completed.</returns>
	public MutableString Append(ReadOnlySpan<char> value)
	{
		return Insert(length, value);
	}

	/// <summary>
	/// Remove specified region in <see cref="MutableString"/>.
	/// </summary>
	/// <param name="index">Index to remove characters.</param>
	/// <param name="length">Length of removed characters.</param>
	/// <returns>A reference to this instance after the remove operation is completed.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="index"/> is negative.
	/// -or-
	/// <paramref name="length"/> is negative or zero.
	/// </exception>
	/// <exception cref="ArgumentException">
	/// <paramref name="length"/> is greater than the number of elements from <paramref name="index"/> to the end of this string.
	/// </exception>
	public MutableString Remove(int index, int length)
	{
#if NET8_0_OR_GREATER
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);
#else
		Guard.IsGreaterThanOrEqualTo(index, 0);
		Guard.IsGreaterThan(length, 0);
#endif
		int lastRemoveIndex = index + length;

		if (lastRemoveIndex > this.length)
		{
			ThrowHelper.ThrowArgumentException(nameof(length), "length and index do not present valid region within string.");
		}

		if (lastRemoveIndex != this.length) // Remove region is in between or at begin
		{
			Array.Copy(this.buffer, lastRemoveIndex, this.buffer, index, this.length - lastRemoveIndex);
		}

		this.length -= length;

		return this;
	}

	/// <summary>
	/// Replace all occurrences of a specified Unicode character in the instance to another specified Unicode character.
	/// </summary>
	/// <param name="oldChar">The Unicode character to be replaced.</param>
	/// <param name="newChar">The Unicode character to replace all occurrences of <paramref name="oldChar"/>.</param>
	/// <returns>A reference to this instance after the replace operation is completed.</returns>
	public MutableString Replace(char oldChar, char newChar)
	{
		if (oldChar == newChar) return this;

		ref char buffer = ref MemoryMarshal.GetArrayDataReference(this.buffer);

		for (int i = 0; i < length; i++)
		{
			if (Unsafe.Add(ref buffer, i) == oldChar)
			{
				Unsafe.Add(ref buffer, i) = newChar;
			}
		}

		return this;
	}

	/// <summary>
	/// Replace all occurrences of a specified Unicode character sequence in the instance to another specified Unicode character sequence.
	/// </summary>
	/// <param name="oldValue">The string span to be replaced.</param>
	/// <param name="newValue">The string span to replace all occurrences of <paramref name="oldValue"/>.</param>
	/// <returns>A reference to this instance after the replace operation is completed.</returns>
	public MutableString Replace(ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue)
	{
		if (oldValue.SequenceEqual(newValue)) return this;
		if (oldValue.IsEmpty) return this;

		int index = 0;
		while (index < this.length)
		{
			index = IndexOf(oldValue, index);

			if (index < 0) break;

			
			if (oldValue.Length > newValue.Length)
			{
				int diff = oldValue.Length - newValue.Length;
				Array.Copy(this.buffer, index + diff, this.buffer, index, this.length - index);
				this.length -= diff;

				newValue.CopyTo(this.buffer.AsSpan()[index..]);
				index += newValue.Length;
			}
			else if (oldValue.Length < newValue.Length)
			{
				int diff = newValue.Length - oldValue.Length;
				if (this.buffer.Length < this.length + diff)
				{
					GrowForInsertion(index + oldValue.Length, diff); // Extending buffer
				}
				else
				{
					Array.Copy(this.buffer, index + oldValue.Length, this.buffer, index + newValue.Length, this.Length - index - oldValue.Length);
				}
				this.length += diff;

				newValue.CopyTo(this.buffer.AsSpan()[index..]);
				index += newValue.Length;
			}
			else
			{
				newValue.CopyTo(this.buffer.AsSpan()[index..]);
				index += newValue.Length;
			}
		}

		return this;
	}

	/// <summary>
	/// Insert character in <see cref="MutableString"/> at specified <paramref name="index"/>.
	/// If required, the string capacity may be increased.
	/// </summary>
	/// <param name="index">Index to insert character.</param>
	/// <param name="value">Character to insert.</param>
	/// <returns>A reference to this instance after the insert operation is completed.</returns>
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
	/// <returns>A reference to this instance after the insert operation is completed.</returns>
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
	/// Returns a value indicating whether a specified substring occurs within this string.
	/// </summary>
	/// <param name="value">The substring to seek.</param>
	/// <returns><see langword="true"/> if substring contains within string; otherwise, <see langword="false"/>.</returns>
	public bool Contains(ReadOnlySpan<char> value)
	{
		return IndexOf(value) != -1;
	}

	/// <summary>
	/// Reverse character order of this string.
	/// </summary>
	/// <returns>A reference to this instance after the reverse operation is completed.</returns>
	public MutableString Reverse()
	{
		Array.Reverse(buffer, 0, length);

		return this;
	}

	/// <summary>
	/// Set string length to 0 (zero).
	/// </summary>
	/// <returns>A reference to this instance after the clear operation is completed.</returns>
	public MutableString Clear()
	{
		length = 0;
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

	/// <summary>
	/// Searches for the first occurrence of a specified substring within the current string from specified <paramref name="index"/>.
	/// </summary>
	/// <param name="value">The character to locate within the string.</param>
	/// <param name="index">The zero-based starting index of the search.</param>
	/// <returns>
	/// The zero-based index of the first occurrence of the specified substring if found; otherwise, -1.
	/// If <paramref name="value"/> is an empty span, 0 is returned.
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="index"/> is outside the range of valid indexes for the string.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int IndexOf(ReadOnlySpan<char> value, int index)
	{
		if (this.length <= index)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(nameof(index), "Index is out of string bounds.");
		}
		return IndexOf(value, index, this.length - index);
	}

	/// <summary>
	/// Searches for the first occurrence of a specified substring within the specified section of current string.
	/// </summary>
	/// <param name="value">The substring to locate within the string.</param>
	/// <param name="index">The zero-based starting index of the search.</param>
	/// <param name="count">The number of elements in the string to search.</param>
	/// <returns>
	/// The zero-based index of the first occurrence of the specified substring if found; otherwise, -1.
	/// If <paramref name="value"/> is an empty span, 0 is returned.
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="index"/> is outside the range of valid indexes for the string.
	/// -or-
	/// <paramref name="count"/> is less than 0.
	/// -or-
	/// <paramref name="index"/> and <paramref name="count"/> do not specify a valid section in the string.
	/// </exception>
	public int IndexOf(ReadOnlySpan<char> value, int index, int count)
	{
		if (index < 0 /* || index >= this.length */)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(nameof(index), "Index is outside the range of valid indexes for the string.");
		}
		if (count < 0)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(nameof(count), "Count must be greater or equal 0 (zero).");
		}
		if (index + count > this.length)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(nameof(count), "Index and count do not specify a valid section in the string.");
		}

		if (value.Length == 0) return index;

		ref char buffer = ref MemoryMarshal.GetArrayDataReference(this.buffer);
		ref char other = ref MemoryMarshal.GetReference(value);
		int length = this.length;

		for (int i = index; i <= index + count - value.Length; i++)
		{
			if (MemoryMarshal.CreateSpan(ref buffer, length).Slice(i, value.Length).SequenceEqual(value))
			{
				return i;
			}
		}

		return -1;
	}

	/// <summary>
	/// Reports the zero-based index of the first occurrence in this instance of any character in a specified span of Unicode characters.
	/// </summary>
	/// <param name="values">A Unicode character span containing one or more characters to seek.</param>
	/// <returns>
	/// The zero-based index position of the first occurrence in this instance where any character in anyOf was found; -1 if no character in anyOf was found.
	/// </returns>
	public int IndexOfAny(params ReadOnlySpan<char> values)
	{
		return MemoryMarshal.CreateReadOnlySpan(ref MemoryMarshal.GetArrayDataReference(this.buffer), Length).IndexOfAny(values);
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
		return this.Equals(other!.AsSpan());
	}

	/// <summary>
	/// Returns a value indicating whether the characters in this instance
	/// are equal to the characters in a specified span.
	/// </summary>
	/// <param name="other">The character span to compare with current instance.</param>
	/// <returns><see langword="true"/> if <paramref name="other"/> content are same; otherwise, <see langword="false"/>.</returns>
	public bool Equals(ReadOnlySpan<char> other)
	{
		if (other!.Length != this.length) return false;

		ref char otherReference = ref MemoryMarshal.GetReference(other);
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
