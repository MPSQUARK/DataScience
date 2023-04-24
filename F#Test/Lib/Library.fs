namespace Lib

module Say =
    let hello name =
        printfn "Hello %s" name

    let goodbye name =
        printfn "Goodbye %s" name

    // add two fp32 numbers
    let add (a: float32) (b: float32) =
        a + b
