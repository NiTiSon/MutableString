#if NET9_0_OR_GREATER
using NUnit.Framework;

namespace NiTiS;

internal sealed class ParseTests
{
	[Test]
	public void ParseUtf8()
	{
		byte[] hello = [0xd0, 0x9f, 0xd1, 0x80, 0xd0, 0xb8, 0xd0, 0xb2, 0xd0, 0xb5, 0xd1, 0x82];

		Assert.AreEqual("Привет", MutableString.Parse(hello, null));
	}

	[Test]
	public void TryParseUtf8()
	{
		byte[] hello = [0xd0, 0x9f, 0xd1, 0x80, 0xd0, 0xb8, 0xd0, 0xb2, 0xd0, 0xb5, 0xd1, 0x82];

		Assert.IsTrue(MutableString.TryParse(hello, null, out _));
	}

	[Test]
	public void ParseString()
	{
		Assert.AreEqual("Привет", MutableString.Parse("Привет", null));
		Assert.AreNotEqual("Привет", MutableString.Parse("привет", null));
	}

	[Test]
	public void TryParseString()
	{
		Assert.IsTrue(MutableString.TryParse("", null, out _));
		Assert.IsFalse(MutableString.TryParse(s: null, null, out _));
	}
}
#endif