# ─── Stage 1: Base runtime ──────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS base

# Non-root user — Trivy flags root containers as HIGH vulnerability
RUN addgroup --system appgroup \
    && adduser --system --ingroup appgroup appuser

WORKDIR /app
EXPOSE 8080

# ─── Stage 2: Build ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy ONLY csproj first → Docker layer cache optimization
# If no dependency changes → restore layer is cached → faster CI builds
COPY ["employee-webapp.csproj", "."]
RUN dotnet restore "employee-webapp.csproj"

# Now copy everything else and build
COPY . .
RUN dotnet build "employee-webapp.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build

# ─── Stage 3: Publish ───────────────────────────────────────────────────────
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "employee-webapp.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# ─── Stage 4: Final ─────────────────────────────────────────────────────────
# Only runtime here — SDK never ships to production
# Result: ~200MB instead of ~800MB
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Non-root user (security + Trivy compliance)
USER appuser

# Exec form → proper signal handling → graceful ECS shutdown
ENTRYPOINT ["dotnet", "employee-webapp.dll"]
```

---
