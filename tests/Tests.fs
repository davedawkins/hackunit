module Tests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

let arithmeticTests =
    testList "Arithmetic tests" [
        test "plus works" {
            Expect.equal (1 + 1) 2 "plus"
        }

        test "Test for falsehood" {
            Expect.isFalse (1 = 2) "false"
        }

        testAsync "Test async code" {
            let! x = async { return 21 }
            let answer = x * 3
            Expect.equal 42 answer "async"
        }
    ]

let allTests = testList "All" [
    arithmeticTests
]

[<EntryPoint>]
let main args =
#if FABLE_COMPILER
    Mocha.runTests allTests
#else
    runTestsWithCLIArgs []  args allTests
#endif
