module DatabaseTests

open Database
open Fable.Core

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

let [<Literal>] DATA_ROOT = "./data"

let testCasePromise (label : string) (p : JS.Promise<unit>) =
    testCaseAsync label (p |> Async.AwaitPromise)

let dbTests =
    testList "Database Tests" [

        testCasePromise "load SKUS" <| promise { 
            let db : IDatabase = DatabaseLocal.create DATA_ROOT
            let! skus = db.AllSkus()
            
            Expect.hasLength skus 20 "20 skus"
        }
        
        testCasePromise "load CUSTOMERS" <| promise { 
            let db : IDatabase = DatabaseLocal.create DATA_ROOT
            let! customers = db.AllCustomers()
            
            Expect.hasLength customers 10 "10 customers"
        }

        testCasePromise "load ORDERS" <| promise { 
            let db : IDatabase = DatabaseLocal.create DATA_ROOT
            let! orders = db.AllOrders()
            
            Expect.hasLength orders 50 "50 orders"
        }

        testCasePromise "load SHIPMENTS" <| promise { 
            let db : IDatabase = DatabaseLocal.create DATA_ROOT
            let! shipments = db.AllShipments()
            
            Expect.hasLength shipments 50 "50 shipments"
        }

        testCasePromise "load CUSTOMER ORDERS" <| promise { 
            let db : IDatabase = DatabaseLocal.create DATA_ROOT
            let! customerOrders = db.AllCustomerOrders()
            
            Expect.hasLength customerOrders 50 "50 customer orders"
        }

    ]

