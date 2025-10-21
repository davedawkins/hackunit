module Users

open Sutil
open Sutil.Bulma
open type Feliz.length

open Sutil.Styling
open System
open Database
open DomainTypes

type Model = {
    Names : User list;
    Name : string
    Surname : string
    Filter : string
    Selected : User option
    Error : string
}

type Message =
    | Create
    | Update
    | Delete
    | Created of User
    | Updated of User
    | Deleted of int
    | SetFilter of string
    | SetName of string
    | SetSurname of string
    | RequestAllNames
    | AllNames of User list
    | Select of id:int
    | ClearSelection
    | Error of string
    | Exception of Exception
    | ClearError

let filter m = m.Filter
let name m = m.Name
let surname m = m.Surname
let error m = m.Error
let names m = m.Names

let selection m = match m.Selected with |None->[] |Some n -> List.singleton n.Id
let matchName filter (name : User) = filter = "" || name.Surname.StartsWith(filter)
let filteredNames m = m.Names |> List.filter (matchName m.Filter)

let canCreate m = m.Name <> "" || m.Surname <> ""
let canUpdate m = m.Selected.IsSome && (m.Name <> "" || m.Surname <> "")
let canDelete m = m.Selected.IsSome

let init () =
    { Names = []; Selected = None; Filter = ""; Name = ""; Surname = ""; Error = "" }, Cmd.ofMsg RequestAllNames

let update msg model =
    match msg with
    | ClearError ->
        { model with Error = "" }, Cmd.none
    | Error msg ->
        { model with Error = msg}, Cmd.none
    | Exception x ->
        { model with Error = x.Message}, Cmd.none
    | Created n ->
        { model with Names = model.Names @ [n] }, Cmd.batch [ Cmd.ofMsg ClearError; Cmd.ofMsg (Select n.Id) ]
    | Updated n ->
        let updatedNames = model.Names |> List.map (fun x -> if x.Id = n.Id then n else x)
        { model with Names = updatedNames }, Cmd.ofMsg ClearError
    | Deleted id ->
        let updatedNames = model.Names |> List.filter (fun x -> x.Id <> id)
        let selnMsg = if updatedNames.IsEmpty then ClearSelection else Select (updatedNames.Head.Id)
        { model with Names = updatedNames }, Cmd.batch [ Cmd.ofMsg ClearError; Cmd.ofMsg selnMsg ]
    | ClearSelection ->
        { model with Name = ""; Surname = ""; Selected = None}, Cmd.none
    | Select id ->
        let n = model.Names |> List.find (fun n -> n.Id = id)
        { model with Selected = Some n }, Cmd.batch [ Cmd.ofMsg (SetName n.Name); Cmd.ofMsg (SetSurname n.Surname)]
    | Create ->
        let n = { Name = model.Name; Surname = model.Surname; Id = 0; Groups = [||] }
        model, Cmd.OfFunc.either Db.create n Created Exception
    | Update ->
        match model.Selected with
        | None -> model, Cmd.ofMsg (Error "No record selected to update")
        | Some n -> model, Cmd.OfFunc.either Db.update { n with Name = model.Name; Surname = model.Surname } Updated Exception
    | Delete  ->
        match model.Selected with
        | None -> model, Cmd.ofMsg (Error "No record selected to delete")
        | Some n -> model, Cmd.OfFunc.either Db.removeId n.Id Deleted Exception
    | SetFilter f ->
        { model with Filter = f }, Cmd.ofMsg ClearSelection
    | SetName n ->
        { model with Name = n }, Cmd.none
    | SetSurname n ->
        { model with Surname = n }, Cmd.none
    | RequestAllNames ->
        model, Cmd.OfFunc.perform Db.fetchAll () AllNames
    | AllNames names ->
        { model with Names = names }, Cmd.none


let appStyle = [
    rule "div.select, select, .width100" [
        Css.width (percent 100) // Streatch list and text box to fit column, looks nicer right aligned
    ]
    rule ".field-label" [
        Css.flexGrow 2// Allow more space for field label
    ]
    rule "label.label" [
        Css.textAlignLeft // To match 7GUI spec
    ]
]

let create() =
    let model, dispatch = () |> Store.makeElmish init update ignore

    let labeledField (label:string) (model : IObservable<string>) (dispatch : string -> unit) =
        bulma.field.div [
            field.isHorizontal
            bulma.fieldLabel [ bulma.label [ Html.text label ] ]
            bulma.fieldBody [
                bulma.control.div [
                    Attr.className "width100"
                    bulma.input.text [
                        Bind.attr ("value",model,dispatch)
                    ]]]]

    let button (label:string) enabled message =
        bulma.control.p [
            bulma.button.button [
                Bind.attr ("disabled", model .> (enabled >> not))
                Html.text label
                Ev.onClick (fun _ -> dispatch message)
                ] ]

    bulma.section [
        bulma.columns [
            bulma.column [
                column.is6
                labeledField "Filter prefix:" (model |> Store.map filter) (dispatch << SetFilter)
            ]
        ]

        bulma.columns [
            bulma.column [
                column.is6

                Sutil.Bulma.Helpers.selectList [
                    Attr.size 6

                    let viewNames =
                        model |> Store.map filteredNames |> Observable.distinctUntilChanged

                    Bind.each (viewNames,fun n ->
                        Html.option [
                            Attr.value n.Id
                            (sprintf "%s, %s" n.Surname n.Name) |> Html.text
                            ])

                    Bind.selected (model |> Store.map selection, List.exactlyOne >> Select >> dispatch)
                ]
            ]
            bulma.column [
                column.is6
                labeledField "Name:" (model |> Store.map name) (dispatch << SetName)
                labeledField "Surname:" (model |> Store.map surname) (dispatch << SetSurname)
            ]
        ]

        bulma.field.div [
            field.isGrouped
            button "Create" canCreate Create
            button "Update" canUpdate Update
            button "Delete" canDelete Delete
        ]

        bulma.field.div [
            color.hasTextDanger
            Bind.fragment (model |> Store.map error) Html.text
        ]

    ] |> withStyle appStyle
