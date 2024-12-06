FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
EXPOSE 5084
EXPOSE 7210
WORKDIR /app
COPY . .
RUN dotnet restore "./Backend.csproj" --disable-parallel
RUN dotnet publish "./Backend.csproj" -c release -o /app/publish

# Generowanie certyfikatów
RUN dotnet dev-certs https --trust

ENV ASPNETCORE_URLS="https://+:7210;http://+:5084" \
    ASPNETCORE_ENVIRONMENT="Development"

ENTRYPOINT ["dotnet", "./publish/Backend.dll"]