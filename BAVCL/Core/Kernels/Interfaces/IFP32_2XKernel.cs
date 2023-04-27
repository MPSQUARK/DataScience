namespace BAVCL.Core.Kernels.Interfaces
{
	internal interface IFP32_2XKernel<T> where T : unmanaged
	{
		T Execute(T x);
	}

	internal interface IFP32_3XKernel
	{
		float Execute(float x, float y);
	}

}