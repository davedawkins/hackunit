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

let shipmentsByStatus() : System.IObservable<(ShipmentStatus * Shipment[])[]> =
    shipments |> Store.map (fun x -> x |> Array.groupBy _.Status)

let updateShipmentStatus( id : string, status : ShipmentStatus ) =
    ()
