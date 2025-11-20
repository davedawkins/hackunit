module Shipments

open Sutil 
open Types
open Database

let private shipments : IStore<Shipment[]> = Store.make Array.empty

let init (db : IDatabase) =
    promise {
        let! data = db.AllShipments()
        data |> Store.set shipments
    }

let allShipments() : System.IObservable<Shipment[]> =
    shipments

let shipmentsByGroup() : System.IObservable<(ShipmentGroup * Shipment[])[]> =
    shipments |> Store.map (fun x -> x |> Array.groupBy _.Status.Group)

let updateShipmentStatus( id : string, status : ShipmentStatus ) =
    shipments.Value
    |> Array.map (fun x -> if x.Id = id then { x with Status = status } else x)
    |> Store.set shipments
