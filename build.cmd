dotnet --info
dotnet restore -v Minimal

dotnet build "src\R4Mvc"
dotnet build "src\R4Mvc.Tools"
dotnet build "src\R4Mvc.Test"

cd test\R4Mvc.Test
dotnet test

cd ..\..
dotnet pack "src\R4Mvc"
dotnet pack "src\R4Mvc.Tools"
