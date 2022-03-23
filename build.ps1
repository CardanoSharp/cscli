dotnet restore
dotnet build --no-restore
dotnet test --no-build
dotnet publish Src/ConsoleTool/CsCli.ConsoleTool.csproj -c Release -o release --nologo