module Database

open Fable.Core.JS
open Types

type IDatabase =
    abstract AllOrders: unit -> Promise<Order[]>
    abstract AllCustomerOrders: unit -> Promise<CustomerOrder[]>
    abstract AllShipments: unit -> Promise<Shipment[]>
    abstract AllCustomers: unit -> Promise<Customer[]>
    abstract AllSkus: unit -> Promise<Sku[]>