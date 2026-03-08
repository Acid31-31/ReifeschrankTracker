# ReifeManager - Installer Builder
# Dieses Script erstellt automatisch einen Setup-Installer

Write-Host "=== ReifeManager Installer Builder ===" -ForegroundColor Cyan
Write-Host ""

# Version aus csproj lesen
[xml]$csproj = Get-Content "ReifeManager_R01/ReifeManager_R01.csproj"
$version = $csproj.Project.PropertyGroup.Version
if ([string]::IsNullOrWhiteSpace($version)) {
    $version = "1.0.1"
}

# 1. Projekt publishen
Write-Host "[ 1/3 ] Publishing Projekt... (v$version)" -ForegroundColor Yellow
dotnet publish ReifeManager_R01/ReifeManager_R01.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish/ReifeManager

if ($LASTEXITCODE -ne 0) {
    Write-Host "FEHLER beim Publish!" -ForegroundColor Red
    exit 1
}

Write-Host "OK Publish erfolgreich!" -ForegroundColor Green
Write-Host ""

# 2. Portable ZIP erstellen
Write-Host "[ 2/3 ] Erstelle Portable ZIP..." -ForegroundColor Yellow
$zipName = "installer/ReifeManager_Portable_v$version.zip"
New-Item -ItemType Directory -Path installer -Force | Out-Null
Compress-Archive -Path publish/ReifeManager/* -DestinationPath $zipName -Force
Write-Host "OK Portable ZIP erstellt: $zipName" -ForegroundColor Green
Write-Host ""

# 3. Inno Setup Installer erstellen (falls installiert)
Write-Host "[ 3/3 ] Erstelle Installer..." -ForegroundColor Yellow
$innoPath = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
$setupName = "ReifeManager_Setup_v$version"

if (Test-Path $innoPath) {
    & $innoPath "/DMyAppVersion=$version" "/DMyOutputBaseFilename=$setupName" ReifeManager_Setup.iss
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "OK Installer erstellt: installer/$setupName.exe" -ForegroundColor Green
    } else {
        Write-Host "WARNUNG Fehler beim Erstellen des Installers" -ForegroundColor Red
    }
} else {
    Write-Host "WARNUNG Inno Setup nicht gefunden!" -ForegroundColor Yellow
    Write-Host "  Download: https://jrsoftware.org/isdl.php" -ForegroundColor Gray
    Write-Host "  Portable ZIP wurde trotzdem erstellt." -ForegroundColor Gray
}

Write-Host ""
Write-Host "=== Fertig! ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "Dateien:" -ForegroundColor White
Write-Host "  - Portable EXE:   publish/ReifeManager/ReifeManager_R01.exe" -ForegroundColor Gray
Write-Host "  - Portable ZIP:   $zipName" -ForegroundColor Gray
if (Test-Path "installer/$setupName.exe") {
    Write-Host "  - Setup Installer: installer/$setupName.exe" -ForegroundColor Gray
}
Write-Host ""
