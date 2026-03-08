# ReifeschrankTracker
````````

This is the description of what the code block changes:
Update README with full project overview, architecture, workflow, and setup instructions provided by the user.

This is the code block that represents the suggested code change:

````````markdown
# 🥩 ReifeManager_R01

Professionelle Reifeschrank-Verwaltung für luftgetrocknete und geräucherte Fleischprodukte.

## 📌 Projektübersicht

`ReifeManager_R01` ist eine Windows-Desktopanwendung zur strukturierten Verwaltung von Fleischreifung in einem Reifeschrank.

Dokumentiert werden:
- Chargen
- Einzelstücke
- Gewichtsverlust
- Temperatur
- Luftfeuchte
- Reifeverlauf
- Statusüberwachung

Ziel ist die digitale Erfassung und Auswertung des Reifeprozesses für Coppa, Pancetta, Schinken und weitere Produkte.

## 🎯 Projektziel

- Handschriftliche Listen ersetzen
- Nachvollziehbare Dokumentation ermöglichen
- Automatische Berechnung des Gewichtsverlusts
- Statusbewertung (`Reift` / `Fertig` / `Warnung` / `Kritisch`)
- Übersichtliche Visualisierung
- Offline nutzbar ohne Server

## 🖥 Plattform & Technologie

| Bereich | Technologie |
|---|---|
| Plattform | Windows Desktop |
| UI Framework | WPF (Windows Presentation Foundation) |
| Framework | .NET 6 (`net6.0-windows`) |
| Sprache | C# |
| Architektur | MVVM |
| Datenspeicherung | JSON (lokal) |
| Diagramme | LiveCharts (optional) |

## 🧱 Systemarchitektur

```text
ReifeManager_R01
│
├── Models
│   ├── Charge.cs
│   ├── Fleischstueck.cs
│   ├── MessEintrag.cs
│
├── ViewModels
│   ├── MainViewModel.cs
│   ├── BaseViewModel.cs
│   ├── RelayCommand.cs
│
├── Services
│   ├── SpeicherService.cs
│   ├── BerechnungsService.cs
│
├── Views
│   ├── MainWindow.xaml
│
├── Data
│   └── chargen.json
```

## 🔁 Funktionsablauf

### 1️⃣ Programmstart
- JSON-Datei wird geladen
- Bestehende Chargen werden eingelesen
- Dashboard wird angezeigt

### 2️⃣ Charge erstellen
Benutzer definiert:
- Bezeichnung
- Fleischtyp
- Startdatum
- Zielverlust %

Speicherung erfolgt lokal in JSON.

### 3️⃣ Stück hinzufügen
Pro Charge können mehrere Stücke angelegt werden.

Eingabe:
- Startgewicht

Automatisch berechnet:
- Aktuelles Gewicht
- Gewichtsverlust %
- Status

### 4️⃣ Messung eintragen
Für jedes Stück können Messungen dokumentiert werden:
- Datum
- Gewicht
- Temperatur
- Luftfeuchte
- Notiz

## 🧮 Berechnungslogik

### Gewichtsverlust
```text
((Startgewicht - AktuellesGewicht) / Startgewicht) * 100
```

### Statuslogik

| Status | Bedingung |
|---|---|
| Reift | Verlust < Zielverlust |
| Fertig | Verlust ≥ Zielverlust |
| Warnung | Verlust ≥ 35% |
| Kritisch | > 1% Verlust pro Tag |

### Reifetage
```text
Heutiges Datum - Startdatum
```

## 📊 Visualisierung

- Gewichtsverlauf pro Stück
- Verlustkurve
- Temperaturverlauf
- Luftfeuchteverlauf

## 💾 Datenspeicherung

Alle Daten werden lokal gespeichert unter:

`/Data/chargen.json`

Keine Cloud, kein Server, keine externe Abhängigkeit.

## 🔐 Erweiterbarkeit

Geplante Erweiterungen:
- SQLite-Datenbank
- Mehrere Reifeschränke
- Benutzerverwaltung
- PDF-Export
- Sensorintegration (ESP32)
- WLAN-Waage
- HACCP-konforme Dokumentation
- Cloud-Backup

## 🎨 Benutzeroberfläche

- Modernes dunkles Design
- Sidebar-Navigation
- Dashboard-Layout
- Kartenstruktur
- Intuitive Bedienung

## 🚀 Installation

Voraussetzungen:
- Windows 10 / 11
- .NET 6 Desktop Runtime
- Visual Studio 2022/2026 (für Entwicklung)

Start:

```bash
dotnet build
dotnet run
```

Oder direkt über Visual Studio.

## 📜 Lizenz

Privates Projekt zur Verwaltung von Reifeschrank-Prozessen.
Lizenz optional definierbar (z. B. MIT).

## 👤 Autor

Entwickelt für strukturierte Reifekontrolle und handwerkliche Fleischveredelung.
````````


