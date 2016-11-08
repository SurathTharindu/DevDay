namespace DevDay

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI.Next
open WebSharper.UI.Next.Server

type EndPoint =
    | [<EndPoint "/">] Home
    | [<EndPoint "/about">] About

module Templating =
    type MainTemplate = Templating.Template<"Main.html">

    let Main title body =
        Content.Page(
            MainTemplate.Doc(
                title = title,
                body = body
            )
        )

module Site =
    open WebSharper.UI.Next.Html

    let HomePage _ =
        Templating.Main "Home" [
            h1 [text "TODO"]
            div [client <@ Client.Main() @>]
        ]

    let AboutPage _ =
        Templating.Main "About" [
            h1 [text "About"]
            p [text "This is a template WebSharper client-server application."]
        ]

    [<Website>]
    let Main =
        Application.MultiPage (fun ctx endpoint ->
            match endpoint with
            | EndPoint.Home -> HomePage ctx
            | EndPoint.About -> AboutPage ctx
        )
