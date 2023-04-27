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

GPU gpu = new();

// run benchmarks
//BenchmarkRunner.Run<Benchmark>();

Vector a = new Vector(gpu, new float[] {6f,3f,2f});
Vector b = Vector.test(a);
b.Print();


// Console.WriteLine($"MathF: {MathF.Sqrt(1.2134f)}");
// Console.WriteLine($"My: {TestCls.newSQRT(1.2134f)}");

// write an extremely optimised function for computing the sqrt of an integer between in the range 1 <= x < 2


Console.Read();