# Response
````````markdown
# ReifeschrankTracker
````````

This is the description of what the code block changes:
Update README with full project overview, architecture, workflow, and setup instructions provided by the user.

This is the code block that represents the suggested code change:

````````markdown
# 🥩 ReifeManager_R01

Professionelle Reifeschrank-Verwaltung für luftgetrocknete und geräucherte Fleischprodukte.

## 📌 Projektübersicht

`ReifeManager_R01` ist eine Windows-Desktopanwendung zur strukturierten Verwaltung von Fleischreifung in einem Reifeschrank.

Dokumentiert werden:
- Chargen
- Einzelstücke
- Gewichtsverlust
- Temperatur
- Luftfeuchte
- Reifeverlauf
- Statusüberwachung

Ziel ist die digitale Erfassung und Auswertung des Reifeprozesses für Coppa, Pancetta, Schinken und weitere Produkte.

## 🎯 Projektziel

- Handschriftliche Listen ersetzen
- Nachvollziehbare Dokumentation ermöglichen
- Automatische Berechnung des Gewichtsverlusts
- Statusbewertung (`Reift` / `Fertig` / `Warnung` / `Kritisch`)
- Übersichtliche Visualisierung
- Offline nutzbar ohne Server

## 🖥 Plattform & Technologie

| Bereich | Technologie |
|---|---|
| Plattform | Windows Desktop |
| UI Framework | WPF (Windows Presentation Foundation) |
| Framework | .NET 6 (`net6.0-windows`) |
| Sprache | C# |
| Architektur | MVVM |
| Datenspeicherung | JSON (lokal) |
| Diagramme | LiveCharts (optional) |

## 🧱 Systemarchitektur

```text
ReifeManager_R01
│
├── Models
│   ├── Charge.cs
│   ├── Fleischstueck.cs
│   ├── MessEintrag.cs
│
├── ViewModels
│   ├── MainViewModel.cs
│   ├── BaseViewModel.cs
│   ├── RelayCommand.cs
│
├── Services
│   ├── SpeicherService.cs
│   ├── BerechnungsService.cs
│
├── Views
│   ├── MainWindow.xaml
│
├── Data
│   └── chargen.json
```

## 🔁 Funktionsablauf

### 1️⃣ Programmstart
- JSON-Datei wird geladen
- Bestehende Chargen werden eingelesen
- Dashboard wird angezeigt

### 2️⃣ Charge erstellen
Benutzer definiert:
- Bezeichnung
- Fleischtyp
- Startdatum
- Zielverlust %

Speicherung erfolgt lokal in JSON.

### 3️⃣ Stück hinzufügen
Pro Charge können mehrere Stücke angelegt werden.

Eingabe:
- Startgewicht

Automatisch berechnet:
- Aktuelles Gewicht
- Gewichtsverlust %
- Status

### 4️⃣ Messung eintragen
Für jedes Stück können Messungen dokumentiert werden:
- Datum
- Gewicht
- Temperatur
- Luftfeuchte
- Notiz

## 🧮 Berechnungslogik

### Gewichtsverlust
```text
((Startgewicht - AktuellesGewicht) / Startgewicht) * 100
```

### Statuslogik

| Status | Bedingung |
|---|---|
| Reift | Verlust < Zielverlust |
| Fertig | Verlust ≥ Zielverlust |
| Warnung | Verlust ≥ 35% |
| Kritisch | > 1% Verlust pro Tag |

### Reifetage
```text
Heutiges Datum - Startdatum
```

## 📊 Visualisierung

- Gewichtsverlauf pro Stück
- Verlustkurve
- Temperaturverlauf
- Luftfeuchteverlauf

## 💾 Datenspeicherung

Alle Daten werden lokal gespeichert unter:

`/Data/chargen.json`

Keine Cloud, kein Server, keine externe Abhängigkeit.

## 🔐 Erweiterbarkeit

Geplante Erweiterungen:
- SQLite-Datenbank
- Mehrere Reifeschränke
- Benutzerverwaltung
- PDF-Export
- Sensorintegration (ESP32)
- WLAN-Waage
- HACCP-konforme Dokumentation
- Cloud-Backup

## 🎨 Benutzeroberfläche

- Modernes dunkles Design
- Sidebar-Navigation
- Dashboard-Layout
- Kartenstruktur
- Intuitive Bedienung

## 🚀 Installation

Voraussetzungen:
- Windows 10 / 11
- .NET 6 Desktop Runtime
- Visual Studio 2022/2026 (für Entwicklung)

Start:

```bash
dotnet build
dotnet run
```

Oder direkt über Visual Studio.

## 📜 Lizenz

Privates Projekt zur Verwaltung von Reifeschrank-Prozessen.
Lizenz optional definierbar (z. B. MIT).

## 👤 Autor

Entwickelt für strukturierte Reifekontrolle und handwerkliche Fleischveredelung.