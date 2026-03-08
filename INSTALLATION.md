# ReifeManager - Installation

## 🚀 Schnellinstallation (EMPFOHLEN)

### **Automatische Installation auf C:\Program Files\ReifeManager**

```powershell
# 1. Installer erstellen
.\Build-Installer.ps1

# 2. Vollautomatisch installieren
.\Install-Silent.ps1
```

**Was passiert:**
- ✅ Installiert nach `C:\Program Files\ReifeManager`
- ✅ Erstellt Desktop-Icon automatisch
- ✅ Erstellt Startmenü-Eintrag
- ✅ Keine Rückfragen während Installation
- ✅ Deinstallation über Systemsteuerung

---

## 📦 Installations-Optionen

### Option 1: Vollautomatische Installation (Empfohlen)

**Voraussetzung:** Inno Setup muss installiert sein ([Download](https://jrsoftware.org/isdl.php))

```powershell
# Installer erstellen + installieren in einem Schritt:
.\Build-Installer.ps1
.\Install-Silent.ps1
```

✅ **Installiert nach:** `C:\Program Files\ReifeManager`

---

### Option 2: Portable Version (Ohne Installation)

Für USB-Stick oder ohne Administratorrechte:

```powershell
# ZIP entpacken und starten:
Expand-Archive installer\ReifeManager_Portable_v1.0.0.zip -DestinationPath C:\Deine\Ordner
Start-Process C:\Deine\Ordner\ReifeManager_R01.exe
```

**Daten werden gespeichert in:**
- `%LOCALAPPDATA%\ReifeManager\chargen.json`

---

### Option 3: Manueller Installer mit Dialogen

Falls du den Installationspfad selbst wählen möchtest:

1. `.\Build-Installer.ps1` ausführen
2. Doppelklick auf `installer\ReifeManager_Setup_v1.0.0.exe`
3. Installationsassistent durchlaufen

---

## 🔧 Inno Setup installieren (einmalig)

Falls noch nicht vorhanden:

```powershell
# Download starten:
Start-Process "https://jrsoftware.org/isdl.php"
```

**Oder manuell:**
1. [Inno Setup 6 downloaden](https://jrsoftware.org/isdl.php)
2. `innosetup-6.x.x.exe` installieren
3. `.\Build-Installer.ps1` ausführen

---

## 📋 Systemanforderungen

- **Betriebssystem:** Windows 10/11 (x64)
- **Framework:** .NET 6.0 Desktop Runtime (inkludiert in Self-Contained Build)
- **Festplatte:** ~150 MB
- **Rechte:** Administrator (nur für Installation)

---

## 🗑️ Deinstallation

```powershell
# Über Systemsteuerung:
Start-Process "ms-settings:appsfeatures"

# Oder über Startmenü:
# ReifeManager R01 > Deinstallieren
```

---

## 🔄 Update installieren

Neue Version über alte installieren:

```powershell
.\Build-Installer.ps1
.\Install-Silent.ps1
```

Bestehende Daten (`chargen.json`) bleiben erhalten.

---

## 📝 Lizenz & Support

- **Lizenz:** Siehe `LICENSE` im Repository
- **Support:** [GitHub Issues](https://github.com/Acid31-31/ReifeschrankTracker/issues)
