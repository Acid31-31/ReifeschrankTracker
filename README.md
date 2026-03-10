# ReifeschrankTracker

Ein WPF-Desktop-Anwendung zum Verfolgen von Reifeprozessen (Räuchern, Lufttrocknen).

---

## 🚨 `Aktualisieren.ps1` fehlt auf deinem PC? – SOFORT-LÖSUNG

Das Skript muss **einmalig heruntergeladen** werden. Danach reicht immer nur noch ein Doppelklick.

### Einmalige Einrichtung – sage deiner KI genau das:

**Öffne Copilot in Visual Studio (Strg+Alt+I) und kopiere diesen Text rein:**

```
Führe diesen PowerShell-Befehl im Terminal aus:

$dest = "$env:USERPROFILE\source\repos\Acid31-31\ReifeschrankTracker\Aktualisieren.ps1"; try { Invoke-WebRequest -Uri "https://raw.githubusercontent.com/Acid31-31/ReifeschrankTracker/main/Aktualisieren.ps1" -OutFile $dest -UseBasicParsing -ErrorAction Stop } catch { Invoke-WebRequest -Uri "https://raw.githubusercontent.com/Acid31-31/ReifeschrankTracker/copilot/implement-reifeschrank-tracker/Aktualisieren.ps1" -OutFile $dest -UseBasicParsing }; Write-Host "Fertig! Datei wurde gespeichert."
```

> ✅ Danach findest du `Aktualisieren.ps1` in deinem Projektordner.
>
> **Nächster Schritt:** Doppelklick auf `Aktualisieren.ps1` – das Programm wird gebaut und gestartet.

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

git -C "$env:USERPROFILE\source\repos\Acid31-31\ReifeschrankTracker" pull origin main

dotnet build "$env:USERPROFILE\source\repos\Acid31-31\ReifeschrankTracker\ReifeschrankTracker\ReifeschrankTracker.csproj" --configuration Release

dotnet run --project "$env:USERPROFILE\source\repos\Acid31-31\ReifeschrankTracker\ReifeschrankTracker\ReifeschrankTracker.csproj"
```

---

## 🔵 Den grünen Knopf drücken – So kriegst du ALLE Updates auf einmal

Du musst das NUR EINMAL machen. Danach läuft `Aktualisieren.ps1` für immer.

### Schritt 1 – Gehe auf diese Seite:
👉 **https://github.com/Acid31-31/ReifeschrankTracker/pull/1**

### Schritt 2 – Klicke auf „Ready for review"
Du siehst einen grauen Knopf mit dem Text **„Ready for review"** – einmal draufklicken.

### Schritt 3 – Klicke auf den grünen Knopf
Jetzt erscheint ein grüner Knopf **„Merge pull request"** – draufklicken, dann nochmal **„Confirm merge"** klicken.

### Schritt 4 – Sage deiner KI:
```
Führe im Terminal aus:
git -C "$env:USERPROFILE\source\repos\Acid31-31\ReifeschrankTracker" pull origin main
```

✅ Danach ist `Aktualisieren.ps1` bei dir und du kannst es per Doppelklick starten.

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
