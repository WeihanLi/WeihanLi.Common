dotnet tool update -g dotnet-execute --prerelease

Write-Host 'dotnet-exec ./build/build.cs "--args=$ARGS"' -ForegroundColor GREEN
 
dotnet-exec ./build/build.cs --args $ARGS
