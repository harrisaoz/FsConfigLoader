module FsConfigLoader.Spec.FromJsonSpec

open FsCheck
open FsCheck.Xunit

open FsConfigLoader.Core
open FsConfigLoader
open FsConfigLoader.Spec.GenJsonObject

[<Property>]
let ``In general, arbitrary text is not a valid JSON configuration document`` (NonEmptyString s) =
    load FromJson.fromJsonText (ConfigSource s)
    |> Result.isError

[<Property>]
let ``Arbitrary valid JSON should be loaded to an Ok Configuration`` () =
    ProtocolGenerators.JObject() |> Prop.forAll
    <| fun json ->
        let jsonText = string json

        ConfigSource jsonText
        |> load FromJson.fromJsonText
        |> Result.isOk
        |> Prop.label jsonText

let example1 =
    """
{
  "m": {
    "m": "ok"
  },
  "C": {
    "m": []
  },
}"""

[<Property>]
let ``Label names may be re-used in isolated contexts`` () =
    ConfigSource example1
    |> load FromJson.fromJsonText
    |> Result.isOk

let example2 =
    """
{
  "t": {},
  "T": "$",
}"""

[<Property>]
let ``Labels differing in only in case are considered duplicates`` () =
    ConfigSource example2
    |> load FromJson.fromJsonText
    |> Result.isError

[<Property>]
let ``Inconsistent types in an array are permitted`` () =
    ConfigSource """{ "x": [ false, 0, { "y": 5E-324 }, [], "a" ] }"""
    |> load FromJson.fromJsonText
    |> Result.isOk

[<Property>]
let ``Zero-length text is not permitted`` () =
    load FromJson.fromJsonText (ConfigSource "")
    |> Result.isError

[<Property>]
let ``An empty JSON object is permitted`` () =
    FromJson.loadFromText (ConfigSource "{}")
    |> Result.isOk
