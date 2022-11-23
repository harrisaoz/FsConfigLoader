module FsConfigLoader.Core

open Microsoft.Extensions.Configuration

type ConfigResult = Result<IConfiguration, string>
type ConfigSource<'a> = ConfigSource of 'a
type Loader<'a> = ConfigSource<'a> -> ConfigResult

let load (loader: Loader<'a>) source : ConfigResult = loader source
