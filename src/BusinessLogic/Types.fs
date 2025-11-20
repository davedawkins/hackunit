module Types

type Sku = string 

type Address = 
    {
        Id : string
        Line1: string
        Line2: string option
        Town: string
        Postcode: string
        Country: string
    }

type Customer = 
    {
        Id : string
        Name : string
        Address : Address
    }

type LineItem =
    {
        Sku : string
        Qty : int
    }

type Order =
    {
        Id : string
        Parts : LineItem list
    }

type CustomerOrder =
    {
        Id : string
        DateReceived : System.DateTime
        Customer : Customer
        Order : Order
    }

type ShipmentGroup =
    Pending | InProgress | Complete

type ShipmentStatus = 
    NotStarted | OnHold of string | InAssembly | AwaitingDispatch | InTransit | Received | Rejected | Lost of string
    member __.Group =
        match __ with
        | NotStarted 
        | OnHold _ -> Pending
        | InAssembly 
        | AwaitingDispatch 
        | InTransit  -> InProgress
        | Received 
        | Rejected 
        | Lost _ -> Complete

type Shipment = 
    {
        Id : string
        Status : ShipmentStatus
        Orders : CustomerOrder list
    }