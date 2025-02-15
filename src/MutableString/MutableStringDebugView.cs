namespace NiTiS;

internal sealed class MutableStringDebugView
{
	private readonly MutableString data;

	public MutableStringDebugView(MutableString data)
	{
		this.data = data;
	}

	public int Length => data.Length;
	public int Capacity => data.Capacity;
}