using System.Buffers;

namespace NiTiS;

public partial class MutableString
{
	#if false
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
}