#!/usr/bin/env pwsh
# Quick Release Upload - ohne interaktiven Login

param(
    [string]$Version = "1.0.33",
    [string]$Token = $env:GH_TOKEN
)

if ([string]::IsNullOrEmpty($Token)) {
    Write-Host "❌ GH_TOKEN Environment-Variable nicht gesetzt!" -ForegroundColor Red
    Write-Host "`n📌 SCHNELL FIX (5 Sekunden):" -ForegroundColor Yellow
    Write-Host "   1. https://github.com/settings/tokens/new" -ForegroundColor Cyan
    Write-Host "      - Scopes: repo (alle), read:packages" -ForegroundColor Cyan
    Write-Host "   2. Token kopieren (z.B. ghp_xxxx...)" -ForegroundColor Cyan
    Write-Host "   3. Diesen Command ausführen:" -ForegroundColor Green
    Write-Host @"
`n[Environment]::SetEnvironmentVariable('GH_TOKEN', 'ghp_dein_token_hier', 'User')
`$env:GH_TOKEN = 'ghp_dein_token_hier'
"@ -ForegroundColor Green
    Write-Host "   4. Dieses Skript erneut aufrufen" -ForegroundColor Cyan
    exit 1
}

Write-Host "🚀 GitHub Release Upload startet..." -ForegroundColor Green
Write-Host "Version: v$Version`n" -ForegroundColor Cyan

# Release erstellen mit Token
$releaseBody = @"
## v$Version Release

### Downloads
- **Setup Installer:** ReifeManager_Setup.exe
- **Portable ZIP:** ReifeManager_Portable.zip

### Features
✨ Auto-Deployment ready
🔍 UpdateCheckWindow doppelt größer
🚀 Automatische Update-Prüfung

Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
"@

$headers = @{
    "Authorization" = "token $Token"
    "Accept" = "application/vnd.github.v3+json"
    "X-GitHub-Media-Type" = "github.v3"
}

# Release existiert bereits?
try {
    $existingRelease = Invoke-RestMethod -Uri "https://api.github.com/repos/Acid31-31/ReifeschrankTracker/releases/tags/v$Version" `
        -Headers $headers -ErrorAction SilentlyContinue
    
    if ($existingRelease) {
        Write-Host "⚠️  Release v$Version existiert bereits - wird aktualisiert..." -ForegroundColor Yellow
        $releaseId = $existingRelease.id
        
        # Release aktualisieren
        Invoke-RestMethod -Uri "https://api.github.com/repos/Acid31-31/ReifeschrankTracker/releases/$releaseId" `
            -Method PATCH `
            -Headers $headers `
            -Body (@{ body = $releaseBody } | ConvertTo-Json) `
            -ContentType "application/json" | Out-Null
    }
} catch {
    # Release existiert nicht, wird neu erstellt
    $createBody = @{
        tag_name = "v$Version"
        name = "v$Version Release"
        body = $releaseBody
        draft = $false
        prerelease = $false
    }
    
    $release = Invoke-RestMethod -Uri "https://api.github.com/repos/Acid31-31/ReifeschrankTracker/releases" `
        -Method POST `
        -Headers $headers `
        -Body ($createBody | ConvertTo-Json) `
        -ContentType "application/json"
    
    $releaseId = $release.id
    Write-Host "✅ Release erstellt: v$Version`n" -ForegroundColor Green
}

# Assets hochladen
$files = @(
    @{ path = "installer/ReifeManager_Setup.exe"; name = "ReifeManager_Setup.exe" },
    @{ path = "installer/ReifeManager_Portable.zip"; name = "ReifeManager_Portable.zip" }
)

foreach ($file in $files) {
    if (Test-Path $file.path) {
        Write-Host "📤 Uploading: $($file.name)..." -ForegroundColor Cyan
        
        $fileBytes = [System.IO.File]::ReadAllBytes($file.path)
        $fileBase64 = [System.Convert]::ToBase64String($fileBytes)
        
        # Asset löschen wenn existiert
        try {
            $assets = Invoke-RestMethod -Uri "https://api.github.com/repos/Acid31-31/ReifeschrankTracker/releases/$releaseId/assets" `
                -Headers $headers -ErrorAction SilentlyContinue
            
            $existingAsset = $assets | Where-Object { $_.name -eq $file.name }
            if ($existingAsset) {
                Invoke-RestMethod -Uri "https://api.github.com/repos/Acid31-31/ReifeschrankTracker/releases/assets/$($existingAsset.id)" `
                    -Method DELETE `
                    -Headers $headers | Out-Null
                Write-Host "  └─ Altes Asset gelöscht" -ForegroundColor Gray
            }
        } catch {}
        
        # Asset hochladen
        $uploadUri = "https://uploads.github.com/repos/Acid31-31/ReifeschrankTracker/releases/$releaseId/assets?name=$($file.name)"
        $uploadHeaders = $headers.Clone()
        $uploadHeaders["Content-Type"] = "application/octet-stream"
        
        Invoke-RestMethod -Uri $uploadUri `
            -Method POST `
            -Headers $uploadHeaders `
            -InFile $file.path | Out-Null
        
        Write-Host "✅ $($file.name) hochgeladen" -ForegroundColor Green
    } else {
        Write-Host "❌ Datei nicht gefunden: $($file.path)" -ForegroundColor Red
    }
}

Write-Host "`n✅ Release fertig!`n" -ForegroundColor Green
Write-Host "🔗 https://github.com/Acid31-31/ReifeschrankTracker/releases/tag/v$Version" -ForegroundColor Cyan
