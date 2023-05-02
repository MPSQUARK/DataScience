using System.Collections.Generic;
using FSL;

namespace BAVCL.Core.FSLWrappers
{
	public sealed class FSLWrappers
	{
		public static Dictionary<int, KernelModule.CompiledKernel> GetKernels() => KernelModule.kernels;
		public static void AddKernel(int id, KernelModule.CompiledKernel kernel) => KernelModule.kernels.Add(id, kernel);
		public static void RemoveKernel(int id) => KernelModule.kernels.Remove(id);
		public static void ClearKernels() => KernelModule.kernels.Clear();
		public static bool ContainsKernel(int id) => KernelModule.kernels.ContainsKey(id);
		public static KernelModule.CompiledKernel GetKernel(int id) => KernelModule.kernels[id];
		
	}
}