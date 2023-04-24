namespace BAVCL.Core.Kernels.Interfaces
{
	internal interface IFP32_2XKernel
	{
		float Execute(float x);
	}

	internal interface IFP32_3XKernel
	{
		float Execute(float x, float y);
	}

}