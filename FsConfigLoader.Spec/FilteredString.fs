module FsConfigLoader.Spec.ParamName

open FsCheck

type ParamName = ParamName of string
type ParamValue = ParamValue of string

let genFilteredString p f =
    let nesMap f (NonEmptyString s) = f s

    Arb.Default.NonEmptyString().Generator
    |> Gen.filter (nesMap p)
    |> Gen.map (nesMap f)

let genParamName =
    genFilteredString (String.forall System.Char.IsLetter) ParamName

let genParamValue =
    genFilteredString (String.forall System.Char.IsDigit) ParamValue

let arbParamName =
    genParamName |> Arb.fromGen

let arbParamValue =
    genParamValue |> Arb.fromGen
