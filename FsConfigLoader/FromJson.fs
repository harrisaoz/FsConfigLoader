module FsConfigLoader.FromJson

open Microsoft.Extensions.Configuration

open FsCombinators.ResultExtensions
open FsConfigLoader.Core
open FsConfigLoader.FromText
open FsConfigLoader.FromFile

let fromJsonText: TextLoader =
    fun (ConfigSource json) ->
        tryAsResult
        <| fun () ->
            let bytes =
                System.Text.Encoding.UTF8.GetBytes json

            ConfigurationBuilder()
                .AddJsonStream(new System.IO.MemoryStream(bytes))
                .Build()
            :> IConfiguration

let fromJsonFileInDir: FileLoader =
    fun (ConfigSource (maybeDir, filename)) ->
        tryAsResult
        <| fun () ->
            let builder: IConfigurationBuilder =
                match maybeDir with
                | None -> ConfigurationBuilder()
                | Some dir -> ConfigurationBuilder().SetBasePath(dir)

            builder.AddJsonFile(filename).Build() :> IConfiguration

let loadFromFile = load fromJsonFileInDir
let loadFromText = load fromJsonText
