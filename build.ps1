dotnet clean
dotnet restore
dotnet build --no-restore -c Release
dotnet test --no-build -c Release
dotnet pack --no-build Src/ConsoleTool/CsCli.ConsoleTool.csproj -o nupkg -c Release

# Generate cross-platform executables
#dotnet publish --no-build Src/ConsoleTool/CsCli.ConsoleTool.csproj -c Release -o release --nologo
#dotnet publish -r win-x64 Src/ConsoleTool/CsCli.ConsoleTool.csproj -c Release -o release -p:PublishSingleFile=true -p:PublishTrimmed=true -p:AssemblyName=cscli --self-contained true
#dotnet publish -r linux-x64 Src/ConsoleTool/CsCli.ConsoleTool.csproj -c Release -o release -p:PublishSingleFile=true --self-contained true
#dotnet publish -r osx-x64 Src/ConsoleTool/CsCli.ConsoleTool.csproj -c Release -o release -p:PublishSingleFile=true --self-contained true
#dotnet publish -r osx.12-arm64 Src/ConsoleTool/CsCli.ConsoleTool.csproj -c Release -o release -p:PublishSingleFile=true --self-contained true

# Another way (per target)
#$targets = @("win-x64", "linux-x64", "osx-x64")
#foreach ($target in $targets) {
#    $tag=$(git describe --tags --abbrev=0)
#    $release_name="cscli-$tag-$target"
#    dotnet publish Src/ConsoleTool/CsCli.ConsoleTool.csproj -r $target -c Release -o $release_name "-p:PublishSingleFile=true" "-p:AssemblyName=cscli.$target" --self-contained true
#}