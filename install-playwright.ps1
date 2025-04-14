Write-Host "Instalowanie przeglądarek dla Playwright..." -ForegroundColor Green

# Upewniamy się, że mamy zaktualizowane pakiety
dotnet restore

# Instalujemy przeglądarki Playwright
dotnet tool install --global Microsoft.Playwright.CLI
playwright install

Write-Host "Przeglądarki dla Playwright zostały pomyślnie zainstalowane!" -ForegroundColor Green 