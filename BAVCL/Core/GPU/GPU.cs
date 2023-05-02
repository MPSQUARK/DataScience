using ILGPU;
using ILGPU.Algorithms;
using ILGPU.Runtime;
using System;
using System.Diagnostics;
using BAVCL.Experimental;
using System.Collections.Generic;
using BAVCL.Core.Interfaces;
using BAVCL.Core;

namespace BAVCL
{
	public class GPU
	{
		protected internal Context context;
		public Accelerator accelerator;

		private IMemoryManager memoryManager;
		
		// Accelerator Preference Order
		Dictionary<AcceleratorType, int> AcceleratorPrefOrder = new()
		{
			{ AcceleratorType.Cuda, 2 },
			{ AcceleratorType.OpenCL, 1 },
			{ AcceleratorType.CPU, 0 }
		};

		// Variables - Kernels
		#region
		// Test KERNELS
		public Action<AcceleratorStream, Index1D, ArrayView<double>, ArrayView<float>> sumKernel;
		public Action<AcceleratorStream, Index1D, ArrayView<float>, ArrayView<float>> TestSQRTKernel;
		public Action<AcceleratorStream, Index1D, ArrayView<float>, ArrayView<float>> TestMYSQRTKernel;


		// ACTUAL KERNELS
		public Action<AcceleratorStream, Index1D, ArrayView<float>, ArrayView<float>, ArrayView<float>, int, int> appendKernel;
		public Action<AcceleratorStream, Index1D, ArrayView<float>, float> nanToNumKernel;
		public Action<AcceleratorStream, Index1D, ArrayView<float>, ArrayView<float>, ArrayView<int>> getSliceKernel;
		public Action<AcceleratorStream, Index1D, ArrayView<float>, ArrayView<float>, ArrayView<float>, SpecializedValue<int>> a_opFKernel;
		public Action<AcceleratorStream, Index1D, ArrayView<float>, ArrayView<float>, float, SpecializedValue<int>> s_opFKernel;
		public Action<AcceleratorStream, Index1D, ArrayView<float>, ArrayView<float>, ArrayView<float>, int, SpecializedValue<int>> vectormatrixOpKernel;

		public Action<AcceleratorStream, Index1D, ArrayView<float>, ArrayView<float>, SpecializedValue<int>> a_FloatOPKernelIP;
		public Action<AcceleratorStream, Index1D, ArrayView<float>, float, SpecializedValue<int>> s_FloatOPKernelIP;
		public Action<AcceleratorStream, Index1D, ArrayView<float>, ArrayView<float>> diffKernel;
		public Action<AcceleratorStream, Index1D, ArrayView<float>> reverseKernel;
		public Action<AcceleratorStream, Index1D, ArrayView<float>> absKernel;
		public Action<AcceleratorStream, Index1D, ArrayView<float>> rcpKernel;
		public Action<AcceleratorStream, Index1D, ArrayView<float>> rsqrtKernel;
		public Action<AcceleratorStream, Index1D, ArrayView<float>, ArrayView<float>, ArrayView<float>> crossKernel;
		public Action<AcceleratorStream, Index1D, ArrayView<float>, ArrayView<float>, int> transposekernel;

		public Action<AcceleratorStream, Index1D, ArrayView<float>, ArrayView<float>, ArrayView<float>> pf32_add_kernel;

		public Action<AcceleratorStream, Index1D, ArrayView<float>, float> LogKernel;
		
		public Action<AcceleratorStream, Index1D, ArrayView<float>, ArrayView<float>> TestKernel;
		
		#endregion

		
		// Constructor
		public GPU(float memorycap = 0.8f, bool forceCPU = false)
		{
			// Create Context
			context = Context.Create(builder => builder.Default().EnableAlgorithms());
			// OptimizationLevel optimizationLevel = OptimizationLevel.Debug

			// Get Accelerator Device
			//this.accelerator = context.GetPreferredDevice(preferCPU: forceCPU).CreateAccelerator(context);
			accelerator = GetPreferedAccelerator(context, forceCPU);
			Console.WriteLine($"Device loaded: {accelerator.Name}");

			// Set Memory Usage Cap
			memoryManager = new LRU(accelerator.MemorySize, memorycap);

			// Load Kernels
			LoadKernels();
		}

