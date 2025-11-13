module StoreTests

open Sutil

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

let storeTests =
    testList "Store tests" [
        test "init works" {
            let store = Store.make 42
            Expect.equal store.Value 42 "store.Value = 42"
        }

        test "update works" {
            let store = Store.make 42
            Store.set store 24
            Expect.equal store.Value 24 "store.Value = 24"
        }

        test "subscribe works" {
            let store = Store.make 42
            let mutable n = 0
            
            store.Subscribe( fun x -> 
                n <- x
            ) |> ignore

            Store.set store 24
            Expect.equal n 24 "n = 24"
        }

        test "unsubscribe works" {
            let store = Store.make 0
            let mutable n = -1
            
            let sub = store.Subscribe( fun x -> 
                n <- x
            ) 

            Store.set store 1
            Expect.equal n 1 "n = 1" // Prove that we are subscribed
            sub.Dispose()
            Store.set store 2
            Expect.equal n 1 "n = 1" // 'n' shouldn't have changed
        }
    
        test "map works" {
            let store = Store.make 0
            let mutable n = -1

            let doubled = store |> Store.map (fun x -> x * 2)

            doubled.Subscribe( fun x -> n <- x ) |> ignore

            4 |> Store.set store

            Expect.equal n 8 "n = 8"
        }
    ]
