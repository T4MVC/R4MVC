dotnet --info
dotnet restore -v Minimal

dotnet build -c Release

cd test\R4Mvc.Test
dotnet test
