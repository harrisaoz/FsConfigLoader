module FsConfigLoader.FromFile

open FsConfigLoader.Core

type MaybeDir = string option
type Filename = string

type FileLoader = Loader<MaybeDir * Filename>
