namespace DevDay

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html

[<JavaScript>]
module Client =

    let Main () =
        let msg = Var.Create ""
        async {
            let! x = Server.helloWorld ()
            msg.Value <- x
        } |> Async.Start
        div [
            h1 [text "To be written"]
            Doc.TextView msg.View
        ]
