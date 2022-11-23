module FsConfigLoader.Read

open Microsoft.Extensions.Configuration

let section (config: IConfiguration) sectionName : IConfiguration option =
    try
        config.GetSection sectionName :> IConfiguration
        |> Some
    with
    | ex -> None

let readMany (section: IConfiguration) parameterName =
    section.AsEnumerable true
    |> Seq.filter (fun (KeyValue (k, v)) ->
        k.StartsWith($"%s{parameterName}:")
        && (not << System.String.IsNullOrWhiteSpace) v)
    |> Seq.map (fun (KeyValue (_, v)) -> v)

let read (config: IConfiguration) parameterName =
    config.Item parameterName
    |> Option.ofObj
    |> Option.filter (not << System.String.IsNullOrWhiteSpace)
