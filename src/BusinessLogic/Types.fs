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

type ShipmentStatus = 
    NotStarted | OnHold of string | InAssembly | AwaitingDispatch | InTransit | Received | Rejected | Lost of string

type Shipment = 
    {
        Id : string
        Status : ShipmentStatus
        Orders : CustomerOrder list
    }