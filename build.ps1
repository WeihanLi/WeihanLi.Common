dotnet tool update -g dotnet-execute --prerelease

Write-Host 'dotnet-exec ./build/build.cs "--args=$ARGS"' -ForegroundColor GREEN
 
dotnet-exec ./build/build.cs --wide false --reference "project:./src/WeihanLi.Common/WeihanLi.Common.csproj" --using "WeihanLi.Common" --using "WeihanLi.Extensions" --using "WeihanLi.Common.Helpers" --args $ARGS
