#!/bin/bash
set -e

dotnet ef migrations add init
dotnet ef database update --no-build --project DnDAPI.csproj
exec dotnet DnDAPI.dll
