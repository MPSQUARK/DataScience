using ILGPU.Runtime;
using ILGPU.Algorithms;
using System;
using BAVCL.Core;
using BAVCL.Ext;
using ILGPU;

namespace BAVCL
{

	/// <summary>
	/// Class for 1D and 2D Vector support
	/// Float Precision
	/// </summary>
	public sealed partial class Vector : VectorBase<float>
	{

		// CONSTRUCTOR
		/// <summary>
		/// Constructs a Vector class object.
		/// </summary>
		/// <param name="gpu">The device to use when computing this Vector.</param>
		/// <param name="values">The array of data contained in this Vector.</param>
		/// <param name="columns">The number of Columns IF this is a 2D Vector, for 1D Vectors use the default Columns = 1</param>
		public Vector(GPU gpu, float[] values, int columns = 1, bool cache = true) :
			base(gpu, values, columns, cache) { }

		public Vector(GPU gpu, int length, int columns = 1) :
			base(gpu, length, columns) { }

		public float this[int i]
		{
			get => Value[i];
			set => Value[i] = value;
		}

		public override void Print() => Console.WriteLine(ToStr());

		public void Print(byte decimalplaces = 2, bool syncCPU = true) => Console.WriteLine(ToStr(decimalplaces, syncCPU));

		// METHODS
		public bool Equals(Vector vector)
		{
			SyncCPU();
			vector.SyncCPU();

			if (Length != vector.Length) return false;

			for (int i = 0; i < Length; i++)
				if (this.Value[i] != vector.Value[i]) return false;

			return true;
		}

		public Vector Copy(bool Cache = true)
		{
			if (ID == 0)
				return new Vector(gpu, Value[..], Columns, Cache);

			return new Vector(gpu, Pull(), Columns, Cache);
		}

		#region "MATHEMATICAL PROPERTIES"
		public override float Mean() => Sum() / Length;

		public float Std() => XMath.Sqrt(Var());

		public float Var()
		{
			SyncCPU();

			if (Length < 10000)
			{
				int
					vectorSize = System.Numerics.Vector<float>.Count,
					i = 0;

				float[] array = this.Value;

				float mean = Mean();

				System.Numerics.Vector<float> meanvec = new(mean);

				System.Numerics.Vector<float> sumVector = System.Numerics.Vector<float>.Zero;

				for (; i <= array.Length - vectorSize; i += vectorSize)
				{
					System.Numerics.Vector<float> input = new(array, i);
					System.Numerics.Vector<float> difference = input - meanvec;

					sumVector += (difference * difference);
				}

				float sum = 0;

				for (int j = 0; j < vectorSize; j++)
					sum += sumVector[j];

				for (; i < array.Length; i++)
					sum += XMath.Pow((array[i] - mean), 2f);

				return sum / Length;
			}

			//Vector diff = Vector.AbsX(this - this.Mean());

			//return (diff * diff).Sum() / this.Length();
			return OP(this, Mean(), Operations.subtractSquare).Sum() / Length;
		}
		public override float Range() => Max() - Min();
		public void Flatten() => this.Columns = 1;

		public override float Min()
		{
			SyncCPU();
			return Value.Min();
		}

		public override float Max()
		{
			SyncCPU();
			return Value.Max();
		}

		#endregion


		#region "CONVERSION"

		public Geometric.Vector3 ToVector3()
		{
			if (Length % 3 != 0) { throw new Exception("Vector length must be a multiple of 3"); }
			if (ID != 0)
				return new Geometric.Vector3(gpu, Pull());

			return new Geometric.Vector3(gpu, Value);
		}

		#endregion


		#region "OPERATORS"
		public static Vector operator +(Vector vector) => 
			AbsX(vector);
		public static Vector operator +(Vector vectorA, Vector vectorB) => 
			OP(vectorA, vectorB, Operations.add);
		public static Vector operator +(Vector vector, float Scalar) => 
			OP(vector, Scalar, Operations.add);
		public static Vector operator +(float Scalar, Vector vector) => 
			OP(vector, Scalar, Operations.add);

		public static Vector operator -(Vector vector) => 
			OP(vector, -1, Operations.multiply);
		public static Vector operator -(Vector vectorA, Vector vectorB) => 
			OP(vectorA, vectorB, Operations.subtract);
		public static Vector operator -(Vector vector, float scalar) => 
			OP(vector, scalar, Operations.subtract);
		public static Vector operator -(float scalar, Vector vector) => 
			OP(vector, scalar, Operations.flipSubtract);

		public static Vector operator *(Vector vectorA, Vector vectorB) => 
			OP(vectorA, vectorB, Operations.multiply);

		public static Vector operator *(Vector vector, float scalar) => 
			OP(vector, scalar, Operations.multiply);

		public static Vector operator *(float scalar, Vector vector) =>
			OP(vector, scalar, Operations.multiply);

