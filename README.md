## R4MVC
R4MVC is a Roslyn code generator for ASP.NET MVC vnext apps that creates strongly typed helpers that eliminate the use of literal strings in many places.  

It is a re-implementation of [T4MVC](https://github.com/T4MVC/T4MVC) for ASP.NET vnext projects, as T4 templates are [not supported](https://github.com/aspnet/Home/issues/272).

## Usage

Instead of

````c#
@Html.ActionLink("Delete Dinner", "Delete", "Dinners", new { id = Model.DinnerID }, null)
````

R4MVC lets you write

````c#
@Html.ActionLink("Delete Dinner", MVC.Dinners.Delete(Model.DinnerID))
````

and that's just the beginning!

## Continuous Integration

[![Build status](https://ci.appveyor.com/api/projects/status/sw4jwy7gtedra5bv?svg=true)](https://ci.appveyor.com/project/T4MVC/r4mvc)
