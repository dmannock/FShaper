﻿namespace Tests

open NUnit.Framework
open FSharper.Core
open FsUnit
open System

[<TestFixture>]
type EnumTests () =

 
    let formatFsharp (s:string) = 

        let indent = "                "
        s.Split ("\n") |> Array.map (fun x -> if x.StartsWith indent then x.Substring indent.Length else x) |> String.concat "\n"
        |> (fun s -> 
            s
                .Replace("\n    \n", "\n\n")
                .Replace("\n            \n", "\n\n")
                .Trim() )


    [<Test>]
    member this.``Simple enum maps directly`` () = 
        let csharp = 
             """enum EnumTest { 
                    None = 0,
                    First = 1
                }"""
    
        let fsharp = 
             """type EnumTest =
                    | None = 0
                    | First = 1"""
                   
        csharp |> Converter.run 
        |> (fun x -> printfn "%s" x; x)
        |> should equal (formatFsharp fsharp)

    [<Test>]
    member this.``Enum with non-sequential values`` () = 
        let csharp = 
                """enum EnumTest { 
                    None = 0,
                    Ten = 10,
                    Twenty = 20
                }"""
        
        let fsharp = 
                """type EnumTest =
                    | None = 0
                    | Ten = 10
                    | Twenty = 20"""
                       
        csharp |> Converter.run 
        |> (fun x -> printfn "%s" x; x)
        |> should equal (formatFsharp fsharp)

    [<Test>]
    member this.``Enum with flags attribute`` () = 
        let csharp = 
                """[Flags]
                enum Days 
                {
                    None = 0,
                    Sunday = 1,
                    Monday = 2,
                    Tuesday = 4,
                    Wednesday = 8,
                    Thursday = 16,
                    Friday = 32,
                    Saturday = 64
                }"""
            
        let fsharp = 
                """[<Flags>]
                type Days =
                    | None = 0
                    | Sunday = 1
                    | Monday = 2
                    | Tuesday = 4
                    | Wednesday = 8
                    | Thursday = 16
                    | Friday = 32
                    | Saturday = 64"""
                           
        csharp |> Converter.run 
        |> (fun x -> printfn "%s" x; x)
        |> should equal (formatFsharp fsharp)

    [<Test>]
    member this.``Enum with various whitespace challenges`` () = 
        let csharp = 
             """enum EnumTest { 
                    None     =   0,
                    Second   =  10,
                    Final    =  20
                }"""

        let fsharp = 
             """type EnumTest =
                    | None = 0
                    | Second = 10
                    | Final = 20"""
               
        csharp |> Converter.run 
        |> (fun x -> printfn "%s" x; x)
        |> should equal (formatFsharp fsharp)


    [<Test>]
    member this.``Enum with flags as hex`` () = 
        let csharp = 
                """[Flags]
                public enum CarOptions
                {
                    SunRoof = 0x01,
                    Spoiler = 0x02,
                    FogLights = 0x04,
                    TintedWindows = 0x08,
                    HeatedSeats = 0x10
                }"""
                
        let fsharp = 
                """[<Flags>]
                type CarOptions =
                    | SunRoof = 1
                    | Spoiler = 2
                    | FogLights = 4
                    | TintedWindows = 8
                    | HeatedSeats = 16"""
                               
        csharp |> Converter.run 
        |> (fun x -> printfn "%s" x; x)
        |> should equal (formatFsharp fsharp)
        
    [<Test>]
    member this.``Enum with specified type of byte`` () = 
        let csharp = 
                """enum EnumTest: byte
                {
                    Min = 0,
                    Max = 255
                }"""
            
        let fsharp = 
                """type EnumTest =
                    | Min = 0uy
                    | Max = 255uy"""
                           
        csharp |> Converter.run 
        |> (fun x -> printfn "%s" x; x)
        |> should equal (formatFsharp fsharp)

    [<Test>]
    member this.``Enum with specified type of long (without numeric suffixes)`` () = 
        let csharp = 
                """enum Range : long { Max = 2147483648, Min = 255 };"""
                
        let fsharp = 
                """type Range =
                    | Max = 2147483648L
                    | Min = 255L"""
                               
        csharp |> Converter.run 
        |> (fun x -> printfn "%s" x; x)
        |> should equal (formatFsharp fsharp)