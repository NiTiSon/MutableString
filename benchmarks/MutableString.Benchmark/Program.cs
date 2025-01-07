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
		BenchmarkRunner.Run<Test>();
	}
}

[SimpleJob(RuntimeMoniker.NativeAot90, baseline: true)]
[SimpleJob(RuntimeMoniker.Net90)]
[SimpleJob(RuntimeMoniker.Net60)]
[MemoryDiagnoser]
public class Test
{

}