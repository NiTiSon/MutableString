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

		Assert.AreEqual(s.Length, 4);
		Assert.AreEqual(s.ToString(), "dcba");

		s.Insert(2, '\0');

		Assert.AreEqual(s.Length, 5);
		Assert.AreEqual(s.ToString(), "dc\0ba");

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

		Assert.AreEqual(s.Length, 6);

		s.Insert(1, "\0");

		Assert.AreEqual(s.Length, 7);
		Assert.AreEqual(s.ToString(), "a\0bobus");

		s.Insert(1, "Vore");

		Assert.AreEqual(s.ToString(), "aVore\0bobus");

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
	public void Contains_Char()
	{
		MutableString s = new("Amongus is sus?7");

		Assert.IsTrue(s.Contains('A'));
		Assert.IsTrue(s.Contains('i'));
		Assert.IsFalse(s.Contains('8'));
	}
}
