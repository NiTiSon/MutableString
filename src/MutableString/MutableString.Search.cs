using CommunityToolkit.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;
using System.Buffers;
using System.Globalization;
using System.Diagnostics;

namespace NiTiS;

public partial class MutableString
{
#if NET9_0_OR_GREATER
	internal static class SearchValuesStorage
	{
		/// <summary>
		/// SearchValues would use SpanHelpers.IndexOfAnyValueType for 5 values in this case.
		/// No need to allocate the SearchValues as a regular Span.IndexOfAny will use the same implementation.
		/// </summary>
		public const string NewLineCharsExceptLineFeed = "\r\f\u0085\u2028\u2029";

		/// <summary>
		/// The Unicode Standard, Sec. 5.8, Recommendation R4 and Table 5-2 state that the CR, LF,
		/// CRLF, NEL, LS, FF, and PS sequences are considered newline functions. That section
		/// also specifically excludes VT from the list of newline functions, so we do not include
		/// it in the needle list.
		/// </summary>
		public static readonly SearchValues<char> NewLineChars =
			SearchValues.Create(NewLineCharsExceptLineFeed + "\n");
	}
#endif

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
	/// Searches for the first occurrence of a specified character within the specified section of current string.
	/// </summary>
	/// <param name="value">The character to locate within the string.</param>
	/// <param name="comparisonType"></param>
	/// <returns>
	/// The zero-based index of the first occurrence of the specified character if found; otherwise, -1.
	/// </returns>
	/// <exception cref="ArgumentException"><paramref name="comparisonType"/> is invalid.</exception>
	public int IndexOf(char value, StringComparison comparisonType)
	{
		switch (comparisonType)
		{
			case StringComparison.CurrentCulture:
			case StringComparison.CurrentCultureIgnoreCase:
				return CultureInfo.CurrentCulture.CompareInfo.IndexOf(AsSpan(), [value], GetCaseCompareOfComparisonCulture(comparisonType));

			case StringComparison.InvariantCulture:
			case StringComparison.InvariantCultureIgnoreCase:
				return CultureInfo.InvariantCulture.CompareInfo.IndexOf(AsSpan(), [value], GetCaseCompareOfComparisonCulture(comparisonType));

			case StringComparison.Ordinal:
				return IndexOf(value);

			case StringComparison.OrdinalIgnoreCase:
				if (value.IsAsciiLetter())
				{
					char valueUc = (char)(value | 0x20);
					char valueLc = (char)(value & ~0x20);

					return AsSpan().IndexOfAny(valueUc, valueLc);
				}
				else if (char.IsAscii(value))
				{
					return AsSpan().IndexOf(value);
				}
				else
				{
					char valueUc = char.ToUpper(value);
					char valueLc = char.ToLower(value);

					return AsSpan().IndexOfAny(valueUc, valueLc);
				}

			default:
				ThrowHelper.ThrowArgumentException(nameof(comparisonType), "Not supported string comparison value.");
				return -1;
		}
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

	internal static CompareOptions GetCaseCompareOfComparisonCulture(StringComparison comparisonType)
	{
		Debug.Assert((uint)comparisonType <= (uint)StringComparison.OrdinalIgnoreCase);

		// Culture enums can be & with CompareOptions.IgnoreCase 0x01 to extract if IgnoreCase or CompareOptions.None 0x00
		//
		// CompareOptions.None                          0x00
		// CompareOptions.IgnoreCase                    0x01
		//
		// StringComparison.CurrentCulture:             0x00
		// StringComparison.InvariantCulture:           0x02
		// StringComparison.Ordinal                     0x04
		//
		// StringComparison.CurrentCultureIgnoreCase:   0x01
		// StringComparison.InvariantCultureIgnoreCase: 0x03
		// StringComparison.OrdinalIgnoreCase           0x05

		return (CompareOptions)((int)comparisonType & (int)CompareOptions.IgnoreCase);
	}
}