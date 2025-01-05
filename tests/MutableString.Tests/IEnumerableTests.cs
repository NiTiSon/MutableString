using NUnit.Framework;

namespace NiTiS;

internal class IEnumerableTests
{
	[Test]
	public void Valid()
	{
		MutableString str = new("Nya");

		int index = 0;
		foreach (char item in str)
		{
			switch (index)
			{
				case 0: Assert.AreEqual('N', item); break;
				case 1: Assert.AreEqual('y', item); break;
				case 2: Assert.AreEqual('a', item); break;
				default: Assert.Fail(); break;
			}

			index++;
		}

		Assert.AreEqual(3, index);
	}
}