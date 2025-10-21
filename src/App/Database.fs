module Database

module DomainTypes =

    type User = {
        Id : int
        Name : string
        Surname : string
        Groups : string[]
    }

    type Group = {
        Name : string
        Description : string
    }


[<RequireQualifiedAccess>]
module Db =
    open DomainTypes

    let initDb = [
        { Id = 1; Name = "Hans"; Surname = "Emil"; Groups = [||] }
        { Id = 2; Name = "Max"; Surname = "Mustermann"; Groups = [||] }
        { Id = 3; Name = "Roman"; Surname = "Tisch"; Groups = [||] }
        { Id = 4; Name = "Steve"; Surname = "Miller"; Groups = [||] }
    ]

    let mutable nextId = 1 + (initDb |> List.map (fun n -> n.Id) |> List.max)
    let mutable db = initDb

    let fetchAll() : list<User> = db

    let assertExists id =
        db |> List.findIndex (fun p -> p.Id = id) |> ignore

    let removeId id =
        assertExists id
        db <- db |> List.filter (fun p -> p.Id <> id)
        Browser.Dom.console.log($"{db}")
        id

    let create (p : User) =
        if p.Name = "" && p.Surname = "" then failwith "Invalid name"
        let p' = { p with Id = nextId }
        nextId <- nextId + 1
        db <- db @ [ p' ]
        p'

    let update (p : User) =
        assertExists p.Id
        db <- db |> List.map (fun x -> if x.Id = p.Id then p else x)
        Browser.Dom.console.log($"{db}")
        p