		// Wrappers for Memory Manager
		public (uint, MemoryBuffer) Allocate<T>(ICacheable<T> cacheable) where T : unmanaged => memoryManager.Allocate(cacheable, accelerator);
		public (uint, MemoryBuffer) Allocate<T>(ICacheable cacheable, T[] values) where T : unmanaged => memoryManager.Allocate(cacheable, values, accelerator);
		public (uint, MemoryBuffer) AllocateEmpty<T>(ICacheable cacheable, int length) where T : unmanaged => memoryManager.AllocateEmpty<T>(cacheable, length, accelerator);
		public MemoryBuffer TryGetBuffer<T>(uint Id) where T : unmanaged => memoryManager.GetBuffer(Id);
		public (uint, MemoryBuffer) UpdateBuffer<T>(ICacheable cacheable, T[] values) where T : unmanaged => memoryManager.UpdateBuffer(cacheable, values, accelerator);
		public (uint, MemoryBuffer) UpdateBuffer<T>(ICacheable<T> cacheable) where T : unmanaged => memoryManager.UpdateBuffer(cacheable, accelerator);
		public uint GCItem(uint Id) => memoryManager.GCItem(Id);
		public string PrintMemoryUsage(bool percentage, string format = "F2") => memoryManager.PrintMemoryUsage(percentage, format);
		public string GetMemUsage() => memoryManager.MemoryUsed.ToString();

		private Accelerator GetPreferedAccelerator(Context context, bool forceCPU)
		{
			var devices = context.Devices;

			if (devices.Length == 0) throw new Exception("No Accelerators");

			Device preferedAccelerator = null;
			for (int i = 0; i < devices.Length; i++)
			{
				if (forceCPU && devices[i].AcceleratorType == AcceleratorType.CPU)
					return devices[i].CreateAccelerator(context);
				
				if (preferedAccelerator == null)
					preferedAccelerator = devices[i];

				if (AcceleratorPrefOrder.TryGetValue(preferedAccelerator.AcceleratorType, out int Prefpriority))
					if (AcceleratorPrefOrder.TryGetValue(devices[i].AcceleratorType, out int Devicepriority))
					{
						if (Devicepriority > Prefpriority)
						{
							preferedAccelerator = devices[i];
							continue;
						}

						if (devices[i].MaxConstantMemory > preferedAccelerator.MaxConstantMemory)
						{
							preferedAccelerator = devices[i];
							continue;
						}
					}
			}
			return preferedAccelerator.CreateAccelerator(context);
		}

