using BAVCL;
using BenchmarkDotNet.Attributes;
using System;
using System.Linq;
using ILGPU.Algorithms;
using System.Threading.Tasks;
using BAVCL.Geometric;
using System.Collections.Generic;
using ILGPU;
using BAVCL.Experimental;
using ILGPU.Runtime;

namespace Testing_Console
{
	[MemoryDiagnoser]
	public class Benchmark
	{
		[Params(10)]
		public float val;
		
		[Benchmark(Baseline = true)]
		public void MathSqrt()
		{
			MathF.Sqrt(val);
		}
		
		[Benchmark]
		public void MySqrt()
		{
			TestCls.newSQRT(val);
		}
		
		[Benchmark]
		public void MyOldSqrt()
		{
			TestCls.sqrt_acc_v2(val);
		}

		// [Params(10, 100, 1000,10000)]
		// public int datalength;

		// GPU gpu;
		// float[] data;
		// Random rnd;

		// [GlobalSetup]
		// public void GlobalSetup()
		// {
		// 	gpu = new();
		// 	rnd = new(4522156);
		// 	data = new float[datalength];
		// 	for (int i = 0; i < data.Length; i++)
		// 		data[i] = rnd.NextSingle();
		// }

		// [Benchmark]
		// public void Create()
		// {
		// 	Vector newVec = new Vector(gpu, data);
		// }

		//[Params(2, 8, 16, 49, 100, 64, 256, 529, 165518, 5131, 123, 71645, 12.1518f)]
		//public float val;
		// public static GPU gpu = new();
		// public static MemoryBuffer1D<float, Stride1D.Dense> input = Vector.Arange(gpu, 0, 1_000_000, 1).GetBuffer();
		// public static int len = (int)input.Length;

		// public static MemoryBuffer1D<float, Stride1D.Dense> output = Vector.Fill(gpu, 0, len).GetBuffer();

		// public static float[] vals = input.GetAsArray1D();
		// public static float[] _output = new float[len];

		// [Benchmark]
		// public void XMathSqrt()
		// {
		//     gpu.TestSQRTKernel(gpu.accelerator.DefaultStream, output.IntExtent, output.View, input.View);
		//     gpu.accelerator.Synchronize();
		// }


		// [Benchmark]
		// public void MySqrt()
		// {
		//     gpu.TestMYSQRTKernel(gpu.accelerator.DefaultStream, output.IntExtent, output.View, input.View);
		//     gpu.accelerator.Synchronize();
		// }


		// [Benchmark]
		// public void MySQRT()
		// {
		//     TestCls.sqrtarr(len, _output, vals);
		// }


		//[Benchmark]
		//public void MathSqrt()
		//{
		//    Math.Sqrt(val);
		//}

		//[Benchmark]
		//public void MySqrt()
		//{
		//    for (int i = 0; i < 10; i++)
		//    {
		//        TestCls.sqrt_acc_v1(val);
		//    }
		//}

		//[Benchmark]
		//public void MySqrt_v2()
		//{
		//    for (int i = 0; i < 10; i++)
		//    {
		//        TestCls.sqrt_acc_v2(val);
		//    }
		//}

		//[Benchmark]
		//public void MySqrt_v3()
		//{
		//    for (int i = 0; i < 10; i++)
		//    {
		//        TestCls.sqrt_acc_v3(val);
		//    }
		//}

		//[Benchmark]
		//public void MathSqrt()
		//{
		//    Math.Sqrt(val);
		//}

		//[Benchmark]
		//public void MySqrt()
		//{
		//    TestCls.Sqrt(val);
		//}

		//[Benchmark]
		//public void sqrt_simplif_v5()
		//{
		//    TestCls.sqrt_simplif_v5(val);
		//}

		//[Benchmark]
		//public void sqrt_taylor_v2()
		//{
		//    TestCls.sqrt_taylor_v2(val);
		//}

		//[Benchmark]
		//public void sqrt_lookup_CPU()
		//{
		//    TestCls.sqrt_lookup_CPU(val);
		//}

		//[Benchmark]
		//public void sqrt_lookup_Inlined()
		//{
		//    TestCls.sqrt_lookup_Inlined(val);
		//}

		//public static GPU gpu = new GPU();

		//[Params(12,120, 1200,12000,120000,1200000)]
		//public int length;
		//public static Vector vector;


		//[Benchmark]
		//public void ToStr()
		//{
		//    vector = Vector.Arange(gpu, -12, length, 1, 1, false);
		//    vector.ToString();
		//}

		//[Benchmark]
		//public void ToStrNewer()
		//{
		//    vector = Vector.Arange(gpu, -12, length, 1, 1, false);
		//    vector.ToStringNEWER();
		//}

		//[Benchmark]
		//public void ToStrNewer2()
		//{
		//    vector = Vector.Arange(gpu, -12, length, 1, 1, false);
		//    vector.ToStringNEWER2();
		//}

		//[Benchmark]
		//public void ToStrNEWEST()
		//{
		//    vector = Vector.Arange(gpu, -12, length, 1, 1, false);
		//    vector.ToStringNEWEST();
		//}



	}



}
