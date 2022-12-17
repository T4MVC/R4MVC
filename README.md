## R4MVC

[![Build status](https://ci.appveyor.com/api/projects/status/sw4jwy7gtedra5bv?svg=true)](https://ci.appveyor.com/project/T4MVC/r4mvc)
[![R4Mvc on NuGet](https://img.shields.io/nuget/v/R4Mvc.svg)](https://www.nuget.org/profiles/R4MVC)  
CI build: [![R4Mvc on MyGet](https://img.shields.io/myget/r4mvc/vpre/R4Mvc.svg)](https://github.com/T4MVC/R4MVC/wiki/CI-Builds)

R4MVC is a Roslyn based code generator for ASP.NET MVC Core apps that creates strongly typed helpers that eliminate the use of literal strings in many places.  

It is a re-implementation of [T4MVC](https://github.com/T4MVC/T4MVC) for ASP.NET Core projects.

R4MVC runs in the dotnet cli or in Visual Studio 2022, and supports ASP.NET Core 6 and 7

## Benefits

Instead of

````c#
@Html.ActionLink("Dinner Details", "Details", "Dinners", new { id = Model.DinnerID }, null)
````
R4MVC lets you write
````c#
@Html.ActionLink("Dinner Details", MVC.Dinners.Details(Model.DinnerID))
````

When you're using tag helpers, instead of
```html
<a asp-action="Details" asp-controller="Dinners" asp-route-id="@Model.DinnerID">Dinner Details</a>
```
you can write (after registering R4Mvc tag helpers in _ViewImports.cshtml with the directive: `@addTagHelper *, R4Mvc`)
```html
<a mvc-action="MVC.Dinners.Details(Model.DinnerID)">Dinner Details</a>
```

and that's just the beginning!

### Use the following links to get started

*   **Install** R4MVC is distributed using using [NuGet](http://nuget.org). Visit the [Installation page](https://github.com/T4MVC/R4MVC/wiki/Installation)
*   **Learn**: visit the [Documentation page](https://github.com/T4MVC/R4MVC/wiki/Documentation)
*   **Discuss**: Discuss it on [GitHub](https://github.com/T4MVC/R4MVC/issues)
*   **Contribute**
*   **History &amp; release notes**: [change history](CHANGELOG.md)
