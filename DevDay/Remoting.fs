namespace DevDay

open WebSharper

module Server =

    [<Rpc>]
    let helloWorld () =
        async {
            return "Hello world"
        }

