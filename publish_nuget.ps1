Remove-Item ./packages/Cle.Identity.Extensions -Recurse
dotnet build ./Cle.Identity.Extensions/Cle.Identity.Extensions.csproj --configuration Release -o ./packages/Cle.Identity.Extensions
.\nuget push .\packages\Cle.Identity.Extensions\*.nupkg -Source https://api.nuget.org/v3/index.json