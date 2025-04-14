param (
    [Parameter()]
    [string]$TestCategory = "All"
)

$unitTestProject = "FlashCard.Tests"
$e2eTestProject = "FlashCard.E2ETests"

# Funkcja do uruchamiania testów z określoną kategorią
function Run-Tests {
    param (
        [string]$Project,
        [string]$Category
    )

    Write-Host "Uruchamianie testów dla projektu $Project..." -ForegroundColor Green
    
    if ($Category -eq "All") {
        dotnet test $Project --verbosity normal
    }
    else {
        dotnet test $Project --filter "Category=$Category" --verbosity normal
    }
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Testy zakończone powodzeniem!" -ForegroundColor Green
    }
    else {
        Write-Host "Testy zakończone niepowodzeniem!" -ForegroundColor Red
    }
}

# Uruchamiamy testy w zależności od kategorii
switch ($TestCategory) {
    "Unit" {
        Run-Tests -Project $unitTestProject -Category "All"
        break
    }
    "E2E" {
        Run-Tests -Project $e2eTestProject -Category "All"
        break
    }
    "All" {
        Write-Host "Uruchamianie wszystkich testów..." -ForegroundColor Green
        Run-Tests -Project $unitTestProject -Category "All"
        Run-Tests -Project $e2eTestProject -Category "All"
        break
    }
    default {
        Run-Tests -Project $unitTestProject -Category $TestCategory
        Run-Tests -Project $e2eTestProject -Category $TestCategory
        break
    }
}

Write-Host "Proces testowania zakończony!" -ForegroundColor Cyan 