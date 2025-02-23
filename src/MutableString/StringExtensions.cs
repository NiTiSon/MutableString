namespace NiTiS;

/// <summary>
/// Class containing extensions for both <see cref="MutableString"/> and <see cref="string"/> classes.
/// </summary>
public static class StringExtensions
{
	/// <summary>
	/// Create mutable string from immutable one.
	/// </summary>
	/// <param name="content"><see cref="string"/> instance.</param>
	/// <returns>New <see cref="MutableString"/> instance with content from <see cref="string"/>.</returns>
	public static MutableString ToMutableString(this string content)
	{
		return new MutableString(content);
	}
}