		public static Vector operator /(Vector vectorA, Vector vectorB) => 
			OP(vectorA, vectorB, Operations.divide);
		public static Vector operator /(Vector vector, float scalar) => 
			OP(vector, scalar, Operations.divide);
		public static Vector operator /(float scalar, Vector vector) =>
			OP(vector, scalar, Operations.flipDivide);

		public static Vector operator ^(Vector vectorA, Vector vectorB) => 
			OP(vectorA, vectorB, Operations.pow);
		public static Vector operator ^(Vector vector, float scalar) => 
			OP(vector, scalar, Operations.pow);
		public static Vector operator ^(float Scalar, Vector vector) => 
			OP(vector, Scalar, Operations.flipPow);


		#endregion
		
		
		public static Vector test(Vector vectorA)
		{
			Vector output = new Vector(vectorA.gpu, vectorA.Length, vectorA.Columns);
			
			MemoryBuffer1D<float, Stride1D.Dense>
				buffer = output.GetBuffer(),        // Output
				buffer2 = vectorA.GetBuffer();       // Input
				
				
			vectorA.gpu.TestKernel(vectorA.gpu.accelerator.DefaultStream,buffer.IntExtent, buffer.View, buffer2.View);
			vectorA.gpu.accelerator.Synchronize();
			return output;
		}



		// FUNCTIONS
		public static Vector OP(Vector vectorA, Vector vectorB, Operations operation, bool Warp = false)
		{
			// Check function conditions
			if (vectorA.Length == vectorB.Length)
				return _VectorVectorOP(vectorA, vectorB, operation);


			bool ThisLonger = vectorA.Length > vectorB.Length;


			// If one input is a Vector and other is Matrix
			if ((vectorA.Columns == 1 && vectorB.Columns > 1) || (vectorA.Columns > 1 && vectorB.Columns == 1))
			{
				if (ThisLonger) return _VectorMatrixOP(vectorB, vectorA, operation);

				return _VectorMatrixOP(vectorA, vectorB, operation);
			}

			throw new IndexOutOfRangeException("Vector A and Vector B provided MUST be of EQUAL length");
		}

		public Vector IPOP(Vector vectorB, Operations operation)
		{
			// If the lengths are the same and both 1D vectors
			if (Length == vectorB.Length && vectorB.Columns == 1 && this.Columns == 1)
				return _VectorVectorOP_IP(vectorB, operation);


			bool ThisLonger = this.Value.Length > vectorB.Value.Length;

			// If one input is a Vector and other is Matrix
			if ((this.Columns == 1 && vectorB.Columns > 1) || (this.Columns > 1 && vectorB.Columns == 1))
			{
				
				
			}

			throw new IndexOutOfRangeException("Vector A and Vector B provided MUST be of EQUAL length");
		}

		public static Vector OP(Vector vector, float scalar, Operations operation)
		{
			GPU gpu = vector.gpu;

			vector.IncrementLiveCount();

			// Make the Output Vector
			Vector Output = new(gpu, vector.Length, vector.Columns);

			Output.IncrementLiveCount();

			// Check if the input & output are in Cache
			MemoryBuffer1D<float, Stride1D.Dense>
				buffer = Output.GetBuffer(),        // Output
				buffer2 = vector.GetBuffer();       // Input

			gpu.s_opFKernel(gpu.accelerator.DefaultStream, buffer.IntExtent, buffer.View, buffer2.View, scalar, new SpecializedValue<int>((int)operation));

			gpu.accelerator.Synchronize();

			vector.DecrementLiveCount();
			Output.DecrementLiveCount();

			return Output;
		}

		public Vector IPOP(float scalar, Operations operation)
		{
			IncrementLiveCount();

			// Check if the input & output are in Cache
			MemoryBuffer1D<float, Stride1D.Dense> buffer = GetBuffer(); // IO

			gpu.s_FloatOPKernelIP(gpu.accelerator.DefaultStream, buffer.IntExtent, buffer.View, scalar, new SpecializedValue<int>((int)operation));

			gpu.accelerator.Synchronize();

			DecrementLiveCount();

			return this;
		}


		internal static Vector _VectorVectorOP(Vector vectorA, Vector vectorB, Operations operation)
		{
			GPU gpu = vectorA.gpu;

			vectorA.IncrementLiveCount();
			vectorB.IncrementLiveCount();

			// Make the Output Vector
			Vector Output = new(gpu, vectorA.Length, vectorA.Columns);
			Output.IncrementLiveCount();

			// Check if the input & output are in Cache
			MemoryBuffer1D<float, Stride1D.Dense>
				buffer = Output.GetBuffer(),        // Output
				buffer2 = vectorA.GetBuffer(),      // Input
				buffer3 = vectorB.GetBuffer();      // Input

			// Run the kernel
			gpu.a_opFKernel(gpu.accelerator.DefaultStream, buffer.IntExtent, buffer.View, buffer2.View, buffer3.View, new SpecializedValue<int>((int)operation));

			// Synchronise the kernel
			gpu.accelerator.Synchronize();

			vectorA.DecrementLiveCount();
			vectorB.DecrementLiveCount();
			Output.DecrementLiveCount();

			// Return the result
			return Output;
		}

