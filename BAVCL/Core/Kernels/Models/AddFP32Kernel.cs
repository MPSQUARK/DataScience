using BAVCL.Core.Kernels.Interfaces;

namespace BAVCL.Core.Kernels.Models
{
	internal struct TestFP32Kernel : IFP32_2XKernel<float>
	{
		public float Execute(float x)
		{
			return x;
		}
	}
	
	internal struct AddFP32Kernel : IFP32_3XKernel
	{
		public float Execute(float x, float y)
		{
			return x + y;
		}
	}

	internal struct SubFP32Kernel : IFP32_3XKernel
	{
		public float Execute(float x, float y)
		{
			return x - y;
		}
	}
	
	internal struct MulFP32Kernel : IFP32_3XKernel
	{
		public float Execute(float x, float y)
		{
			return x * y;
		}
	}
	
	internal struct DivFP32Kernel : IFP32_3XKernel
	{
		public float Execute(float x, float y)
		{
			return x / y;
		}
	}
	
	internal struct PowFP32Kernel : IFP32_3XKernel
	{
		public float Execute(float x, float y)
		{
			return (float)System.Math.Pow(x, y);
		}
	}
	
	internal struct DiffOfSquaresFP32Kernel : IFP32_3XKernel
	{
		public float Execute(float x, float y)
		{
			return (x * x) - (y * y);
		}
	}
	
	
}
