using NUnit.Framework;
using System;

namespace NiTiS;

internal sealed class MutableStringTest
{
	[Test]
	public void Reverse()
	{
		MutableString str = new();

		Assert.DoesNotThrow(() => str.Reverse());

		MutableString str2 = new("12345");

		str2.Reverse();

		Assert.AreEqual("54321", str2.ToString());
	}

	[Test]
	public void StartsWith()
	{
		MutableString s = new("t23123123zxc981231z");

		Assert.IsTrue(s.StartsWith('t'));
		Assert.IsTrue(s.StartsWith("t"));
		Assert.IsTrue(s.StartsWith("t2312"));
		Assert.IsFalse(s.StartsWith("t2312я"));

		Assert.IsTrue(s.StartsWith(""));
	}

	[Test]
	public void EndsWith()
	{
		MutableString s = new("t23123123zxc981231z");

		Assert.IsTrue(s.EndsWith('z'));
		Assert.IsTrue(s.EndsWith("z"));
		Assert.IsTrue(s.EndsWith("31z"));
		Assert.IsFalse(s.EndsWith("31z2"));

		Assert.IsTrue(s.EndsWith(""));
	}

	[Test]
	public void Remove()
	{
		MutableString str = new("12345");

		str.Remove(2, 2);
		Assert.AreEqual("125", str.ToString());


		str.Remove(0, 1);
		Assert.AreEqual("25", str.ToString());


		str.Remove(str.Length - 1, 1);
		Assert.AreEqual("2", str.ToString());

		Assert.Throws<ArgumentException>(() =>
		{
			str.Remove(1, 1);
		});

		Assert.Throws<ArgumentOutOfRangeException>(() =>
		{
			str.Remove(0, 0);
		});

		Assert.Throws<ArgumentOutOfRangeException>(() =>
		{
			str.Remove(-1, 1);
		});
	}

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

	[Test]
	public void Contains_ReadOnlySpan()
	{
		MutableString s = new("Amongus is sus?7");

		Assert.IsFalse(s.Contains("Xi"));
		Assert.IsFalse(s.Contains("Ox120-312-30912-3012-03912-312"));
		Assert.IsTrue(s.Contains("sus"));
	}

	[Test]
	public void IndexOf_Char()
	{
		MutableString s = new("Amongus is sus?7");

		Assert.AreEqual(0, s.IndexOf('A'));
		Assert.AreEqual(s.Length - 1, s.IndexOf('7'));
		Assert.AreEqual(-1, s.IndexOf('Я'));
	}

	[Test]
	public void IndexOf_Char2()
	{
		MutableString s = new("Amongus is sus?7");

		Assert.AreEqual(0, s.IndexOf('A', 0, 1));
		Assert.AreEqual(-1, s.IndexOf('7', 0, 1));
		Assert.AreEqual(-1, s.IndexOf('Я', 0, 1));

		Assert.AreEqual(6, s.IndexOf('s', 0));
		Assert.AreEqual(9, s.IndexOf('s', 7));
		Assert.AreEqual(11, s.IndexOf('s', 10, 2));
		Assert.AreEqual(13, s.IndexOf('s', 12, 2));

		Assert.Throws<ArgumentOutOfRangeException>(() =>
		{
			s.IndexOf('x', -1);
		});

		Assert.Throws<ArgumentOutOfRangeException>(() =>
		{
			s.IndexOf('x', -1, 0);
		});

		Assert.Throws<ArgumentOutOfRangeException>(() =>
		{
			s.IndexOf('x', 0, -1);
		});

		Assert.AreEqual(-1, s.IndexOf('Я', 0, 0));
	}

	[Test]
	public void IndexOf_ReadOnlySpan()
	{
		MutableString s = new("Amongus is sus?7");

		Assert.AreEqual(0, s.IndexOf("Amongus"));
		Assert.AreEqual(8, s.IndexOf("is"));
		Assert.AreEqual(-1, s.IndexOf('Я'));

		Assert.AreEqual(-1, s.IndexOf("as;dskjalkdjalsjdalskjdlaskjdalsk"));
	}

