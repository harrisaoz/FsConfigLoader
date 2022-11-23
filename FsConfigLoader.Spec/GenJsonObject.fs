/// The basis for this module is the gist at:
/// https://gist.github.com/vbfox/9e84088cd48861e70e386e632b06fbcc
module FsConfigLoader.Spec.GenJsonObject

open FsCheck
open Newtonsoft.Json.Linq

module OptionExt = FsCombinators.OptionExtensions

open FsConfigLoader.Spec.ParamName

let genByteArray =
    Gen.arrayOf Arb.generate<byte>

let paramValueAsJValue (ParamValue s) = JValue s

let genJValue =
    [ Arb.generate<int> |> Gen.map JValue
      genParamValue |> Gen.map paramValueAsJValue
      Arb.generate<System.UInt32> |> Gen.map JValue
      Arb.generate<bool> |> Gen.map JValue
      Arb.generate<char> |> Gen.map JValue
      Arb.generate<System.DateTime> |> Gen.map JValue
      Arb.generate<System.DateTimeOffset>
      |> Gen.map JValue
      Arb.generate<System.Decimal> |> Gen.map JValue
      Arb.generate<double> |> Gen.map JValue
      Arb.generate<System.Guid> |> Gen.map JValue
      Arb.generate<int64> |> Gen.map JValue
      Arb.generate<System.TimeSpan> |> Gen.map JValue
      Arb.generate<System.UInt64> |> Gen.map JValue ]
    |> Gen.oneof

let mapGenToToken<'t when 't :> JToken> =
    Gen.map (fun (x: 't) -> x :> JToken)

let rec genJToken' size =
    match size with
    | 0 -> genJValue |> mapGenToToken
    | n when n > 0 ->
        Gen.oneof [ genJValue |> mapGenToToken
                    genJArray' (n / 2) |> mapGenToToken
                    genJObject' (n / 2) |> mapGenToToken ]
    | _ -> invalidArg "s" "Only positive arguments are allowed"

and genJArray' s =
    match s with
    | 0 -> Gen.constant (JArray())
    | n when n > 0 -> genJToken' n |> Gen.arrayOf |> Gen.map JArray
    | _ -> invalidArg "s" "Only positive arguments are allowed"

and genJObject' s =
    match s with
    | 0 -> Gen.constant (JObject())
    | n when n > 0 ->
        gen {
            let! key = genParamName
            let! value = genJToken' n
            return key, value
        }
        |> Gen.arrayOf
        |> Gen.map (fun arr ->
            let result = JObject()

            for ParamName key, value in arr do
                OptionExt.tryAsOption
                <| fun () -> result.Add(key.ToLower(), value)
                |> ignore

            result)
    | _ -> invalidArg "s" "Only positive arguments are allowed"

let genJToken = Gen.sized genJToken'
let genJArray = Gen.sized genJArray'
let genJObject = Gen.sized genJObject'

type ProtocolGenerators =
    static member JToken() = Arb.fromGen genJToken
    static member JArray() = Arb.fromGen genJArray
    static member JObject() = Arb.fromGen genJObject

let config =
    { Config.Default with Arbitrary = [ typeof<ProtocolGenerators> ] }
