# ============================================================
#  Reifeschrank Tracker – Automatisches Update-Skript
#  Doppelklick auf diese Datei genügt!
# ============================================================

# Fallback-Branch, solange der PR noch nicht in main gemergt wurde.
# Nach dem Merge kann dieser Wert auf "main" geändert oder entfernt werden.
$fallbackBranch = "copilot/implement-reifeschrank-tracker"

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

$pullResult = git pull origin main 2>&1
$pullOutput = $pullResult | Out-String

if ($LASTEXITCODE -ne 0) {
    Write-Host "  HINWEIS: 'git pull origin main' schlug fehl." -ForegroundColor Yellow
    Write-Host $pullOutput -ForegroundColor Gray
    Write-Host "  Versuche Fallback-Branch..." -ForegroundColor Yellow
    $pullResult2 = git pull origin $fallbackBranch 2>&1
    $pullOutput2 = $pullResult2 | Out-String
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  FEHLER: Konnte Code nicht von GitHub laden." -ForegroundColor Red
        Write-Host $pullOutput2 -ForegroundColor Gray
        Write-Host ""
        Write-Host "Drücke eine Taste zum Beenden..." -ForegroundColor Gray
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
        exit 1
    }
}

# Prüfen ob das neue Projekt-Verzeichnis vorhanden ist; falls nicht, PR-Branch direkt auschecken
$projPath = Join-Path $scriptDir "ReifeschrankTracker\ReifeschrankTracker.csproj"
if (-not (Test-Path $projPath)) {
    Write-Host ""
    Write-Host "  Neue Projektdateien noch nicht in main – lade direkt vom Update-Branch..." -ForegroundColor Yellow
    $fetchResult = git fetch origin $fallbackBranch 2>&1
    $checkoutResult = git checkout FETCH_HEAD -- ReifeschrankTracker ReifeschrankTracker.sln 2>&1
    if (-not (Test-Path $projPath)) {
        Write-Host "  FEHLER: Projektdateien konnten nicht geladen werden." -ForegroundColor Red
        Write-Host ($fetchResult | Out-String) -ForegroundColor Gray
        Write-Host ($checkoutResult | Out-String) -ForegroundColor Gray
        Write-Host ""
        Write-Host "Drücke eine Taste zum Beenden..." -ForegroundColor Gray
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
        exit 1
    }
    Write-Host "  -> Projektdateien geladen!" -ForegroundColor Green
}

Write-Host "  -> Code aktualisiert!" -ForegroundColor Green
Write-Host ""

# ---- Schritt 2: Projekt bauen ----
Write-Host "[2/3] Baue das Projekt..." -ForegroundColor Yellow
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
