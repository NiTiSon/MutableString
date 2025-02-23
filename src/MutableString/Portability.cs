namespace NiTiS;

internal static class Portability
{
	public static bool IsAsciiLetter(this char c) => (uint)((c | 0x20) - 'a') <= 'z' - 'a';
}