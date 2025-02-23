using System;

namespace NiTiS;

internal static class SpanExtensions
{
	public static int IndexOfOrdinalIgnoreCase(this ReadOnlySpan<char> str, ReadOnlySpan<char> value)
	{
		if (value.Length == 0)
			return 0; // Empty string is always found at index 0

		if (str.Length < value.Length)
			return -1; // Value cannot be found if it's longer than the string

		for (int i = 0; i <= str.Length - value.Length; i++)
		{
			bool match = true;
			for (int j = 0; j < value.Length; j++)
			{
				if (char.ToUpperInvariant(str[i + j]) != char.ToUpperInvariant(value[j]))
				{
					match = false;
					break;
				}
			}

			if (match)
				return i;
		}

		return -1; // Value not found
	}
}