		internal static Vector _VecVecOP(Vector vectorA, Vector vectorB, Action<AcceleratorStream, Index1D, ArrayView<float>, ArrayView<float>, ArrayView<float>> kernel)
		{
			GPU gpu = vectorA.gpu;

			vectorA.IncrementLiveCount();
			vectorB.IncrementLiveCount();

			// Make the Output Vector
			Vector Output = new(gpu, vectorA.Length, vectorA.Columns);
			Output.IncrementLiveCount();

			// Check if the input & output are in Cache
			MemoryBuffer1D<float, Stride1D.Dense>
				buffer = Output.GetBuffer(),        // Output
				buffer2 = vectorA.GetBuffer(),      // Input
				buffer3 = vectorB.GetBuffer();      // Input

			// Run the kernel
			kernel(gpu.accelerator.DefaultStream, buffer.IntExtent, buffer.View, buffer2.View, buffer3.View);

			// Synchronise the kernel
			gpu.accelerator.Synchronize();

			vectorA.DecrementLiveCount();
			vectorB.DecrementLiveCount();
			Output.DecrementLiveCount();

			// Return the result
			return Output;
		}

		internal Vector _VectorVectorOP_IP(Vector vectorB, Operations operation)
		{
			vectorB.IncrementLiveCount();
			IncrementLiveCount();

			// Check if the input & output are in Cache
			MemoryBuffer1D<float, Stride1D.Dense>
				buffer = GetBuffer(),               // IO
				buffer2 = vectorB.GetBuffer();      // Input

			// Run the kernel
			gpu.a_FloatOPKernelIP(gpu.accelerator.DefaultStream, buffer.IntExtent, buffer.View, buffer2.View, new SpecializedValue<int>((int)operation));

			// Synchronise the kernel
			gpu.accelerator.Synchronize();

			vectorB.DecrementLiveCount();
			DecrementLiveCount();

			return this;
		}

		internal static Vector _VectorMatrixOP(Vector vector, Vector matrix, Operations operation)
		{
			GPU gpu = vector.gpu;

			vector.IncrementLiveCount();
			matrix.IncrementLiveCount();

			// Make the Output Vector
			Vector Output = new(gpu, vector.Length, vector.Columns);

			Output.IncrementLiveCount();

			// Check if the input & output are in Cache
			MemoryBuffer1D<float, Stride1D.Dense>
				buffer = Output.GetBuffer(),        // Output
				buffer2 = vector.GetBuffer(),       // Input
				buffer3 = matrix.GetBuffer();       // Input

			// Run the kernel
			gpu.vectormatrixOpKernel(gpu.accelerator.DefaultStream, matrix.RowCount(), buffer.View, buffer2.View, buffer3.View, matrix.Columns, new SpecializedValue<int>((int)operation));

			// Synchronise the kernel
			gpu.accelerator.Synchronize();

			vector.DecrementLiveCount();
			matrix.DecrementLiveCount();
			Output.DecrementLiveCount();

			// Return the result
			return Output;
		}

		internal Vector _VectorMatrixOP_IP(Vector matrix, Operations operation)
		{

			IncrementLiveCount();
			matrix.IncrementLiveCount();

			// Make the Output Vector
			Vector Output = new(gpu, matrix.Length, Columns);

			Output.IncrementLiveCount();

			// Check if the input & output are in Cache
			MemoryBuffer1D<float, Stride1D.Dense>
				buffer = Output.GetBuffer(),        // Output
				buffer2 = GetBuffer(),              // Input
				buffer3 = matrix.GetBuffer();       // Input

			// Run the kernel
			gpu.vectormatrixOpKernel(gpu.accelerator.DefaultStream, matrix.RowCount(), buffer.View, buffer2.View, buffer3.View, matrix.Columns, new SpecializedValue<int>((int)operation));

			// Synchronise the kernel
			gpu.accelerator.Synchronize();

			DecrementLiveCount();
			matrix.DecrementLiveCount();
			Output.DecrementLiveCount();

			return this;
		}



		public Vector Log_IP(float @base)
		{
			IncrementLiveCount();

			MemoryBuffer1D<float, Stride1D.Dense> buffer = GetBuffer();

			gpu.LogKernel(gpu.accelerator.DefaultStream, buffer.IntExtent, buffer.View, @base);

			gpu.accelerator.Synchronize();

			DecrementLiveCount();

			return this;
		}

	}



}
