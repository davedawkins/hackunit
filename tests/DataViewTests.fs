module DataViewTests

open Database
open Types
open Fable.Core

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

let testCasePromise (label : string) (p : JS.Promise<unit>) =
    testCaseAsync label (p |> Async.AwaitPromise)

let shipmentTests =
    testList "Shipment Tests" [

        testCasePromise "Shipments By Group" <| promise { 
            
            // Initialize the Shipments module with test data as in IDatabase interface
            // The Shipments module doesn't care if this is production or test data
            do! Shipments.init (DatabaseLocal.create(DatabaseTests.DATA_ROOT))
            
            // Get the observable that we want to test
            let grouped = Shipments.shipmentsByGroup()

            // Initialize our capture variable - this will record the most recent
            // value of "grouped"
            let mutable grouping : Map<ShipmentGroup, Shipment[]> = Map.empty

            // Subscribe to our test observable, capturing its value into 'grouped'
            grouped.Subscribe( fun x -> grouping <- x |> Map) |> ignore
            
            // Verify initial groupings
            // We can do this because we know what the origin of 'grouped' is a Sutil store, 
            // and a Sutil store *always* publishes its current value to any new subscribers.
            Expect.hasLength (grouping[ShipmentGroup.Pending]) 9 "Pending Shipments"
            Expect.hasLength (grouping[ShipmentGroup.InProgress]) 33 "InProgress Shipments"
            Expect.hasLength (grouping[ShipmentGroup.Complete]) 8 "Complete Shipments"
        
            // Update a single shipment from "InTransit" (not asserted) to "Received"
            Shipments.updateShipmentStatus( "SHIP-040", Received )

            // Verify the new groupings (captured by the subscription)
            Expect.hasLength (grouping[ShipmentGroup.Pending]) 9 "Pending Shipments"
            Expect.hasLength (grouping[ShipmentGroup.InProgress]) 32 "InProgress Shipments"
            Expect.hasLength (grouping[ShipmentGroup.Complete]) 9 "Complete Shipments"
        
        }
        
    ]

