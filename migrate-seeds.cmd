@echo off
cls

cd ./src/DevTools/DbSeeds && dotnet run --project DbSeeds.csproj -- --env=dev