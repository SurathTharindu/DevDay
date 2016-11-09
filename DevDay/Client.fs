namespace DevDay

open WebSharper

[<JavaScript>]
module Client =
    open WebSharper.JavaScript
    open WebSharper.UI.Next
    open WebSharper.UI.Next.Client
    open WebSharper.UI.Next.Html

    let private todoList = ListModel.Create (fun (n, _) -> n) []

    let private initTodo () =
        async {
            let! items = Server.getTodoItems ()
            items
            |> Array.iter (fun x ->
                todoList.Add (x.Id, x.TodoItem)
            )
        } |> Async.Start

    let private addTodoItem s =
        async {
            let! id = Server.addTodoItem s
            todoList.Add (id, s)
        } |> Async.Start

    let private rmTodoItem id =
        Server.rmTodoItem id |> Async.Start
        todoList.RemoveByKey id

    let private special (s: string)=
        match s with
        | x when x.StartsWith "NB!" -> "alert alert-danger"
        | x when x.StartsWith "Important:" -> "alert alert-warning"
        | x when x.StartsWith "Note:" -> "alert alert-notice"
        | x when x.StartsWith "Done:" -> "alert alert-success"
        | _ -> "alert"

    let Main () =
        initTodo ()
        let todoText = Var.Create ""
        let tagIt = Submitter.Create todoText.View ""
        let todoTagged = tagIt.View.MapAsync (fun x ->
            match x with
            | "" -> async {return ""}
            | s -> Server.tagWithDateTime s
            )
        let submitTodo = Submitter.Create todoTagged ""
        submitTodo.View |> View.Sink (function
            | "" -> ()
            | x -> addTodoItem x
        )
        div [
            Doc.Input [] todoText
            Doc.Button "Tag" [attr.``class`` "btn"] tagIt.Trigger
            h5Attr [attr.classDyn (todoText.View.Map special)] [textView todoText.View]
            hr []
            divAttr [attr.``class`` "jumbotron"] [
                h3 [text "Preview:"]
                h4 [textView todoTagged]
            ]
            Doc.EmbedView <| tagIt.View.Map (function
                | "" -> Doc.Empty
                | _ -> Doc.Button "Add" [attr.``class`` "btn btn-primary"] submitTodo.Trigger :> Doc
                )
            h2 [text "Todo items"]
            tableAttr [attr.``class`` "table"] [
                tbody [
                    todoList.View
                    |> Doc.BindSeqCached (fun (id, x) ->
                        tr [
                            tdAttr [attr.``class`` <| special x] [text <| string id + " " + x]
                            td [
                                buttonAttr [
                                    attr.``class`` "btn btn-danger"
                                    on.click (fun _ _ -> rmTodoItem id)
                                ] [spanAttr [attr.``class`` "glyphicon glyphicon-minus"] []]
                            ]
                        ]
                    )
                ]
            ]
        ]
