dotnet --info
dotnet restore -v Minimal

dotnet build "src\R4Mvc"
dotnet build "src\R4MvcHostApp"
dotnet build "src\R4Mvc.Test"

cd test\R4Mvc.Test
dotnet test
