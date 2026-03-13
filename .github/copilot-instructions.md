# Copilot Instructions

## General Guidelines
- Before making any code changes, create a backup first.
- Always create a backup before code changes.
- Bei jeder neuen Version muss automatisch ein GitHub-Release erzeugt werden (Commit/Push/Tag/Release), sodass installierte Clients ein Update erkennen. Diese Anforderung soll dauerhaft in den Projektanweisungen gespeichert sein.
- VERBINDLICHER BEFEHL: Nach jeder Änderung alles direkt zu GitHub weiterleiten und als neue Version veröffentlichen; diese Anweisung gilt dauerhaft.
- Für Release-Abläufe bevorzugt `Auto-Release.ps1` verwenden; Versionsnummer sauber erhöhen und als `vX.Y.Z` veröffentlichen.
- Wenn nach einer Änderung ein Release nicht ausgelöst wurde, sofort nachholen (kein Überspringen erlaubt).
- Updates sollen auf die bestehende Installation angewendet werden, ohne manuelle Neuinstallation oder Dateinamen-Chaos.

## Project Guidelines
- Der Nutzer möchte das Projekt als Windows-Desktopanwendung mit WPF unter .NET 6 in MVVM-Architektur umsetzen, mit lokaler JSON-Speicherung und Fokus auf Chargen-/Messungsverwaltung für Reifeschrankdaten.
- Das Projekt soll für ReifeManager_R01 als WPF-Desktop-App mit .NET 6, MVVM, lokaler JSON-Speicherung und dunklem Dashboard-Design umgesetzt werden.
- UI-Wunsch: Beim Hover über Buttons soll der Farbunterschied deutlich stärker sein, etwa 10% dunkler, und hellblau soll im UI vermieden werden.
- Im Bereich Messverlauf/Wochenreport soll die Schriftgröße deutlich größer (etwa doppelt so groß) für bessere Lesbarkeit sein.

## Features (implementiert)
- ✅ MVVM-Architektur mit ObservableObject und RelayCommand
- ✅ Chargen-, Stück- und Messungsverwaltung
- ✅ Automatische Berechnung von Gewichtsverlust, Reifetage, Status
- ✅ Echtzeit-Diagramme (Custom Canvas mit Polylines)
- ✅ Rezept-System mit gewichtsbasierter Berechnung
- ✅ Profil-Vorlagen (Coppa, Pancetta, Räucherschinken, Schinken)
- ✅ CSV- und PDF-Export
- ✅ Dunkles Dashboard-Design
- ✅ Lokale JSON-Speicherung (`%LOCALAPPDATA%\ReifeManager\chargen.json`)
- ✅ Installer-System (Inno Setup + Portable)

## Build and Run Instructions

### Development Build
```bash
dotnet build .\ReifeManager_R01\ReifeManager_R01.csproj
Start-Process .\ReifeManager_R01\bin\Debug\net6.0-windows\ReifeManager_R01.exe
```

### Release Build + Installer
```powershell
# Vollständigen Installer erstellen:
.\Build-Installer.ps1

# Ergebnis:
# - publish/ReifeManager/ReifeManager_R01.exe (Portable)
# - installer/ReifeManager_Portable_v1.0.0.zip
# - installer/ReifeManager_Setup_v1.0.0.exe (benötigt Inno Setup)
```

### Silent Installation
```powershell
.\Install-Silent.ps1
```

## Projektstruktur
```
ReifeManager_R01/
├── Infrastructure/
│   ├── ObservableObject.cs       # MVVM Base-Klasse
│   └── RelayCommand.cs            # ICommand-Implementation
├── Models/
│   ├── Charge.cs                  # Haupt-Datenmodell
│   ├── Fleischstueck.cs          # Fleischstück mit Messungen
│   ├── MessEintrag.cs            # Einzelmessung
│   ├── Rezept.cs                 # Rezept mit Zutaten
│   ├── ChartAxisLabel.cs         # Diagramm-Label
│   └── ReifeStatus.cs            # Enum: Kritisch, Warnung, Gut, Fertig
├── Services/
│   ├── JsonStorageService.cs     # Persistierung
│   ├── ReifeBerechnungService.cs # Business Logic
│   ├── ReifePlanService.cs       # Profil-Vorlagen
│   ├── RezeptService.cs          # Rezept-Bibliothek
│   ├── DiagrammDataService.cs    # Chart-Daten
│   ├── CsvExportService.cs       # CSV-Export
│   └── PdfExportService.cs       # PDF-Bericht
├── ViewModels/
│   └── MainViewModel.cs          # Haupt-ViewModel
├── Views/
│   ├── RezeptAuswahlWindow.xaml  # Rezept-Auswahl
│   └── RezeptDetailWindow.xaml   # Rezept-Details + Druck
├── Utils/
│   └── ValidationHelper.cs       # Eingabevalidierung
├── MainWindow.xaml               # Haupt-UI
└── App.xaml                      # Global Styles
```

## Wichtige Hinweise

### Vor Code-Änderungen
```powershell
# Immer erst Backup anlegen:
$ts=Get-Date -Format yyyyMMdd_HHmmss
Compress-Archive -Path ReifeManager_R01 -DestinationPath "Backups/backup_$ts.zip"
```

### Bei Absturz/Fehler
```powershell
# Letztes funktionierendes Backup wiederherstellen:
$latest = Get-ChildItem Backups/*.zip | Sort-Object LastWriteTime -Descending | Select-Object -First 1
Expand-Archive $latest.FullName -DestinationPath temp_restore -Force
Copy-Item temp_restore/ReifeManager_R01/* ReifeManager_R01/ -Recurse -Force
Remove-Item temp_restore -Recurse -Force
```

### Programm läuft noch beim Build
```powershell
Stop-Process -Name "ReifeManager_R01" -Force -ErrorAction SilentlyContinue
dotnet build ReifeManager_R01.sln
```

## Bekannte Probleme & Lösungen

### Problem: Absturz beim Löschen
**Ursache:** Diagramm-Aktualisierung mit null-Daten  
**Lösung:** Try-Catch in `AktualisiereDiagramm()` und null-Checks in Properties

### Problem: ComboBox funktioniert nicht
**Ursache:** Defektes Custom-Template  
**Lösung:** Natives Template mit Dark-Style verwenden

### Problem: Installer fehlt
**Ursache:** Inno Setup nicht installiert  
**Lösung:** Portable Version nutzen oder Inno Setup installieren

## Version History
- **v1.0.0** (2026-03-08): Initial Release mit Installer
  - Chargen-/Messungsverwaltung
  - Rezept-System
  - Automatischer Installer
  - Bug-Fixes für Stabilität