		private void LoadKernels()
		{
			Stopwatch timer = new();
			timer.Start();

			// TEST KERNELS
			sumKernel = accelerator.LoadAutoGroupedKernel<Index1D, ArrayView<double>, ArrayView<float>>(SumKernel);
			TestSQRTKernel = accelerator.LoadAutoGroupedKernel<Index1D, ArrayView<float>, ArrayView<float>>(Testsqrtkernel);
			TestMYSQRTKernel = accelerator.LoadAutoGroupedKernel<Index1D, ArrayView<float>, ArrayView<float>>(Testmysqrtkernel);


			appendKernel = accelerator.LoadAutoGroupedKernel<Index1D, ArrayView<float>, ArrayView<float>, ArrayView<float>, int, int>(AppendKernel);
			nanToNumKernel = accelerator.LoadAutoGroupedKernel<Index1D, ArrayView<float>, float>(Nan_to_numKernel);
			getSliceKernel = accelerator.LoadAutoGroupedKernel<Index1D, ArrayView<float>, ArrayView<float>, ArrayView<int>>(AccessSliceKernel);
			
			a_opFKernel = accelerator.LoadAutoGroupedKernel<Index1D, ArrayView<float>, ArrayView<float>, ArrayView<float>, SpecializedValue<int>>(A_FloatOPKernel);
			s_opFKernel = accelerator.LoadAutoGroupedKernel<Index1D, ArrayView<float>, ArrayView<float>, float, SpecializedValue<int>>(S_FloatOPKernel);
			vectormatrixOpKernel = accelerator.LoadAutoGroupedKernel<Index1D, ArrayView<float>, ArrayView<float>, ArrayView<float>, int, SpecializedValue<int>>(VectorMatrixKernel);

			a_FloatOPKernelIP = accelerator.LoadAutoGroupedKernel<Index1D, ArrayView<float>, ArrayView<float>, SpecializedValue<int>>(A_FloatOPKernelIP);
			s_FloatOPKernelIP = accelerator.LoadAutoGroupedKernel<Index1D, ArrayView<float>, float, SpecializedValue<int>>(S_FloatOPKernelIP);

			diffKernel = accelerator.LoadAutoGroupedKernel<Index1D, ArrayView<float>, ArrayView<float>> (DiffKernel);
			reverseKernel = accelerator.LoadAutoGroupedKernel<Index1D, ArrayView<float>> (ReverseKernel);
			absKernel = accelerator.LoadAutoGroupedKernel<Index1D, ArrayView<float>>(AbsKernel);
			rcpKernel = accelerator.LoadAutoGroupedKernel<Index1D, ArrayView<float>>(ReciprocalKernel);
			rsqrtKernel = accelerator.LoadAutoGroupedKernel<Index1D, ArrayView<float>>(RsqrtKernel);

			crossKernel = accelerator.LoadAutoGroupedKernel<Index1D, ArrayView<float>, ArrayView<float>, ArrayView<float>>(CrossKernel);
			transposekernel = accelerator.LoadAutoGroupedKernel<Index1D, ArrayView<float>, ArrayView<float>, int>(TransposeKernel);

			LogKernel = accelerator.LoadAutoGroupedKernel<Index1D, ArrayView<float>, float>(LogKern);
			

			timer.Stop();
			Console.WriteLine($"Kernels Loaded in: {timer.Elapsed.TotalMilliseconds} MS");
		}

		// Test Kernels
		static void SumKernel(Index1D index, ArrayView<double> Output, ArrayView<float> Input)
		{
			double sum = 0;
			for (int i = index * 100000; i < (index + 1) * 100000; i++)
			{
				sum += Input[i];
			}
			Output[index] += sum;
		}

		#region "Kernels"
		static void AppendKernel(Index1D index, ArrayView<float> Output, ArrayView<float> vecA, ArrayView<float> vecB, int vecAcol, int vecBcol)
		{

			for (int i = 0, j=0; j < vecBcol; i++)
			{
				if (i < vecAcol)
				{
					Output[index * (vecAcol + vecBcol) + i] = vecA[index * vecAcol + i];
					continue;
				}

				Output[index * (vecAcol + vecBcol) + i] = vecB[index * vecBcol + j];
				j++;

			}

		}

		static void Nan_to_numKernel(Index1D index, ArrayView<float> IO, float num)
		{
			if (float.IsNaN(IO[index]) || float.IsInfinity(IO[index]))
			{
				IO[index] = num;
			}

		}

		static void AccessSliceKernel(Index1D index, ArrayView<float> OutPut, ArrayView<float> Input, ArrayView<int> ChangeSelectLength)
		{
			OutPut[index] = Input[
				index * ChangeSelectLength[1] +                         // iRcL
				ChangeSelectLength[0]];                                 // Cs
		}


		static void A_FloatOPKernel(Index1D index, ArrayView<float> OutPut, ArrayView<float> InputA, ArrayView<float> InputB, SpecializedValue<int> operation)
		{
			switch ((Operations)operation.Value)
			{
				case Operations.multiply:
					OutPut[index] = InputA[index] * InputB[index];
					break;
				case Operations.add:
					OutPut[index] = InputA[index] + InputB[index];
					break;
				case Operations.subtract:
					OutPut[index] = InputA[index] - InputB[index];
					break;
				case Operations.flipSubtract:
					OutPut[index] = InputB[index] - InputA[index];
					break;
				case Operations.divide:
					OutPut[index] = InputA[index] / InputB[index];
					break;
				case Operations.flipDivide:
					OutPut[index] = InputB[index] / InputA[index];
					break;
				case Operations.pow:
					OutPut[index] = XMath.Pow(InputA[index], InputB[index]);
					break;
				case Operations.flipPow:
					OutPut[index] = XMath.Pow(InputB[index], InputA[index]);
					break;
				case Operations.subtractSquare:
					OutPut[index] = (InputA[index]*InputA[index]) - (InputB[index]*InputB[index]);
					break;

			}
		}

