﻿using System;
using DataScience;
using ILGPU;
using ILGPU.Runtime;
using System.Linq;

using System.Diagnostics;

namespace Testing_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Context context = new Context();
            Accelerator gpu;

            gpu = Setup.GetGPU(context);

            // SAMPLE AND TEST CODE


            Vector vectorA = Vector.Linspace(-1000000, -1, 1000000);
            //Vector vectorB = Vector.Arange(-10, 10, 5);

            //vectorA.Print();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            Vector vectorA_abs = Vector.Abs(vectorA);
            Console.WriteLine($"{sw.ElapsedMilliseconds} ms");
            sw.Stop();

            sw.Reset();
            sw.Start();
            Vector vectorA_abs2 = Vector.AbsX(gpu, vectorA);
            Console.WriteLine($"{sw.ElapsedMilliseconds} ms");
            sw.Stop();

            sw.Reset();
            sw.Start();
            Vector vectorA_abs3 = Vector.AbsX2(gpu, vectorA);
            Console.WriteLine($"{sw.ElapsedMilliseconds} ms");
            sw.Stop();

            //vectorA_abs3.Print();

            Console.WriteLine();
            Console.WriteLine($"Minimum in Abs : {vectorA_abs.Value.Min()} \nMinimum in Abs2 : {vectorA_abs2.Value.Min()} \nMinimum in Abs3 : {vectorA_abs2.Value.Min() }");

        }
    }
}
