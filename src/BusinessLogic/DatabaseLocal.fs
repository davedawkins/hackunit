module DatabaseLocal

open Types
open Database
open Fable.Core

[<Import("getFile", "./fs.js")>] 
let getFile( path : string ) : string = jsNative

type private DatabaseLocal( dataRoot : string ) =

    member inline __.loadTable( file : string ) : JS.Promise<'T[]> =
        promise {
            let items = Thoth.Json.Decode.Auto.fromString<'T[]>( getFile( dataRoot + "/" + file ) )
            match items with
            | Ok data -> return data
            | Error e -> return failwithf "Loading %s: %s" file e
        }

    interface IDatabase with        

        member this.AllCustomerOrders(): JS.Promise<CustomerOrder array> = 
            this.loadTable "all-customer-orders.json"
        member this.AllCustomers(): JS.Promise<Customer array> = 
            this.loadTable "all-customers.json"
        member this.AllOrders(): JS.Promise<Order array> = 
            this.loadTable "all-orders.json"
        member this.AllShipments(): JS.Promise<Shipment array> = 
            this.loadTable "all-shipments.json"
        member this.AllSkus(): JS.Promise<Sku array> = 
            this.loadTable "all-skus.json"


let create (dataRoot : string ) : IDatabase = DatabaseLocal(dataRoot)