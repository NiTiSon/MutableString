namespace NiTiS;

internal sealed class MutableStringDebugView
{
	private readonly MutableString data;
	public MutableStringDebugView(MutableString data)
	{
		this.data = data;
	}

	public int Length { get { return data.Length; } }
	public int Capacity { get { return data.Capacity; } }
}