namespace FSL

module KernelModule =
    open System
    open System.Collections.Generic

    type CompiledKernel<'T> =
        { action : 'T }
        static member FloatFloat (f : Action<Single, Single>) = { action = f }
        static member Int (f : Action<int>) = { action = f }
        static member DoubleDoubleDouble (f : Action<double, double, double>) = { action = f }

    type CompiledKernel =
        | FloatFloat of CompiledKernel<Action<Single, Single>>
        | Int of CompiledKernel<Action<int>>
        | DoubleDoubleDouble of CompiledKernel<Action<double, double, double>>


    // Kernel Container
    let mutable kernels: Dictionary<int,CompiledKernel> = new Dictionary<int, CompiledKernel>()

    let getKernel (key: int) =
        match kernels[key] with
        | FloatFloat kernel -> (FloatFloat kernel)
        | Int kernel -> (Int kernel)
        | DoubleDoubleDouble kernel -> (DoubleDoubleDouble kernel)
