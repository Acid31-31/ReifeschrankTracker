# ReifeschrankTracker

Ein WPF-Desktop-Anwendung zum Verfolgen von Reifeprozessen (Räuchern, Lufttrocknen).

---

## ⚡ SO AKTUALISIERST DU DAS PROGRAMM – 2 Wege

---

### 🟢 Weg 1 – Doppelklick (einfachster Weg, kein Wissen nötig)

1. Öffne den Projektordner auf deinem PC
2. Doppelklick auf **`Aktualisieren.ps1`**
3. Falls Windows fragt: **„Ja, ausführen"** klicken
4. Das Skript lädt alles neu, baut das Programm und startet es automatisch ✅

> ⚠️ Falls Windows das Skript blockiert: Rechtsklick auf `Aktualisieren.ps1` → **„Mit PowerShell ausführen"**

---

### 🤖 Weg 2 – Befehl an deine KI (z. B. Copilot in Visual Studio)

**Öffne deinen KI-Chat** (in Visual Studio: **Strg+Alt+I**) und **kopiere diesen Text genau so rein:**

```
Führe diese Befehle der Reihe nach im Terminal aus und zeige mir das Ergebnis:

cd C:\DEIN\PROJEKTORDNER

git pull origin main

dotnet build ReifeschrankTracker\ReifeschrankTracker.csproj --configuration Release

dotnet run --project ReifeschrankTracker\ReifeschrankTracker.csproj
```

> ⚠️ Ersetze `C:\DEIN\PROJEKTORDNER` durch den echten Pfad auf deinem PC, z. B. `C:\Users\DeinName\source\repos\ReifeschrankTracker`

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
