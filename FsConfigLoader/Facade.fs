module FsConfigLoader.Facade

let tryRun readConfigResult runWithProgramSpecificConfiguration argv =
    Core.ConfigSource(Array.tryItem 1 argv, Array.item 0 argv)
    |> FromJson.loadFromFile
    |> Result.bind readConfigResult
    |> Result.map runWithProgramSpecificConfiguration

module SimpleConsoleApp =
    let tryWithConfiguration readConfigResult runWithProgramSpecificConfiguration argv =
        System.Console.OutputEncoding <- System.Text.Encoding.UTF8

        tryRun readConfigResult runWithProgramSpecificConfiguration argv
        |> function
            | Result.Ok _ -> 0
            | Result.Error msg ->
                // ReSharper disable once FSharpInterpolatedString
                printfn "[\u274c] {%s}" msg
                1
