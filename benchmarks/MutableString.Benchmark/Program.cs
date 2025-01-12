using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using NiTiS;
using System;

namespace Benchmark;

public sealed class Program
{
	public static void Main(string[] args)
	{
		BenchmarkRunner.Run<IndexOf_Char_Benchmark>();
	}
}

[SimpleJob(RuntimeMoniker.Net90, baseline: true)]
[SimpleJob(RuntimeMoniker.Net60)]
[MemoryDiagnoser]
public class IndexOf_Char_Benchmark
{
	private MutableString str;

	[GlobalSetup]
	public void Setup()
	{
		str = new("{asdasd0w912kjzxc-0988yiu231hgiu1hxihhjwsajhdgjhsgdjhd}");
	}

	[Benchmark]
	public int IndexOf_X()
	{
		return str.IndexOf('Я');
	}

	//[Benchmark]
	//public int IndexOf_Y()
	//{
	//	return str.IndexOf_Alt('Я');
	//}
}