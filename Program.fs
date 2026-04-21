open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Hosting

let builder = WebApplication.CreateBuilder()
let app = builder.Build()

type TaskItem = {
    Id: int
    Text: string
    Done: bool
    Priority: string
    Category: string
}

let mutable tasks : TaskItem list = []
let mutable idCounter = 0

let getColor priority =
    match priority with
    | "High" -> "red"
    | "Medium" -> "orange"
    | _ -> "green"

let filterTasks filter tasks =
    match filter with
    | "done" -> tasks |> List.filter (fun t -> t.Done)
    | "active" -> tasks |> List.filter (fun t -> not t.Done)
    | _ -> tasks

let searchTasks (query:string) tasks =
    if String.IsNullOrWhiteSpace(query) then tasks
    else
        tasks
        |> List.filter (fun t ->
            t.Text.ToLower().Contains(query.ToLower()) ||
            t.Category.ToLower().Contains(query.ToLower())
        )

let htmlPage filter search =
    let filtered =
        tasks
        |> filterTasks filter
        |> searchTasks search

    let taskItems =
        filtered
        |> List.map (fun t ->
            let status = if t.Done then "✔" else "❌"
            let color = getColor t.Priority

            sprintf "
            <li style='border:1px solid #ccc; padding:10px; margin:10px; color:%s'>
                <b>%s</b><br/>
                Priority: %s | Category: %s | Status: %s<br/>
                <a href='/toggle/%d'>Toggle</a>
                <a href='/delete/%d'>Delete</a>
            </li>
            " color t.Text t.Priority t.Category status t.Id t.Id
        )
        |> String.concat ""

    $"
    <html>
    <head>
        <title>Smart Task Manager</title>
    </head>

    <body style='font-family:Arial; text-align:center;'>

        <h1>Smart Task Manager (F#)</h1>

        <form method='post' action='/add'>
            <input name='task' placeholder='Task' required />

            <select name='priority'>
                <option>Low</option>
                <option>Medium</option>
                <option>High</option>
            </select>

            <select name='category'>
                <option>Work</option>
                <option>Personal</option>
                <option>School</option>
            </select>

            <button>Add</button>
        </form>

        <form method='get'>
            <input name='search' placeholder='Search...' value='{search}' />
            <button>Search</button>
        </form>

        <div>
            <a href='/?filter=all'>All</a> |
            <a href='/?filter=active'>Active</a> |
            <a href='/?filter=done'>Done</a>
        </div>

        <ul style='list-style:none; padding:0;'>
            {taskItems}
        </ul>

    </body>
    </html>
    "

app.MapGet("/", Func<HttpContext, IResult>(fun ctx ->
    let filter =
        if ctx.Request.Query.ContainsKey("filter")
        then ctx.Request.Query["filter"].ToString()
        else "all"

    let search =
        if ctx.Request.Query.ContainsKey("search")
        then ctx.Request.Query["search"].ToString()
        else ""

    Results.Content(htmlPage filter search, "text/html")
)) |> ignore

app.MapPost("/add", Func<HttpContext, Threading.Tasks.Task<IResult>>(fun ctx ->
    task {
        let! form = ctx.Request.ReadFormAsync()

        let text = form["task"].ToString()
        let priority = form["priority"].ToString()
        let category = form["category"].ToString()

        if text <> "" then
            idCounter <- idCounter + 1

            let newTask = {
                Id = idCounter
                Text = text
                Done = false
                Priority = priority
                Category = category
            }

            tasks <- newTask :: tasks

        return Results.Redirect("/")
    }
)) |> ignore

app.MapGet("/delete/{id}", Func<int, IResult>(fun id ->
    tasks <- tasks |> List.filter (fun t -> t.Id <> id)
    Results.Redirect("/")
)) |> ignore

app.MapGet("/toggle/{id}", Func<int, IResult>(fun id ->
    tasks <-
        tasks
        |> List.map (fun t ->
            if t.Id = id then { t with Done = not t.Done }
            else t
        )

    Results.Redirect("/")
)) |> ignore

app.Run()