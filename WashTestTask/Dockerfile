FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["WashTestTask/WashTestTask.csproj", "WashTestTask/"]
RUN dotnet restore "WashTestTask/WashTestTask.csproj"
COPY . .
WORKDIR "/src/WashTestTask"
RUN dotnet build "WashTestTask.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WashTestTask.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WashTestTask.dll"]