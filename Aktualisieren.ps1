# ============================================================
#  Reifeschrank Tracker – Automatisches Update-Skript
#  Doppelklick auf diese Datei genügt!
# ============================================================

$Host.UI.RawUI.WindowTitle = "ReifeschrankTracker – Aktualisieren"
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  ReifeschrankTracker wird aktualisiert" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Skript läuft im eigenen Ordner
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir

# ---- Schritt 1: Neuesten Code von GitHub laden ----
Write-Host "[1/3] Lade neuesten Code von GitHub..." -ForegroundColor Yellow
git pull origin main 2>&1 | Out-Null

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "HINWEIS: 'git pull' schlug fehl – versuche alternativen Weg..." -ForegroundColor Yellow

    # Fallback: einzelne Dateien direkt von GitHub herunterladen
    $baseUrl = "https://raw.githubusercontent.com/Acid31-31/ReifeschrankTracker/main"
    $files = @(
        "ReifeschrankTracker/App.xaml",
        "ReifeschrankTracker/Views/MainWindow.xaml",
        "ReifeschrankTracker/Views/NeuChargeDialog.xaml",
        "ReifeschrankTracker/Views/GewichtEintragenDialog.xaml"
    )

    foreach ($file in $files) {
        $url  = "$baseUrl/$file"
        $dest = Join-Path $scriptDir $file
        try {
            Invoke-WebRequest -Uri $url -OutFile $dest -UseBasicParsing -ErrorAction Stop
            Write-Host "  OK: $file" -ForegroundColor Green
        } catch {
            Write-Host "  FEHLER beim Download: $file" -ForegroundColor Red
        }
    }
}

Write-Host "  -> Code aktualisiert!" -ForegroundColor Green
Write-Host ""

# ---- Schritt 2: Projekt bauen ----
Write-Host "[2/3] Baue das Projekt..." -ForegroundColor Yellow
$projPath = Join-Path $scriptDir "ReifeschrankTracker\ReifeschrankTracker.csproj"
dotnet build $projPath --configuration Release --nologo 2>&1 | Tee-Object -Variable buildOutput | Out-Null

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "FEHLER beim Bauen!" -ForegroundColor Red
    $buildOutput | Select-Object -Last 20 | ForEach-Object { Write-Host $_ }
    Write-Host ""
    Write-Host "Drücke eine Taste zum Beenden..." -ForegroundColor Gray
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}

Write-Host "  -> Erfolgreich gebaut!" -ForegroundColor Green
Write-Host ""

# ---- Schritt 3: Programm starten ----
Write-Host "[3/3] Starte das Programm..." -ForegroundColor Yellow
$exePath = Get-ChildItem -Path (Join-Path $scriptDir "ReifeschrankTracker\bin\Release") `
           -Filter "ReifeschrankTracker.exe" -Recurse -ErrorAction SilentlyContinue |
           Select-Object -First 1 -ExpandProperty FullName

if ($exePath) {
    Start-Process $exePath
    Write-Host "  -> Programm wurde gestartet!" -ForegroundColor Green
} else {
    Write-Host "  -> Konnte EXE nicht finden – bitte in Visual Studio starten (F5)." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Fertig! Alles auf dem neuesten Stand." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Drücke eine Taste zum Beenden..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
