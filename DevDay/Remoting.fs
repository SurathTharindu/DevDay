namespace DevDay

open WebSharper

module Server =
    open Microsoft.FSharp.Data.TypeProviders

    type Db = SqlDataConnection<ConnectionString = "Data Source=LOCALHOST\SQLEXPRESS;Initial Catalog=Todo;Integrated Security=True;">

    type TodoItem = {
        Id: int
        TodoItem: string
    }

    [<Rpc>]
    let helloWorld () =
        async {
            return "Hello world"
        }

    [<Rpc>]
    let tagWithDateTime str =
        let str' = sprintf "%s [%A]" str System.DateTime.Now
        async {
            return str'
        }

    [<Rpc>]
    let getTodoItems () =
        let db = Db.GetDataContext ()
        let items =
            query {
                for x in db.Item do
                select x
            }
            |> Array.ofSeq
            |> Array.map (fun x ->
                {Id = x.Id; TodoItem = x.TodoItem}
            )
        async { return items }

    [<Rpc>]
    let addTodoItem s =
        let db = Db.GetDataContext ()
        let i = Db.ServiceTypes.Item (TodoItem = s)
        i |> db.Item.InsertOnSubmit
        db.DataContext.SubmitChanges ()
        async {return i.Id}

    [<Rpc>]
    let rmTodoItem id =
        let db = Db.GetDataContext ()
        db.DataContext.SubmitChanges ()
        query {
            for x in db.Item do
            where (x.Id = id)
            select x
        } |> db.Item.DeleteAllOnSubmit
        db.DataContext.SubmitChanges ()
        async {return ()}
