dotnet --info
dotnet build -c Release  ./src/WeihanLi.Common/WeihanLi.Common.csproj
dotnet build -c Release  ./src/WeihanLi.Common.Logging.Log4Net/WeihanLi.Common.Logging.Log4Net.csproj
dotnet build -c Release  ./src/WeihanLi.Data/WeihanLi.Data.csproj
pause