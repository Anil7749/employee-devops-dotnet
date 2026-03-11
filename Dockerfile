# ── Stage 1: Build ──────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# You're already inside employee-devops-dotnet, so copy current dir
COPY . .

RUN dotnet restore employee-webapp.csproj
RUN dotnet publish employee-webapp.csproj -c Release -o /app/publish

# ── Stage 2: Runtime ────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080

ENTRYPOINT ["dotnet", "employee-webapp.dll"]
