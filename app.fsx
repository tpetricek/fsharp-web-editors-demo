#r "packages/FSharp.Compiler.Service/lib/net45/FSharp.Compiler.Service.dll"
#r "packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#r "packages/Suave/lib/net40/Suave.dll"
#r "System.Runtime.Serialization.dll"
#load "paket-files/tpetricek/fsharp-web-editors/server/editor.fs"

open Suave
open Suave.Filters
open Microsoft.FSharp.Compiler.SourceCodeServices
open FsWebTools

let scriptSetup = [ "open System" ]
let scriptName = __SOURCE_DIRECTORY__ + "/test.fsx"

let app ctx = async {
  try
    printfn "Handling: %A" ctx.request
    let! res = 
      ctx |> choose [
        Editor.part scriptName scriptSetup (FSharpChecker.Create())
        Files.browse (System.IO.Path.Combine(__SOURCE_DIRECTORY__, "paket-files", "tpetricek", "fsharp-web-editors", "client")) ]
    printfn "Produced: %A" ctx.response
    return res 
  with e ->
    printfn "Something went wrong: %A" e
    return None }

let config = 
  { defaultConfig with 
      bindings = [HttpBinding.mkSimple HTTP "0.0.0.0" 80] 
      logger = Logging.Loggers.saneDefaultsFor Logging.Verbose }

startWebServer config app