FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet restore "Backend.csproj" --disable-parallel
RUN dotnet publish "Backend.csproj" -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY --from=build /app .

EXPOSE 5084
EXPOSE 7210

ENV ASPNETCORE_URLS="https://+:7210;http://+:5084" \
    ASPNETCORE_ENVIRONMENT="Development"

ENTRYPOINT ["dotnet", "Backend.dll"]
