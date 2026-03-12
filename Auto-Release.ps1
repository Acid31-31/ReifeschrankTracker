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

# ========== 2. VERSION SYNCHRONISIEREN ==========
Write-Host "`n🧾 [2/7] Projektversion wird gesetzt..." -ForegroundColor Yellow
try {
    $projectPath = "ReifeManager_R01/ReifeManager_R01.csproj"
    [xml]$proj = Get-Content $projectPath

    $parts = $Version.Split('.')
    if ($parts.Count -lt 3) {
        throw "Version muss mindestens im Format X.Y.Z angegeben werden."
    }

    $major = [int]$parts[0]
    $minor = [int]$parts[1]
    $patch = [int]$parts[2]

    $projectContent = Get-Content $projectPath -Raw
    $projectContent = [regex]::Replace($projectContent, '<Version>.*?</Version>', "<Version>$major.$minor.$patch</Version>")
    $projectContent = [regex]::Replace($projectContent, '<AssemblyVersion>.*?</AssemblyVersion>', "<AssemblyVersion>$major.$minor.0.$patch</AssemblyVersion>")
    $projectContent = [regex]::Replace($projectContent, '<FileVersion>.*?</FileVersion>', "<FileVersion>$major.$minor.0.$patch</FileVersion>")
    $projectContent = [regex]::Replace($projectContent, '<InformationalVersion>.*?</InformationalVersion>', "<InformationalVersion>$major.$minor.$patch</InformationalVersion>")
    Set-Content -Path $projectPath -Value $projectContent -Encoding UTF8

    Write-Host ("✅ Versionen in csproj gesetzt: {0}.{1}.{2}" -f $major, $minor, $patch) -ForegroundColor Green
} catch {
    Write-Host "❌ Versions-Synchronisierung fehlgeschlagen: $_" -ForegroundColor Red
    exit 1
}

# ========== 3. BUILD ==========
Write-Host "`n🔨 [3/7] Release-Build wird durchgeführt..." -ForegroundColor Yellow
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

# ========== 4. INSTALLER ==========
Write-Host "`n📦 [4/7] Installer wird gepackt..." -ForegroundColor Yellow
try {
    & ".\Build-Installer.ps1" -Version $Version | Out-Null
    Write-Host "✅ Installer erstellt" -ForegroundColor Green
} catch {
    Write-Host "❌ Installer-Build fehlgeschlagen: $_" -ForegroundColor Red
    exit 1
}

# ========== 5. GIT COMMIT ==========
Write-Host "`n📝 [5/7] Git Commit und Push..." -ForegroundColor Yellow
try {
    git add -A
    git commit -m "v${Version}: Auto-Release - $Message" -q
    git push origin main -q
    Write-Host "✅ Code zu GitHub gepusht" -ForegroundColor Green
} catch {
    Write-Host "⚠️  Git Commit fehlgeschlagen (ignoriert): $_" -ForegroundColor Yellow
}

# ========== 6. GIT TAG ==========
Write-Host "`n🏷️  [6/7] Git Tag erstellen..." -ForegroundColor Yellow
try {
    $tagMessage = "ReifeManager v$Version - Auto-Release $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
    git tag -a "v$Version" -m "$tagMessage" -f
    git push origin "v$Version" -f -q
    Write-Host "✅ Release-Tag v$Version erstellt und gepusht" -ForegroundColor Green
} catch {
    Write-Host "⚠️  Tag-Fehler (ignoriert): $_" -ForegroundColor Yellow
}

# ========== 7. GITHUB RELEASE (Token-basiert) ==========
Write-Host "`n📤 [7/7] GitHub Release mit Assets..." -ForegroundColor Yellow

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
