#r "paket:
nuget FSharp.Core 4.6.0.0
nuget Fake.Core.Target
nuget Fake.DotNet.Cli //"

#load ".fake/build.fsx/intellisense.fsx"
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

Target.initEnvironment()

let solutionFile = "FSharper.sln"
let fshaperProject = "FSharper.Core/FSharper.Core.fsproj"
let fshaperTestsProject = "FSharper.Tests.fsproj"
let testsProjectDir = "FSharper.Tests"

// Lazily install DotNet SDK in the correct version if not available
let install = lazy DotNet.install DotNet.Versions.Release_2_1_302

// Set general properties without arguments
let inline dotnetSimple arg = DotNet.Options.lift install.Value arg

let inline withWorkDir wd =
    DotNet.Options.lift install.Value
    >> DotNet.Options.withWorkingDirectory wd

Target.create "Clean" (fun _ ->
    !! "**/bin"
    ++ "**/obj"
    |> Shell.cleanDirs 
)

Target.create "Restore" (fun _ -> DotNet.restore dotnetSimple solutionFile )
Target.create "Build" (fun _ -> DotNet.build dotnetSimple solutionFile )
Target.create "Test" (fun _ -> 
  System.IO.Directory.SetCurrentDirectory testsProjectDir
  DotNet.test id fshaperTestsProject)

Target.create "All" ignore

"Clean"
  ==> "Restore"
  ==> "Build"
  ==> "Test"
  ==> "All"

Target.runOrDefault "Test"
