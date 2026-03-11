#!/usr/bin/env pwsh
# Rename v1.0.33 zu v1.0.0

Write-Host "🔄 Benenne Installer um..." -ForegroundColor Cyan

$old_setup = "installer/ReifeManager_Setup_v1.0.33.exe"
$old_zip = "installer/ReifeManager_Portable_v1.0.33.zip"

$new_setup = "installer/ReifeManager_Setup_v1.0.0.exe"
$new_zip = "installer/ReifeManager_Portable_v1.0.0.zip"

if (Test-Path $old_setup) {
    Rename-Item $old_setup $new_setup -Force
    Write-Host "✅ $old_setup → $new_setup" -ForegroundColor Green
} else {
    Write-Host "❌ $old_setup nicht gefunden" -ForegroundColor Red
}

if (Test-Path $old_zip) {
    Rename-Item $old_zip $new_zip -Force
    Write-Host "✅ $old_zip → $new_zip" -ForegroundColor Green
} else {
    Write-Host "❌ $old_zip nicht gefunden" -ForegroundColor Red
}

Write-Host "✅ Fertig!" -ForegroundColor Green
