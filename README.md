## R4MVC

[![Build status](https://ci.appveyor.com/api/projects/status/sw4jwy7gtedra5bv?svg=true)](https://ci.appveyor.com/project/T4MVC/r4mvc)
[![R4Mvc on NuGet](https://img.shields.io/nuget/v/R4Mvc.svg)](https://www.nuget.org/packages/R4Mvc)  
CI build: [![R4Mvc on MyGet](https://img.shields.io/myget/r4mvc/vpre/R4Mvc.svg)](https://github.com/T4MVC/R4MVC/wiki/CI-Builds)

R4MVC is a Roslyn code generator for ASP.NET MVC Core apps that creates strongly typed helpers that eliminate the use of literal strings in many places.  

It is a re-implementation of [T4MVC](https://github.com/T4MVC/T4MVC) for ASP.NET Core projects.

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

## Usage

R4MVC runs in Visual Studio 2017, and supports ASP.NET Core MVC 1 (v2 support in progress)

To use it, simply install the [R4Mvc](https://www.nuget.org/packages/r4mvc) and [R4Mvc.Tools](https://www.nuget.org/packages/r4mvc.tools) package using NuGet. It will add the necessary tooling to your project, which will be invoked on install. You will instantly see a number of files get created in your project.

Unlike T4MVC, R4MVC isn't based on a T4 template, so triggering the generation code is slightly different. Currently, you would need to open the Package Manager Console (Tools Menu -> NuGet Package Manager -> Package Manager Console) and running the following PowerShell command:

```powershell
> Generate-R4MVC
```

### Use the following links to get started

*   **Install** R4MVC in your MVC app using [NuGet](http://nuget.org)
*   **Learn**: visit the [Documentation page](https://github.com/T4MVC/R4MVC/wiki/Documentation)
*   **Discuss**: Discuss it on [GitHub](https://github.com/T4MVC/R4MVC/issues)
*   **Contribute**
*   **History &amp; release notes**: [change history](CHANGELOG.md)