	[Test]
	public void IndexOf_ReadOnlySpan2()
	{
		MutableString s = new("Amongus is sus?7");

		Assert.AreEqual(0, s.IndexOf("A", 0, 1));
		Assert.AreEqual(-1, s.IndexOf("7", 0, 1));
		Assert.AreEqual(-1, s.IndexOf("Я", 0, 1));

		Assert.AreEqual(-1, s.IndexOf("sus?71221-1203-12", 0, s.Length));

		Assert.AreEqual(6, s.IndexOf("s", 0));
		Assert.AreEqual(9, s.IndexOf("s", 7));
		Assert.AreEqual(11, s.IndexOf("s", 10, 2));
		Assert.AreEqual(13, s.IndexOf("s", 12, 2));
		Assert.AreEqual(13, s.IndexOf("s", 12, 3));
		Assert.AreEqual(-1, s.IndexOf("s", 12, 1));

		Assert.AreEqual(2, s.IndexOf("", 2, 0)); // Empty strings are always found

		Assert.Throws<ArgumentOutOfRangeException>(() =>
		{
			s.IndexOf("x", -1);
		});

		Assert.Throws<ArgumentOutOfRangeException>(() =>
		{
			s.IndexOf("x", -1, 0);
		});

		Assert.Throws<ArgumentOutOfRangeException>(() =>
		{
			s.IndexOf("x", 0, -1);
		});

		Assert.AreEqual(-1, s.IndexOf("Я", 0, 0));

		Assert.AreEqual(0, s.IndexOf("", 0, 0));
	}

	[Test]
	public void IndexOfAny()
	{
		MutableString str = "Kzwstne";

		Assert.AreEqual(2, str.IndexOfAny('x', 'w', 'e'));
		Assert.AreEqual(6, str.IndexOfAny('x', 'e'));

		Assert.AreEqual(-1, str.IndexOfAny());
	}

	[Test]
	public void CopyTo()
	{
		MutableString s = new("1234567890X");
		char[] dst = new char[10];
		char[] dst2 = new char[11];

		Assert.Throws<ArgumentException>(() => // Not long enough
		{
			s.CopyTo(dst);
		});

		Assert.Throws<ArgumentOutOfRangeException>(() =>
		{
			s.CopyTo(dst2, -1);
		});

		Assert.Throws<ArgumentOutOfRangeException>(() =>
		{
			s.CopyTo(dst2, 0, 12);
		});
	}

	[Test]
	public void AsSpan()
	{
		MutableString s = new("Hello world");

		Assert.AreEqual(s.Length, s.AsSpan().Length);

		Assert.IsTrue(s.Equals(s.AsSpan()));
		Assert.IsFalse(s.Equals(s.AsSpan()[..1]));
	}

	[Test]
	public void Chars()
	{
		MutableString s = new("xyz");

		Assert.AreEqual('x', s[0]);
		Assert.AreEqual('y', s[1]);
		Assert.AreEqual('z', s[2]);

		Assert.Throws<ArgumentOutOfRangeException>(() =>
		{
			_ = s[-1];
		});


		Assert.Throws<ArgumentOutOfRangeException>(() =>
		{
			_ = s[s.Length];
		});
	}

	[Test]
	public new void ToString()
	{
		MutableString str = new("abc345вгд");

		Assert.AreEqual("345вгд", str.ToString(3));
		Assert.AreEqual("345", str.ToString(3, 3));
		Assert.AreEqual("вгд", str.ToString(6));
		Assert.AreEqual("вгд", str.ToString(6, 3));

		Assert.AreEqual(str.ToString(), str.ToString(0, str.Length));
	}

	[Test]
	public unsafe void Fixed()
	{
		MutableString str = "Hello";

		fixed (char* pStr = str)
		{
			pStr[0] = 'a';
			pStr[str.Length - 1] = 'w';
		}

		Assert.AreEqual("aellw", str.ToString());
	}
}