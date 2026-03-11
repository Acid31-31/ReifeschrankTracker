#!/usr/bin/env pwsh
# Auto-Release Script für ReifeManager - vollständiger Workflow

param(
    [string]$Version = "1.0.33",
    [string]$Message = "Auto-Release $(Get-Date -Format 'yyyyMMdd_HHmmss')"
)

Write-Host "🚀 ========== ReifeManager Auto-Release ==========" -ForegroundColor Green
Write-Host "Version: $Version" -ForegroundColor Cyan
Write-Host "Zeit: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor Cyan

# ========== 1. BACKUP ==========
Write-Host "`n📦 [1/6] Backup wird erstellt..." -ForegroundColor Yellow
$ts = Get-Date -Format yyyyMMdd_HHmmss
$backupPath = "Backups/backup_$ts.zip"
try {
    Compress-Archive -Path "ReifeManager_R01" -DestinationPath $backupPath -Force
    Write-Host "✅ Backup erstellt: $backupPath" -ForegroundColor Green
} catch {
    Write-Host "❌ Backup fehlgeschlagen: $_" -ForegroundColor Red
    exit 1
}

# ========== 2. BUILD ==========
Write-Host "`n🔨 [2/6] Release-Build wird durchgeführt..." -ForegroundColor Yellow
try {
    Stop-Process -Name "ReifeManager_R01" -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 1
    $buildOutput = dotnet build "ReifeManager_R01/ReifeManager_R01.csproj" -c Release 2>&1
    if ($buildOutput -match "erfolgreich") {
        Write-Host "✅ Build erfolgreich" -ForegroundColor Green
    } else {
        Write-Host "❌ Build fehlgeschlagen" -ForegroundColor Red
        Write-Host $buildOutput
        exit 1
    }
} catch {
    Write-Host "❌ Build-Fehler: $_" -ForegroundColor Red
    exit 1
}

# ========== 3. INSTALLER ==========
Write-Host "`n📦 [3/6] Installer wird gepackt..." -ForegroundColor Yellow
try {
    & ".\Build-Installer.ps1" -Version $Version | Out-Null
    Write-Host "✅ Installer erstellt" -ForegroundColor Green
} catch {
    Write-Host "❌ Installer-Build fehlgeschlagen: $_" -ForegroundColor Red
    exit 1
}

# ========== 4. GIT COMMIT ==========
Write-Host "`n📝 [4/6] Git Commit und Push..." -ForegroundColor Yellow
try {
    git add -A
    git commit -m "v${Version}: Auto-Release - $Message" -q
    git push origin main -q
    Write-Host "✅ Code zu GitHub gepusht" -ForegroundColor Green
} catch {
    Write-Host "⚠️  Git Commit fehlgeschlagen (ignoriert): $_" -ForegroundColor Yellow
}

# ========== 5. GIT TAG ==========
Write-Host "`n🏷️  [5/6] Git Tag erstellen..." -ForegroundColor Yellow
try {
    $tagMessage = "ReifeManager v$Version - Auto-Release $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
    git tag -a "v$Version" -m "$tagMessage" -f
    git push origin "v$Version" -f -q
    Write-Host "✅ Release-Tag v$Version erstellt und gepusht" -ForegroundColor Green
} catch {
    Write-Host "⚠️  Tag-Fehler (ignoriert): $_" -ForegroundColor Yellow
}

# ========== 6. GITHUB RELEASE (Token-basiert) ==========
Write-Host "`n📤 [6/6] GitHub Release mit Assets..." -ForegroundColor Yellow

if ([string]::IsNullOrEmpty($env:GH_TOKEN)) {
    Write-Host "GH_TOKEN nicht gesetzt - Release wird uebersprungen" -ForegroundColor Yellow
    Write-Host "" -ForegroundColor Cyan
    Write-Host "Einmalige Einrichtung in 2 Min" -ForegroundColor Cyan
    Write-Host "1. Token: https://github.com/settings/tokens/new?scopes=repo" -ForegroundColor Cyan
    Write-Host "2. Diesen Befehl ausfuehren:" -ForegroundColor Cyan
    Write-Host "   [Environment]::SetEnvironmentVariable('GH_TOKEN', 'ghp_xxx', 'User')" -ForegroundColor Green
    Write-Host "3. Terminal neu starten" -ForegroundColor Cyan
} else {
    try {
        & ".\Quick-Release.ps1" -Version $Version
    } catch {
        Write-Host "Release-Upload fehlgeschlagen: $_" -ForegroundColor Yellow
        Write-Host "Manuell: https://github.com/Acid31-31/ReifeschrankTracker/releases/new?tag=v$Version" -ForegroundColor Cyan
    }
}

# ========== SUMMARY ==========
Write-Host "`n✅ ========== Auto-Release FERTIG ==========" -ForegroundColor Green
Write-Host "📍 Version: v$Version" -ForegroundColor Cyan
Write-Host "📦 Backup: $backupPath" -ForegroundColor Cyan
Write-Host "🔗 GitHub: https://github.com/Acid31-31/ReifeschrankTracker/releases/tag/v$Version" -ForegroundColor Cyan
Write-Host "`n⏰ Fertiggestellt: $(Get-Date -Format 'HH:mm:ss')" -ForegroundColor Green
