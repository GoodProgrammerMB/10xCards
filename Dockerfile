# Dockerfile
# Stage 1: Build
ARG DOTNET_VERSION=8.0
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION}-alpine AS build
WORKDIR /source

# Kopiuj pliki projektu i przywróć zależności
COPY FlashCard.Api/FlashCard.Api.csproj ./FlashCard.Api/
RUN dotnet restore "FlashCard.Api/FlashCard.Api.csproj" --use-current-runtime

# Kopiuj resztę kodu źródłowego
COPY . .

# Publikuj aplikację w trybie Release
RUN dotnet publish "FlashCard.Api/FlashCard.Api.csproj" -c Release -o /app/publish --no-restore --use-current-runtime /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}-alpine AS final
WORKDIR /app

# Zainstaluj pakiety globalizacji
RUN apk add --no-cache icu-libs curl
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV LC_ALL=en_US.UTF-8
ENV LANG=en_US.UTF-8

# Utwórz użytkownika i grupę 'appuser'
RUN addgroup -S appgroup && adduser -S appuser -G appgroup
RUN chown -R appuser:appgroup /app

# Kopiuj opublikowaną aplikację
COPY --chown=appuser:appgroup --from=build /app/publish ./

# Przełącz na użytkownika 'appuser'
USER appuser

# Ustaw zmienne środowiskowe
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

# Zmienne przesłane przez docker-compose
# ENV ConnectionStrings__DefaultConnection=""
# ENV OPENROUTER_API_KEY=""

EXPOSE 8080

# Health Check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/swagger || exit 1

# Punkt wejścia
ENTRYPOINT ["dotnet", "FlashCard.Api.dll"]