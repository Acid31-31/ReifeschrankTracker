# ReifeschrankTracker

Ein WPF-Desktop-Anwendung zum Verfolgen von Reifeprozessen (Räuchern, Lufttrocknen).

## Voraussetzungen

- Windows 10 oder neuer
- [Visual Studio 2022](https://visualstudio.microsoft.com/) mit der Workload **.NET-Desktopentwicklung**
- .NET 8 SDK

## 🔄 Änderungen von GitHub auf deinen PC übertragen

Wenn du (oder GitHub Copilot) Änderungen auf GitHub gemacht hat und du sie lokal sehen willst, musst du diese Schritte ausführen:

### Schritt 1 – Pull Request mergen (auf GitHub.com)

1. Gehe zu **https://github.com/Acid31-31/ReifeschrankTracker**
2. Klicke oben auf **„Pull requests"**
3. Öffne den offenen Pull Request
4. Klicke auf **„Merge pull request"** → **„Confirm merge"**

### Schritt 2 – Änderungen in Visual Studio herunterladen

**Option A – In Visual Studio (empfohlen):**
1. Öffne dein Projekt in Visual Studio
2. Klicke oben in der Menüleiste auf **Git** → **Pullen** (oder **Git → Pull**)
3. Die Änderungen werden automatisch heruntergeladen

**Option B – Per Git-Befehl im Terminal:**
```cmd
git checkout main
git pull origin main
```

> 💡 **Tipp für Copilot-Nutzer:** Sage deiner KI in Visual Studio:
> `„Merge den Pull Request und pullen die neuesten Änderungen von GitHub"` –
> oder führe einfach **Git → Pull** in Visual Studio durch.

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
