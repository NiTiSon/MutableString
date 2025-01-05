using System.Collections;
using System.Collections.Generic;

namespace NiTiS;

public partial class MutableString
{
	private sealed class Enumerator : IEnumerator<char>
	{
		private MutableString instance;
		private int index;

		public Enumerator(MutableString instance)
		{
			this.instance = instance;
			index = -1;
		}

		public char Current => instance.buffer[index];

		object IEnumerator.Current => Current;

		public void Dispose()
		{
			instance = null!;
		}

		public bool MoveNext()
		{
			int index = this.index + 1;
			int length = instance.Length;

			if (index < length)
			{
				this.index = index;
				return true;
			}

			this.index = length;
			return false;
		}

		public void Reset()
		{
			index = -1;
		}
	}
}