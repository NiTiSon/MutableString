using NUnit.Framework;
using System;

namespace NiTiS;

internal class InstantiationTests
{
	[Test]
	public void DefaultCtor()
	{
		MutableString str = new();

		Assert.IsTrue(str.Length == 0);
	}

	[Test]
	public void SpanCtor()
	{
		ReadOnlySpan<char> text = "Hello World";

		MutableString str = new(text);

		Assert.AreEqual(text.Length, str.Length);
		Assert.AreEqual(text.ToString(), str.ToString());
	}
}
