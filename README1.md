# Smart Task Manager (F#)

## Description

This project is a web-based task manager built using F# and ASP.NET Core.
It allows users to manage tasks with priorities, categories, filtering, and search.

The goal of this project is to demonstrate functional programming concepts in F# while building a practical application.

---

## Features

* Add new tasks
* Delete tasks
* Mark tasks as done / not done
* Set priority (Low, Medium, High)
* Assign categories (Work, Personal, School)
* Filter tasks (All / Active / Done)
* Search tasks

---

## Technologies Used

* F#
* ASP.NET Core
* .NET SDK

---

## How to Run

1. Install .NET SDK

2. Clone the repository:

   ```bash
   git clone https://github.com/KARAM-BIY/fsharp-task-manager.git
   ```

3. Open the project folder:

   ```bash
   cd fsharp-task-manager
   ```

4. Run the project:

   ```bash
   dotnet run
   ```

5. Open in browser:

   ```
   http://localhost:5193
   ```

---



---

## Project Structure

* Program.fs → main application logic
* Routes → handle add, delete, toggle actions
* HTML rendering → displays the UI

---

## Notes

This project was developed as part of a university course on functional programming in F#.