		static void A_FloatOPKernelIP(Index1D index, ArrayView<float> IO, ArrayView<float> Input, SpecializedValue<int> operation)
		{
			switch ((Operations)operation.Value)
			{
				case Operations.multiply:
					IO[index] = IO[index] * Input[index];
					break;
				case Operations.add:
					IO[index] = IO[index] + Input[index];
					break;
				case Operations.subtract:
					IO[index] = IO[index] - Input[index];
					break;
				case Operations.flipSubtract:
					IO[index] = IO[index] - Input[index];
					break;
				case Operations.divide:
					IO[index] = IO[index] / Input[index];
					break;
				case Operations.flipDivide:
					IO[index] = IO[index] / Input[index];
					break;
				case Operations.pow:
					IO[index] = XMath.Pow(IO[index], Input[index]);
					break;
				case Operations.flipPow:
					IO[index] = XMath.Pow(IO[index], Input[index]);
					break;
				case Operations.subtractSquare:
					IO[index] = (IO[index] * IO[index]) - (Input[index] * Input[index]);
					break;
			}
		}

		static void S_FloatOPKernel(Index1D index, ArrayView<float> OutPut, ArrayView<float> Input, float Scalar, SpecializedValue<int> operation)
		{
			switch ((Operations)operation.Value)
			{
				case Operations.multiply:
					OutPut[index] = Input[index] * Scalar;
					break;
				case Operations.add:
					OutPut[index] = Input[index] + Scalar;
					break;
				case Operations.subtract:
					OutPut[index] = Input[index] - Scalar;
					break;
				case Operations.flipSubtract:
					OutPut[index] = Scalar - Input[index];
					break;
				case Operations.divide:
					OutPut[index] = Input[index] / Scalar;
					break;
				case Operations.flipDivide:
					OutPut[index] = Scalar / Input[index];
					break;
				case Operations.pow:
					OutPut[index] = XMath.Pow(Input[index], Scalar);
					break;
				case Operations.flipPow:
					OutPut[index] = XMath.Pow(Scalar, Input[index]);
					break;
				case Operations.subtractSquare:
					OutPut[index] = (Input[index] * Input[index]) - (Scalar * Scalar);
					break;
			}
		}
		static void S_FloatOPKernelIP(Index1D index, ArrayView<float> IO, float Scalar, SpecializedValue<int> operation)
		{
			switch ((Operations)operation.Value)
			{
				case Operations.multiply:
					IO[index] = IO[index] * Scalar;
					break;
				case Operations.add:
					IO[index] = IO[index] + Scalar;
					break;
				case Operations.subtract:
					IO[index] = IO[index] - Scalar;
					break;
				case Operations.flipSubtract:
					IO[index] = Scalar - IO[index];
					break;
				case Operations.divide:
					IO[index] = IO[index] / Scalar;
					break;
				case Operations.flipDivide:
					IO[index] = Scalar / IO[index];
					break;
				case Operations.pow:
					IO[index] = XMath.Pow(IO[index], Scalar);
					break;
				case Operations.flipPow:
					IO[index] = XMath.Pow(Scalar, IO[index]);
					break;
				case Operations.subtractSquare:
					IO[index] = (IO[index] * IO[index]) - (Scalar * Scalar);
					break;
			}
		}

