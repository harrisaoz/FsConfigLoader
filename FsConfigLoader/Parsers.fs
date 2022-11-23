module FsConfigLoader.Parsers

let tryParseInt (s: string) : int option =
    try
        s |> int |> Some
    with
    | :? System.FormatException -> None
