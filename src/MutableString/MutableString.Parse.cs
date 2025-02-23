#if NET9_0_OR_GREATER
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Unicode;

namespace NiTiS;


public partial class MutableString : IParsable<MutableString>, ISpanParsable<MutableString>, IUtf8SpanParsable<MutableString>
{
	/// <inheritdoc/>
	public static MutableString Parse(string s, IFormatProvider? provider)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc/>
	public static MutableString Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
	{
		return new(s);
	}

	/// <inheritdoc/>
	public static unsafe MutableString Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
	{
		int width = Encoding.UTF8.GetCharCount(utf8Text);

		MutableString result = new(width)
		{
			length = width
		};

		Utf8.ToUtf16(utf8Text, result.AsSpan(), out _, out _);

		return result;
	}

	/// <inheritdoc/>
	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out MutableString result)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc/>
	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out MutableString result)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc/>
	public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, [MaybeNullWhen(false)] out MutableString result)
	{
		int width = Encoding.UTF8.GetCharCount(utf8Text);

		result = new(width)
		{
			length = width
		};

		Utf8.ToUtf16(utf8Text, result.AsSpan(), out _, out _);

		return true;
	}
}

#endif