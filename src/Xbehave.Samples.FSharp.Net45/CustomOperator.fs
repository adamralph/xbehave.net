// <copyright file="CustomOperator.fs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

// Quickstart sample conversion in F# using a custom operator
// based on http://fssnip.net/ko by Phillip Trelford http://trelford.com/blog
namespace Xbehave.Samples.FSharp

module CustomOperator =
    open Xbehave
    open Xunit

    // custom operator
    let (-->) (s:string) f = s.f(System.Action< >(f)) |> ignore

    // SUT
    type Calculator () = member __.Add(x,y) = x + y

    [<Scenario>]
    let addition(x:int, y:int, calculator:Calculator, answer:int) =
        let x, y, calculator, answer = ref x, ref y, ref calculator, ref answer

        "Given the number 1"
            --> fun () -> x := 1

        "And the number 2"
            --> fun () -> y := 2

        "And a calculator"
            --> fun () -> calculator := Calculator()

        "When I add the numbers together"
            --> fun () -> answer := (!calculator).Add(!x, !y)

        "Then the answer is 3"
            --> fun () -> Assert.Equal(3, !answer)

    [<Scenario>]
    [<Example(1, 2, 3)>]
    [<Example(2, 3, 5)>]
    let additionWithExamples(x:int, y:int, expectedAnswer:int, calculator:Calculator, answer:int) =
        let calculator, answer = ref calculator, ref answer
        
        "Given the number {0}"
            --> id
        
        "And the number {1}"
            --> id
        
        "And a calculator"
            --> fun () -> calculator := Calculator()
        
        "When I add the numbers together"
            --> fun () -> answer := (!calculator).Add(x, y)
        
        "Then the answer is {2}"
            --> fun () -> Assert.Equal(expectedAnswer, !answer)
