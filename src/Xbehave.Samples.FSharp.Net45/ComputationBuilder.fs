// <copyright file="ComputationBuilder.fs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

// Quickstart sample conversion using a custom F# computation builder for steps and custom functions for Given/When/Then/And/But
// based on http://fssnip.net/kq by Tomas Petricek http://tomasp.net/blog
namespace Xbehave.Samples.FSharp

module ComputationBuilder =
    open Xbehave
    open Xunit

    // computation builder
    type StepBuilder(text:string) = 
        member x.Zero() = ()
        member x.Delay(f) = f
        member x.Run(f) = text.f(System.Action< >(f)) |> ignore

    // helper functions
    let Given text = StepBuilder("Given " + text)
    let When text = StepBuilder("When " + text)
    let Then text = StepBuilder("Then " + text)
    let And text = StepBuilder("And " + text)
    let But text = StepBuilder("But " + text)

    // SUT
    type Calculator () = member __.Add(x,y) = x + y

    [<Scenario>]
    let addition(x:int, y:int, calculator:Calculator, answer:int) =
        let x, y, calculator, answer = ref x, ref y, ref calculator, ref answer

        Given "the number 1"
            { x := 1 }

        And "the number 2"
            { y := 2 }

        And "a calculator"
            { calculator := Calculator() }

        When "I add the numbers together"
            { answer := (!calculator).Add(!x, !y) }

        Then "the answer is 3"
            { Assert.Equal(3, !answer) }
