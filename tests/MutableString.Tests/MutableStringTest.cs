using NUnit.Framework;
using System;

namespace NiTiS;

internal class MutableStringTest
{
	[Test]
	public void Insert_Char()
	{
		MutableString s = new(capacity: 1);

		s.Insert(0, 'a');
		s.Insert(0, 'b');
		s.Insert(0, 'c');
		s.Insert(0, 'd');

		Assert.AreEqual(4, s.Length);
		Assert.AreEqual("dcba", s.ToString());

		s.Insert(2, '\0');

		Assert.AreEqual(5, s.Length);
		Assert.AreEqual("dc\0ba", s.ToString());

		Assert.Throws<ArgumentOutOfRangeException>(() =>
		{
			s.Insert(-1, 'A');
		});

		Assert.DoesNotThrow(() =>
		{
			s.Insert(s.Length, 'A');
		});

		Assert.Throws<ArgumentOutOfRangeException>(() =>
		{
			s.Insert(s.Length + 1, 'A');
		});
	}

	[Test]
	public void Insert_ReadOnlySpan()
	{
		MutableString s = new(capacity: 1);

		s.Insert(0, "abobus");

		Assert.AreEqual(6, s.Length);

		s.Insert(1, "\0");

		Assert.AreEqual(7, s.Length);
		Assert.AreEqual("a\0bobus", s.ToString());

		s.Insert(1, "Vore");

		Assert.AreEqual("aVore\0bobus", s.ToString());

		Assert.Throws<ArgumentOutOfRangeException>(() =>
		{
			s.Insert(-1, "A");
		});

		Assert.DoesNotThrow(() =>
		{
			s.Insert(s.Length, "A");
		});

		Assert.Throws<ArgumentOutOfRangeException>(() =>
		{
			s.Insert(s.Length + 1, "A");
		});
	}

	[Test]
	public void Append_Char()
	{
		MutableString s = new("Xyዱ");

		s.Append('x');

		Assert.AreEqual("Xyዱx", s.ToString());
	}

	[Test]
	public void Append_ReadOnlySpan()
	{
		MutableString s = new("qwăe");

		s.Append("oooK");

		Assert.AreEqual("qwăeoooK", s.ToString());
	}

	[Test]
	public void Contains_Char()
	{
		MutableString s = new("Amongus is sus?7");

		Assert.IsTrue(s.Contains('A'));
		Assert.IsTrue(s.Contains('i'));
		Assert.IsFalse(s.Contains('8'));
	}
}
