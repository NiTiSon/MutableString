using CommunityToolkit.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;

namespace NiTiS;

public partial class MutableString
{
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
}