		static void VectorMatrixKernel(Index1D index, ArrayView<float> OutPut, ArrayView<float> InputA, ArrayView<float> InputB, int Cols, SpecializedValue<int> operation)
		{
			int startidx = index * Cols;

			switch ((Operations)operation.Value)
			{
				case Operations.multiply:
					for (int i = 0; i < Cols; i++)
					{
						OutPut[index] += InputA[i] * InputB[startidx + i];
					}
					break;
				case Operations.add:
					for (int i = 0; i < Cols; i++)
					{
						OutPut[index] += InputA[i] + InputB[startidx + i];
					}
					break;
				case Operations.subtract:
					for (int i = 0; i < Cols; i++)
					{
						OutPut[index] += InputA[i] - InputB[startidx + i];
					}
					break;
				case Operations.flipSubtract:
					for (int i = 0; i < Cols; i++)
					{
						OutPut[index] += InputB[startidx + i] - InputA[i];
					}
					break;
				case Operations.divide:
					for (int i = 0; i < Cols; i++)
					{
						OutPut[index] += InputA[i] / InputB[startidx + i];
					}
					break;
				case Operations.flipDivide:
					for (int i = 0; i < Cols; i++)
					{
						OutPut[index] += InputB[startidx + i] / InputA[i];
					}
					break;
				case Operations.pow:
					for (int i = 0; i < Cols; i++)
					{
						OutPut[index] += XMath.Pow(InputA[i], InputB[startidx + i]);
					}
					break;
				case Operations.flipPow:
					for (int i = 0; i < Cols; i++)
					{
						OutPut[index] += XMath.Pow(InputB[startidx + i], InputA[i]);
					}
					break;
				case Operations.subtractSquare:
					for (int i = 0; i < Cols; i++)
					{
						OutPut[index] += ((InputA[i] * InputA[i]) - (InputB[startidx + i] * InputB[startidx + i]));
					}
					break;

			}
		}


		static void DiffKernel(Index1D index, ArrayView<float> Output, ArrayView<float> Input)
		{
			Output[index] = Input[index + 1] - Input[index];
		}

		static void ReverseKernel(Index1D index, ArrayView<float> IO)
		{
			int idx = IO.IntLength - 1 - index;
			(IO[index], IO[idx]) = (IO[idx], IO[index]);
		}

		static void AbsKernel(Index1D index, ArrayView<float> IO)
		{
			IO[index] = XMath.Abs(IO[index]);
		}

		static void ReciprocalKernel(Index1D index, ArrayView<float> IO)
		{
			IO[index] = XMath.Rcp(IO[index]);
		}

		static void RsqrtKernel(Index1D index, ArrayView<float> IO)
		{
			IO[index] = XMath.Rsqrt(IO[index]);
		}



		static void CrossKernel(Index1D index, ArrayView<float> Output, ArrayView<float> InputA, ArrayView<float> InputB)
		{
			Output[index*3]     = InputA[index * 3 + 1] * InputB[index * 3 + 2] - InputA[index * 3 + 2] * InputB[index * 3 + 1];
			Output[index*3 + 1] = InputA[index * 3 + 2] * InputB[index * 3    ] - InputA[index * 3    ] * InputB[index * 3 + 2];
			Output[index*3 + 2] = InputA[index * 3    ] * InputB[index * 3 + 1] - InputA[index * 3 + 1] * InputB[index * 3    ];
		}

		static void TransposeKernel(Index1D index, ArrayView<float> Output, ArrayView<float> Input, int columns)
		{
			int rows = Input.IntLength / columns;
			int col = index % columns;
			int row = (int)XMath.Floor(index / columns);

			int idx = col * rows + row;

			Output[idx] = Input[index];
		}


		static void Testsqrtkernel(Index1D index, ArrayView<float> Output, ArrayView<float> Input)
		{
			Output[index] = XMath.Sqrt(Input[index]);
		}

		static void Testmysqrtkernel(Index1D index, ArrayView<float> Output, ArrayView<float> Input)
		{
			Output[index] = TestCls.sqrt_acc_v2(Input[index]);
		}


		public static void LogKern(Index1D index, ArrayView<float> IO, float @base)
		{
			IO[index] = XMath.Log(IO[index],@base);
		}

		#endregion

	}

}
