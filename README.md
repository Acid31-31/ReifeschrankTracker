# ReifeschrankTracker

Ein WPF-Desktop-Anwendung zum Verfolgen von Reifeprozessen (Räuchern, Lufttrocknen).

## Voraussetzungen

- Windows 10 oder neuer
- [Visual Studio 2022](https://visualstudio.microsoft.com/) mit der Workload **.NET-Desktopentwicklung**
- .NET 8 SDK

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
