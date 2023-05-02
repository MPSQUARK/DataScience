using System;
using BenchmarkDotNet.Running;
using BAVCL;
using BAVCL.Core;
using BAVCL.Geometric;
using System.Threading.Tasks;
using Testing_Console;
using System.Diagnostics;
using ILGPU.Runtime;
using ILGPU;
using BAVCL.Extensions;
using BAVCL.Experimental;
using ILGPU.Backends;
using FSL;

void addfunc(float x, float y)
{
	Console.WriteLine(x + y);
}

//GPU gpu = new();
KernelModule.kernels.Add(0, KernelModule.CompiledKernel.NewFloatFloat(new Action<float,float>(addfunc)));
Console.WriteLine(KernelModule.kernels.Count);

Console.WriteLine(KernelModule.getKernel(0));
// if (KernelModule.kernels.TryGetValue(0, out KernelModule.CompiledKernel kern))

// {
// 	if (kern is KernelModule.CompiledKernel.FloatFloat floatKernel)
// 	{
// 		floatKernel.action(1.0f, 2.0f); // Example usage
// 	}
// }

// run benchmarks
//BenchmarkRunner.Run<Benchmark>();

// Console.WriteLine($"MathF: {MathF.Sqrt(1.2134f)}");
// Console.WriteLine($"My: {TestCls.newSQRT(1.2134f)}");

// write an extremely optimised function for computing the sqrt of an integer between in the range 1 <= x < 2


Console.Read();
