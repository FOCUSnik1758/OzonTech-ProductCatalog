
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src


COPY ["src/ProductCatalog.Api/ProductCatalog.Api.csproj", "src/ProductCatalog.Api/"]

RUN dotnet restore "src/ProductCatalog.Api/ProductCatalog.Api.csproj"


COPY . .

WORKDIR "/src/src/ProductCatalog.Api"


RUN dotnet publish "ProductCatalog.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .


ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "ProductCatalog.Api.dll"]