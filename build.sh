#!/bin/sh

dotnet tool update -g dotnet-execute --prerelease
export PATH="$PATH:$HOME/.dotnet/tools"

echo "dotnet-exec ./build/build.cs --args $@"
dotnet-exec ./build/build.cs --args "$@"
