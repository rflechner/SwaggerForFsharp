#load ".fake/build.fsx/intellisense.fsx"

#if !FAKE
#r "netstandard"
let execContext = Fake.Core.Context.FakeExecutionContext.Create false "build.fsx" []
Fake.Core.Context.setExecutionContext (Fake.Core.Context.RuntimeContext.Fake execContext)
#endif

open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

Target.create "Clean" (fun _ ->
  !! "src/**/bin"
  ++ "src/**/obj"
  |> Shell.cleanDirs 
)

Target.create "Restore" (fun _ ->
  !! "src/**/*.*proj"
  |> Seq.iter (DotNet.restore id)
)

Target.create "Build" (fun _ ->
  !! "src/**/*.*proj"
  |> Seq.iter (DotNet.build id)
)

Target.create "Pack" (fun _ ->
  let nugetsDir = __SOURCE_DIRECTORY__ </> "releases"
  !! "src/SwaggerForFsharp.Giraffe/*.fsproj"
  |> Seq.iter (
      DotNet.pack (fun settings -> { settings with OutputPath=Some nugetsDir })
     )
)

Target.create "All" ignore

"Clean"
==> "Restore"
==> "Build"
==> "Pack"
==> "All"

Target.runOrDefault "All"
