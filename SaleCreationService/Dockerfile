FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SaleCreationService/SaleCreationService.csproj", "SaleCreationService/"]
RUN dotnet restore "SaleCreationService/SaleCreationService.csproj"
COPY . .
WORKDIR "/src/SaleCreationService"
RUN dotnet build "SaleCreationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SaleCreationService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SaleCreationService.dll"]