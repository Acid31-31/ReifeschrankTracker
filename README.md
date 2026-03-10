# ReifeschrankTracker

Ein WPF-Desktop-Anwendung zum Verfolgen von Reifeprozessen (Räuchern, Lufttrocknen).

---

## ⚡ JETZT SOFORT STARTEN – 3 Befehle im PowerShell-Terminal

Du hast bereits den Projektordner auf dem PC. Öffne dein **PowerShell-Terminal** (z. B. Visual Studio Developer PowerShell) und führe diese 3 Befehle der Reihe nach aus:

```powershell
git -C "$env:USERPROFILE\source\repos\Acid31-31\ReifeschrankTracker" pull origin copilot/implement-reifeschrank-tracker
```
```powershell
dotnet build "$env:USERPROFILE\source\repos\Acid31-31\ReifeschrankTracker\ReifeschrankTracker\ReifeschrankTracker.csproj" --configuration Release
```
```powershell
dotnet run --project "$env:USERPROFILE\source\repos\Acid31-31\ReifeschrankTracker\ReifeschrankTracker\ReifeschrankTracker.csproj"
```

> ✅ Danach startet das Programm mit **dunklem Kalender-Popup**.

---

## 🔄 Automatisch updaten per Doppelklick (für später)

Sobald `Aktualisieren.ps1` in deinem Projektordner liegt, reicht immer nur ein **Doppelklick** darauf – kein weiteres Tippen nötig.

### `Aktualisieren.ps1` noch nicht vorhanden? Einmalig im Terminal ausführen:

```powershell
Invoke-WebRequest -Uri "https://raw.githubusercontent.com/Acid31-31/ReifeschrankTracker/copilot/implement-reifeschrank-tracker/Aktualisieren.ps1" -OutFile "$env:USERPROFILE\source\repos\Acid31-31\ReifeschrankTracker\Aktualisieren.ps1"; Write-Host "Fertig!"
```

> Danach: Doppelklick auf `Aktualisieren.ps1` im Projektordner → alles wird automatisch geladen, gebaut und gestartet.

---

## 🔵 Updates dauerhaft auf den Hauptzweig (main) bringen

Du musst das **nur einmal** machen. Danach zeigt der Kalender immer dunkel.

### Schritt 1 – Gehe auf diese Seite:
👉 **https://github.com/Acid31-31/ReifeschrankTracker/pull/1**

### Schritt 2 – Klicke auf „Ready for review"
Grauer Knopf oben → einmal klicken.

### Schritt 3 – Grünen Knopf klicken
Grüner Knopf **„Merge pull request"** → klicken, dann **„Confirm merge"**.

### Schritt 4 – Dann im Terminal:
```powershell
git -C "$env:USERPROFILE\source\repos\Acid31-31\ReifeschrankTracker" pull origin main
dotnet run --project "$env:USERPROFILE\source\repos\Acid31-31\ReifeschrankTracker\ReifeschrankTracker\ReifeschrankTracker.csproj"
```

---

## Erstellen und Starten (Visual Studio)

1. Repository klonen oder ZIP entpacken.
2. `ReifeschrankTracker.sln` in Visual Studio öffnen.
3. **Strg+Umschalt+B** zum Erstellen (Build).
4. **F5** zum Starten mit Debugger oder **Strg+F5** ohne Debugger.

## Erstellen und Starten (Kommandozeile)

```bash
cd ReifeschrankTracker
dotnet run --project ReifeschrankTracker
```

## Speicherort der Datenbank

Die SQLite-Datenbank wird automatisch beim ersten Start erstellt:

```
%LOCALAPPDATA%\ReifeschrankTracker\reifen.db
```

Unter Windows typischerweise:
```
C:\Users\<Benutzername>\AppData\Local\ReifeschrankTracker\reifen.db
```

## Funktionen

- **Charge anlegen**: Neues Produkt mit Startgewicht, Methode und Ziel erfassen.
- **Gewicht eintragen**: Messpunkte im Verlauf des Reifeprozesses erfassen.
- **Verlustanzeige**: Gewichtsverlust in Gramm und Prozent wird automatisch berechnet.
- **Warnungen**: Visuelle Hinweise bei ≥30%, ≥35%, ≥40% Verlust.
- **Diagramm**: Gewichtsverlauf über Zeit als Liniendiagramm.
- **Zielerreichung**: Automatische Erkennung und Statusänderung bei Zielerreichung.

## Technologie

- .NET 8 / WPF
- Entity Framework Core (SQLite)
- LiveChartsCore (Diagramme)
- MVVM-Architektur
