# ReifeManager - Silent Installer
# Installiert ReifeManager vollautomatisch auf C:\Program Files\ReifeManager

Write-Host "=== ReifeManager Silent Install ===" -ForegroundColor Cyan
Write-Host ""

$installerPath = "installer\ReifeManager_Setup_v1.0.0.exe"

if (-not (Test-Path $installerPath)) {
    Write-Host "FEHLER: Installer nicht gefunden: $installerPath" -ForegroundColor Red
    Write-Host "Bitte erst '.\Build-Installer.ps1' ausfuehren!" -ForegroundColor Yellow
    exit 1
}

Write-Host "Starte vollautomatische Installation..." -ForegroundColor Yellow
Write-Host "Installationspfad: C:\Program Files\ReifeManager" -ForegroundColor Gray
Write-Host ""

# Installer im Silent-Modus starten (keine Dialoge)
Start-Process -FilePath $installerPath -ArgumentList "/VERYSILENT","/NORESTART","/TASKS=desktopicon" -Wait

if ($LASTEXITCODE -eq 0) {
    Write-Host "OK Installation erfolgreich!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Installiert in: C:\Program Files\ReifeManager" -ForegroundColor Gray
    Write-Host "Desktop-Icon wurde erstellt." -ForegroundColor Gray
    Write-Host ""
    Write-Host "Moechten Sie ReifeManager jetzt starten? (J/N)" -ForegroundColor Yellow
    $antwort = Read-Host
    if ($antwort -eq "J" -or $antwort -eq "j") {
        Start-Process "C:\Program Files\ReifeManager\ReifeManager_R01.exe"
    }
} else {
    Write-Host "FEHLER bei der Installation!" -ForegroundColor Red
    Write-Host "Bitte manuell als Administrator ausfuehren." -ForegroundColor Yellow
}
