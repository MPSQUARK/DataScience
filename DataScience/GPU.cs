﻿using ILGPU;
using ILGPU.Algorithms;
using ILGPU.IR.Transformations;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataScience
{
    public class GPU
    {
        Context context;
        public Accelerator accelerator;

        public Hashtable Data = new Hashtable(); // GPU-side memory caching
        private Queue<int> LRU = new Queue<int>(); // GPU-side memory caching
        private Random rnd = new Random();

        // Variables - Kernels
        #region
        public Action<AcceleratorStream, Index1, ArrayView<double>, ArrayView<float>> sumKernel;

        public Action<AcceleratorStream, Index1, ArrayView<float>, ArrayView<float>, ArrayView<float>, int, int> appendKernel;
        public Action<AcceleratorStream, Index1, ArrayView<float>, float> nanToNumKernel;
        public Action<AcceleratorStream, Index1, ArrayView<float>, ArrayView<float>, ArrayView<int>> accessSliceKernel;
        public Action<AcceleratorStream, Index1, ArrayView<float>, ArrayView<float>, ArrayView<float>, SpecializedValue<int>> consecutiveOperationKernel;
        public Action<AcceleratorStream, Index1, ArrayView<float>, ArrayView<float>, float, SpecializedValue<int>> scalarConsecutiveOperationKernel;
        public Action<AcceleratorStream, Index1, ArrayView<float>, ArrayView<float>> diffKernel;
        public Action<AcceleratorStream, Index1, ArrayView<float>, ArrayView<float>> reverseKernel;
        public Action<AcceleratorStream, Index1, ArrayView<float>> absKernel;
        public Action<AcceleratorStream, Index1, ArrayView<float>> reciprocalKernel;
        public Action<AcceleratorStream, Index1, ArrayView<float>, ArrayView<float>, ArrayView<float>> crossKernel;
        public Action<AcceleratorStream, Index1, ArrayView<float>, ArrayView<float>, int> transposekernel;

        #endregion


        // Constructor
        public GPU(bool forceCPU = false, ContextFlags flags = ContextFlags.None, OptimizationLevel optimizationLevel = OptimizationLevel.Debug)
        {
            this.context = new Context(flags, optimizationLevel);
            this.context.EnableAlgorithms();

            this.accelerator = GetGpu(context, forceCPU);
            Console.WriteLine("Device loaded: " + accelerator.Name);

            LoadKernels();
            Console.WriteLine("Device Kernels Loaded");
        }
        private void LoadKernels()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            sumKernel = accelerator.LoadAutoGroupedKernel<Index1, ArrayView<double>, ArrayView<float>>(SumKernel);

            appendKernel = accelerator.LoadAutoGroupedKernel<Index1, ArrayView<float>, ArrayView<float>, ArrayView<float>, int, int>(AppendKernel);
            nanToNumKernel = accelerator.LoadAutoGroupedKernel<Index1, ArrayView<float>, float>(Nan_to_numKernel);
            accessSliceKernel = accelerator.LoadAutoGroupedKernel<Index1, ArrayView<float>, ArrayView<float>, ArrayView<int>>(AccessSliceKernel);
            consecutiveOperationKernel = accelerator.LoadAutoGroupedKernel<Index1, ArrayView<float>, ArrayView<float>, ArrayView<float>, SpecializedValue<int>>(ConsecutiveOperationKernel);
            scalarConsecutiveOperationKernel = accelerator.LoadAutoGroupedKernel<Index1, ArrayView<float>, ArrayView<float>, float, SpecializedValue<int>>(ScalarConsecutiveOperationKernel);
            diffKernel = accelerator.LoadAutoGroupedKernel<Index1, ArrayView<float>, ArrayView<float>> (DiffKernel);
            reverseKernel = accelerator.LoadAutoGroupedKernel<Index1, ArrayView<float>, ArrayView<float>> (ReverseKernel);
            absKernel = accelerator.LoadAutoGroupedKernel<Index1, ArrayView<float>>(AbsKernel);
            reciprocalKernel = accelerator.LoadAutoGroupedKernel<Index1, ArrayView<float>>(ReciprocalKernel);
            crossKernel = accelerator.LoadAutoGroupedKernel<Index1, ArrayView<float>, ArrayView<float>, ArrayView<float>>(CrossKernel);
            transposekernel = accelerator.LoadAutoGroupedKernel<Index1, ArrayView<float>, ArrayView<float>, int>(TransposeKernel);

            timer.Stop();
            Console.WriteLine("Kernels Loaded in: " + timer.Elapsed.TotalMilliseconds + " MS");
        }


        // Get 'Best' GPU
        private Accelerator GetGpu(Context context, bool prefCPU = false)
        {
            var groupedAccelerators = Accelerator.Accelerators
                    .GroupBy(x => x.AcceleratorType)
                    .ToDictionary(x => x.Key, x => x.ToList());

            if (prefCPU)
            {
                return new CPUAccelerator(context);
            }

            if (groupedAccelerators.TryGetValue(AcceleratorType.Cuda, out var nv))
            {
                return Accelerator.Create(context, nv[0]);
            }

            if (groupedAccelerators.TryGetValue(AcceleratorType.OpenCL, out var cl))
            {
                return Accelerator.Create(context, cl[0]);
            }

            //fallback
            Console.WriteLine("Warning : Could not find gpu, falling back to Default device");
            return new CPUAccelerator(context);
        }


        // Memory Caching & Management
        private int GenerateId()
        {
            return rnd.Next(-100000, 100000);
        }
        private bool TestId(int id)
        {
            return this.Data.ContainsKey(id);
        }
        public int Cache(float[] array)
        {
            int Id = GenerateId();
            while (TestId(Id))
            {
                Id = GenerateId();
            }

            // Try Allocate - Need to add remove least recently used if not enough space
            MemoryBuffer<float> buffer = this.accelerator.Allocate<float>(array.Length);
            buffer.CopyFrom(array, 0, 0, array.Length);

            LRU.Enqueue(Id);
            Data[Id] = buffer;
            return Id;
        }
        public void DeCache(int Id)
        {
            MemoryBuffer<float> buffer = (MemoryBuffer<float>)Data[Id];
            buffer.Dispose();
            Data.Remove(Id);


        }
        private void DeCacheLast()
        {

        }




        // Test Kernels
        static void SumKernel(Index1 index, ArrayView<double> Output, ArrayView<float> Input)
        {
            double sum = 0;
            for (int i = index * 100000; i < (index + 1) * 100000; i++)
            {
                sum += Input[i];
            }
            Output[index] += sum;
        }




        //Kernels
        static void AppendKernel(Index1 index, ArrayView<float> Output, ArrayView<float> vecA, ArrayView<float> vecB, int vecAcol, int vecBcol)
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



        static void Nan_to_numKernel(Index1 index, ArrayView<float> IO, float num)
        {
            if (float.IsNaN(IO[index]) || float.IsInfinity(IO[index]))
            {
                IO[index] = num;
            }

        }

        //static void AccessSliceKernel(Index1 index, ArrayView<float> OutPut, ArrayView<float> Input, ArrayView<int> ChangeSelectLength)
        //{
        //    OutPut[index] = Input[
        //        index * ChangeSelectLength[0] * ChangeSelectLength[4] + // iRcL
        //        index * ChangeSelectLength[1] +                         // iCc
        //        ChangeSelectLength[2] * ChangeSelectLength[4] +         // RsL
        //        ChangeSelectLength[3]];                                 // Cs
        //}

        static void AccessSliceKernel(Index1 index, ArrayView<float> OutPut, ArrayView<float> Input, ArrayView<int> ChangeSelectLength)
        {
            OutPut[index] = Input[
                index * ChangeSelectLength[1] +                         // iRcL
                ChangeSelectLength[0]];                                 // Cs
        }

        static void ConsecutiveOperationKernel(Index1 index, ArrayView<float> InputA, ArrayView<float> InputB, ArrayView<float> OutPut, SpecializedValue<int> operation)
        {
            switch ((Operations)operation.Value)
            {
                case Operations.multiplication:
                    OutPut[index] = InputA[index] * InputB[index];
                    break;
                case Operations.addition:
                    OutPut[index] = InputA[index] + InputB[index];
                    break;
                case Operations.subtraction:
                    OutPut[index] = InputA[index] - InputB[index];
                    break;
                case Operations.flipSubtraction:
                    OutPut[index] = InputB[index] - InputA[index];
                    break;
                case Operations.division:
                    OutPut[index] = InputA[index] / InputB[index];
                    break;
                case Operations.inverseDivision:
                    OutPut[index] = InputB[index] / InputA[index];
                    break;
                case Operations.power:
                    OutPut[index] = XMath.Pow(InputA[index], InputB[index]);
                    break;
                case Operations.powerFlipped:
                    OutPut[index] = XMath.Pow(InputB[index], InputA[index]);
                    break;
                case Operations.squareOfDiffs:
                    OutPut[index] = XMath.Pow((InputA[index] - InputB[index]), 2f);
                    break;

            }
        }

        static void ScalarConsecutiveOperationKernel(Index1 index, ArrayView<float> OutPut, ArrayView<float> Input, float Scalar, SpecializedValue<int> operation)
        {
            switch ((Operations)operation.Value)
            {
                case Operations.multiplication:
                    OutPut[index] = Input[index] * Scalar;
                    break;
                case Operations.addition:
                    OutPut[index] = Input[index] + Scalar;
                    break;
                case Operations.subtraction:
                    OutPut[index] = Input[index] - Scalar;
                    break;
                case Operations.flipSubtraction:
                    OutPut[index] = Scalar - Input[index];
                    break;
                case Operations.division:
                    OutPut[index] = Input[index] / Scalar;
                    break;
                case Operations.inverseDivision:
                    OutPut[index] = Scalar / Input[index];
                    break;
                case Operations.power:
                    OutPut[index] = XMath.Pow(Input[index], Scalar);
                    break;
                case Operations.powerFlipped:
                    OutPut[index] = XMath.Pow(Scalar, Input[index]);
                    break;
                case Operations.squareOfDiffs:
                    OutPut[index] = XMath.Pow((Input[index] - Scalar), 2f);
                    break;
            }
        }

        static void DiffKernel(Index1 index, ArrayView<float> Output, ArrayView<float> Input)
        {
            Output[index] = Input[index + 1] - Input[index];
        }

        static void ReverseKernel(Index1 index, ArrayView<float> Output, ArrayView<float> Input)
        {
            Output[index] = Input[Input.Length - 1 - index];
        }

        static void AbsKernel(Index1 index, ArrayView<float> IO)
        {
            IO[index] = XMath.Abs(IO[index]);
        }

        static void ReciprocalKernel(Index1 index, ArrayView<float> IO)
        {
            IO[index] = XMath.Rcp(IO[index]);
        }

        static void CrossKernel(Index1 index, ArrayView<float> Output, ArrayView<float> InputA, ArrayView<float> InputB)
        {
            Output[index*3]     = InputA[index * 3 + 1] * InputB[index * 3 + 2] - InputA[index * 3 + 2] * InputB[index * 3 + 1];
            Output[index*3 + 1] = InputA[index * 3 + 2] * InputB[index * 3    ] - InputA[index * 3    ] * InputB[index * 3 + 2];
            Output[index*3 + 2] = InputA[index * 3    ] * InputB[index * 3 + 1] - InputA[index * 3 + 1] * InputB[index * 3    ];
        }

        static void TransposeKernel(Index1 index, ArrayView<float> Output, ArrayView<float> Input, int columns)
        {
            // (int)Math.Floor(Input.Length / columns) => The Row
            // (int)(Input.Length % columns) => The Column
            float invcol = 1f / columns;
            Output[(index % columns) * ((int)(Input.Length * invcol)) + ((int)XMath.Floor(index * invcol))] = Input[index];
        }












    }

    public enum Operations
    {
        multiplication = 0,
        addition = 1,
        subtraction = 2,
        division = 3,
        power = 4,
        inverseDivision = 5,
        flipSubtraction = 6,
        powerFlipped = 7,
        squareOfDiffs = 8,
    }
}
