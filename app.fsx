#r "packages/FSharp.Compiler.Service/lib/net45/FSharp.Compiler.Service.dll"
#r "packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#r "packages/Suave/lib/net40/Suave.dll"
#r "System.Runtime.Serialization.dll"
#load "paket-files/tpetricek/fsharp-web-editors/server/editor.fs"
#load "paket-files/tpetricek/fsharp-web-editors/server/evaluator.fs"

open Suave
open Suave.Filters
open Suave.Operators
open System.IO
open FsWebTools

// Initialization code that's hidden from the user
let scriptSetup = [ "open System" ]
// Folder in which F# Interactive is started
let rootFolder = __SOURCE_DIRECTORY__
// Implicit name of script in which code is checked
let scriptName = Path.Combine(rootFolder, "test.fsx")

// Folder with client-side scripts required by CodeMirror
let clientFolder = Path.Combine(__SOURCE_DIRECTORY__, "paket-files", "tpetricek", "fsharp-web-editors", "client")

let app = 
  choose [
    // Handlers for editor and F# interactive
    Editor.part scriptName scriptSetup
    Evaluator.part rootFolder scriptSetup    
    // Serving demo and required script files
    path "/" >=> Files.browseFile clientFolder "fsharp-web-editors-demo.html"
    Files.browse clientFolder ]

let config = 
  { defaultConfig with 
      bindings = [HttpBinding.mkSimple HTTP "0.0.0.0" 8083 ] 
      logger = Logging.Loggers.saneDefaultsFor Logging.LogLevel.Warn }

startWebServer config app