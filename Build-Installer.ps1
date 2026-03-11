param(
    [string]$Version = ""
)

# ReifeManager - Installer Builder
Write-Host "=== ReifeManager Installer Builder ===" -ForegroundColor Cyan
Write-Host ""

if ([string]::IsNullOrWhiteSpace($Version)) {
    $projectFile = "ReifeManager_R01/ReifeManager_R01.csproj"
    [xml]$proj = Get-Content $projectFile
    $assemblyVersion = $proj.Project.PropertyGroup.AssemblyVersion | Select-Object -First 1

    if ([string]::IsNullOrWhiteSpace($assemblyVersion)) {
        $Version = "1.0.0"
    }
    else {
        $v = [Version]$assemblyVersion
        $Version = if ($v.Build -eq 0) { "$($v.Major).$($v.Minor).$($v.Revision)" } else { "$($v.Major).$($v.Minor).$($v.Build)" }
    }
}

Write-Host "Version: $Version" -ForegroundColor Cyan
Write-Host ""

Write-Host "[ 1/3 ] Publishing Projekt..." -ForegroundColor Yellow
dotnet publish ReifeManager_R01/ReifeManager_R01.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish/ReifeManager

if ($LASTEXITCODE -ne 0) {
    Write-Host "FEHLER beim Publish!" -ForegroundColor Red
    exit 1
}

Write-Host "OK Publish erfolgreich!" -ForegroundColor Green
Write-Host ""

# 2. Portable ZIP erstellen
Write-Host "[ 2/3 ] Erstelle Portable ZIP..." -ForegroundColor Yellow
New-Item -ItemType Directory -Path installer -Force | Out-Null

$zipName = "installer/ReifeManager_Portable_v$Version.zip"
$setupName = "ReifeManager_Setup_v$Version"

Compress-Archive -Path publish/ReifeManager/* -DestinationPath $zipName -Force
Write-Host "OK Portable ZIP erstellt: $zipName" -ForegroundColor Green
Write-Host ""

# 3. Inno Setup Installer erstellen
Write-Host "[ 3/3 ] Erstelle Installer..." -ForegroundColor Yellow
$innoPath = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"

if (Test-Path $innoPath) {
    & $innoPath "/DMyAppVersion=$Version" "/DMyOutputBaseFilename=$setupName" ReifeManager_Setup.iss

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
Write-Host "  - Portable EXE:    publish/ReifeManager/ReifeManager_R01.exe" -ForegroundColor Gray
Write-Host "  - Portable ZIP:    $zipName" -ForegroundColor Green
Write-Host "  - Setup Installer: installer/$setupName.exe" -ForegroundColor Green
Write-Host ""
