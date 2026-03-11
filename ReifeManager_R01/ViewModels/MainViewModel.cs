using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ReifeManager_R01.Infrastructure;
using ReifeManager_R01.Models;
using ReifeManager_R01.Services;
using ReifeManager_R01.Utils;

namespace ReifeManager_R01.ViewModels;

public class MainViewModel : ObservableObject
{
    private const double ChartWidth = 1080;
    private const double ChartHeight = 220;
    private const double ChartLeft = 10;
    private const double ChartTop = 10;
    private const double ChartRight = 10;
    private const double ChartBottom = 24;

    private readonly JsonStorageService _storageService = new();
    private readonly ReifeBerechnungService _berechnungService = new();
    private readonly CsvExportService _csvExportService = new();
    private readonly PdfExportService _pdfExportService = new();
    private readonly DiagrammDataService _diagrammDataService = new();
    private readonly ReifePlanService _reifePlanService = new();
    private readonly RezeptService _rezeptService = new();
    private readonly UpdateService _updateService = new();
    private UpdateInfo? _verfuegbaresUpdate;
    private readonly ObservableCollection<Fleischstueck> _leereStuecke = new();
    private readonly ObservableCollection<MessEintrag> _leereMessungen = new();

    public ObservableCollection<string> ProfilOptionen { get; }

    private string _ausgewaehltesProfil = string.Empty;
    public string AusgewaehltesProfil
    {
        get => _ausgewaehltesProfil;
        set
        {
            if (!SetProperty(ref _ausgewaehltesProfil, value))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_ausgewaehltesProfil))
            {
                ProfilEmpfehlung = string.Empty;
                return;
            }

            ProfilEmpfehlung = _reifePlanService.HoleEmpfohlenesFleisch(_ausgewaehltesProfil);
            
            NeueBezeichnung = ErzeugeNaechsteBezeichnung(_ausgewaehltesProfil);
            NeuerFleischtyp = ProfilEmpfehlung;
            NeuesZielverlustProzent = _reifePlanService.HoleEmpfohlenenZielverlust(_ausgewaehltesProfil).ToString("F0", CultureInfo.CurrentCulture);
        }
    }

    private string _profilEmpfehlung = string.Empty;
    public string ProfilEmpfehlung
    {
        get => _profilEmpfehlung;
        set => SetProperty(ref _profilEmpfehlung, value);
    }

    public PointCollection GewichtChartPoints
    {
        get => _gewichtChartPoints;
        private set => SetProperty(ref _gewichtChartPoints, value);
    }

    public PointCollection VerlustChartPoints
    {
        get => _verlustChartPoints;
        private set => SetProperty(ref _verlustChartPoints, value);
    }

    public PointCollection ZielverlustChartPoints
    {
        get => _zielverlustChartPoints;
        private set => SetProperty(ref _zielverlustChartPoints, value);
    }

    public PointCollection ZielgewichtChartPoints
    {
        get => _zielgewichtChartPoints;
        private set => SetProperty(ref _zielgewichtChartPoints, value);
    }

    private PointCollection _gewichtChartPoints = new();
    private PointCollection _verlustChartPoints = new();
    private PointCollection _zielverlustChartPoints = new();
    private PointCollection _zielgewichtChartPoints = new();

    public ObservableCollection<Charge> Chargen { get; }

    public ObservableCollection<Fleischstueck> AktiveStuecke => SelectedCharge?.Stuecke ?? _leereStuecke;
    public ObservableCollection<MessEintrag> AktiveMessungen => SelectedStueck?.Messungen ?? _leereMessungen;

    private Charge? _selectedCharge;
    public Charge? SelectedCharge
    {
        get => _selectedCharge;
        set
        {
            if (!SetProperty(ref _selectedCharge, value))
            {
                return;
            }

            SelectedStueck = _selectedCharge?.Stuecke.FirstOrDefault();
            OnPropertyChanged(nameof(AktiveStuecke));
            AktualisiereDiagramm();
        }
    }

    private Fleischstueck? _selectedStueck;
    public Fleischstueck? SelectedStueck
    {
        get => _selectedStueck;
        set
        {
            if (!SetProperty(ref _selectedStueck, value))
            {
                return;
            }

            if (_selectedStueck is not null)
            {
                NeuesStartgewicht = _selectedStueck.Startgewicht.ToString("F0", CultureInfo.CurrentCulture);
            }
            else
            {
                NeuesStartgewicht = string.Empty;
            }

            SelectedMessung = _selectedStueck?.Messungen.OrderByDescending(m => m.Datum).FirstOrDefault();
            OnPropertyChanged(nameof(AktiveMessungen));
            AktualisiereDiagramm();
        }
    }

    private MessEintrag? _selectedMessung;
    public MessEintrag? SelectedMessung
    {
        get => _selectedMessung;
        set
        {
            if (!SetProperty(ref _selectedMessung, value))
            {
                return;
            }

            if (_selectedMessung is null)
            {
                return;
            }

            NeuesMessdatum = _selectedMessung.Datum;
            NeuesMessgewicht = _selectedMessung.Gewicht.ToString("F0", CultureInfo.CurrentCulture);
            NeueTemperatur = _selectedMessung.Temperatur.ToString("F1", CultureInfo.CurrentCulture);
            NeueLuftfeuchte = _selectedMessung.Luftfeuchte.ToString("F1", CultureInfo.CurrentCulture);
            NeueNotiz = _selectedMessung.Notiz;
        }
    }

    private string _neueBezeichnung = string.Empty;
    public string NeueBezeichnung
    {
        get => _neueBezeichnung;
        set => SetProperty(ref _neueBezeichnung, value);
    }

    private string _neuerFleischtyp = string.Empty;
    public string NeuerFleischtyp
    {
        get => _neuerFleischtyp;
        set => SetProperty(ref _neuerFleischtyp, value);
    }

    private DateTime _neuesStartdatum = DateTime.Today;
    public DateTime NeuesStartdatum
    {
        get => _neuesStartdatum;
        set => SetProperty(ref _neuesStartdatum, value);
    }

    private string _neuesZielverlustProzent = "30";
    public string NeuesZielverlustProzent
    {
        get => _neuesZielverlustProzent;
        set => SetProperty(ref _neuesZielverlustProzent, value);
    }

    private string _neuesStartgewicht = string.Empty;
    public string NeuesStartgewicht
    {
        get => _neuesStartgewicht;
        set => SetProperty(ref _neuesStartgewicht, value);
    }

    private DateTime _neuesMessdatum = DateTime.Today;
    public DateTime NeuesMessdatum
    {
        get => _neuesMessdatum;
        set => SetProperty(ref _neuesMessdatum, value);
    }

    private string _neuesMessgewicht = string.Empty;
    public string NeuesMessgewicht
    {
        get => _neuesMessgewicht;
        set => SetProperty(ref _neuesMessgewicht, value);
    }

    private string _neueTemperatur = string.Empty;
    public string NeueTemperatur
    {
        get => _neueTemperatur;
        set => SetProperty(ref _neueTemperatur, value);
    }

    private string _neueLuftfeuchte = string.Empty;
    public string NeueLuftfeuchte
    {
        get => _neueLuftfeuchte;
        set => SetProperty(ref _neueLuftfeuchte, value);
    }

    private string _neueNotiz = string.Empty;
    public string NeueNotiz
    {
        get => _neueNotiz;
        set => SetProperty(ref _neueNotiz, value);
    }

    private string _statusmeldung = "ReifeManager bereit";
    public string Statusmeldung
    {
        get => _statusmeldung;
        set => SetProperty(ref _statusmeldung, value);
    }

    private string _updateHinweis = "Update-Prüfung läuft...";
    public string UpdateHinweis
    {
        get => _updateHinweis;
        set => SetProperty(ref _updateHinweis, value);
    }

    private bool _updateVerfuegbar;
    public bool UpdateVerfuegbar
    {
        get => _updateVerfuegbar;
        set => SetProperty(ref _updateVerfuegbar, value);
    }

    public ICommand ChargeHinzufuegenCommand { get; }
    public ICommand ChargeLoeschenCommand { get; }
    public ICommand StueckHinzufuegenCommand { get; }
    public ICommand StueckBearbeitenCommand { get; }
    public ICommand StueckLoeschenCommand { get; }
    public ICommand MessungHinzufuegenCommand { get; }
    public ICommand MessungBearbeitenCommand { get; }
    public ICommand MessungLoeschenCommand { get; }
    public ICommand VerlaufExportierenCommand { get; }
    public ICommand BerichtExportierenCommand { get; }
    public ICommand ProfilInfoAnzeigenCommand { get; }
    public ICommand ExportDateiOeffnenCommand { get; }
    public ICommand ExportOrdnerOeffnenCommand { get; }
    public ICommand UpdateStartenCommand { get; }
    public ICommand UpdatesPruefenCommand { get; }

    public ObservableCollection<ChartAxisLabel> ChartXAxisLabels { get; } = new();
    public ObservableCollection<ChartAxisLabel> ChartYAxisLabels { get; } = new();
    public ObservableCollection<ExportEintrag> ExportHistorie { get; } = new();

    public MainViewModel()
    {
        ProfilOptionen = new ObservableCollection<string>(_reifePlanService.HoleProfile());
        ProfilEmpfehlung = string.Empty;

        var geladeneChargen = _storageService.Laden();
        Chargen = new ObservableCollection<Charge>(geladeneChargen);

        // Wenn leer, Demo-Daten erstellen
        if (Chargen.Count == 0)
        {
            ErstelleDemoDaten();
        }

        foreach (var charge in Chargen)
        {
            if (charge.PoekelnTage == 0 && charge.AbbrennenTage == 0 && charge.RaeuchernTage == 0 && charge.ReifenTage == 0)
            {
                _reifePlanService.AnwendenProfil(charge);
            }

            AktualisiereCharge(charge);
        }

        SelectedCharge = Chargen.FirstOrDefault();

        ChargeHinzufuegenCommand = new RelayCommand(_ => ChargeHinzufuegen());
        ChargeLoeschenCommand = new RelayCommand(_ => ChargeLoeschen());
        StueckHinzufuegenCommand = new RelayCommand(_ => StueckHinzufuegen());
        StueckBearbeitenCommand = new RelayCommand(_ => StueckBearbeiten());
        StueckLoeschenCommand = new RelayCommand(_ => StueckLoeschen());
        MessungHinzufuegenCommand = new RelayCommand(_ => MessungHinzufuegen());
        MessungBearbeitenCommand = new RelayCommand(_ => MessungBearbeiten());
        MessungLoeschenCommand = new RelayCommand(_ => MessungLoeschen());
        VerlaufExportierenCommand = new RelayCommand(_ => VerlaufExportieren());
        BerichtExportierenCommand = new RelayCommand(_ => BerichtExportieren());
        ProfilInfoAnzeigenCommand = new RelayCommand(_ => ProfilInfoAnzeigen());
        ExportDateiOeffnenCommand = new RelayCommand(p => OeffneExportDateiAusHistorie(p));
        ExportOrdnerOeffnenCommand = new RelayCommand(p => OeffneExportOrdnerAusHistorie(p));
        UpdateStartenCommand = new RelayCommand(_ => _ = UpdateStartenAsync());
        UpdatesPruefenCommand = new RelayCommand(_ => _ = PruefeAufUpdateAsync(true));

        Statusmeldung = $"✓ Programm gestartet — {Chargen.Count} Chargen geladen";
        _ = PruefeAufUpdateAsync();
    }

    private void ErstelleDemoDaten()
    {
        var demoCharge = new Charge
        {
            Bezeichnung = "Demo Coppa März 2026",
            Fleischtyp = "Coppa",
            Startdatum = DateTime.Today.AddDays(-10),
            ZielverlustProzent = 30,
            StatusUebersicht = "Reift"
        };

        var stueck1 = new Fleischstueck { Startgewicht = 2500 };
        var stueck2 = new Fleischstueck { Startgewicht = 2300 };

        // Messungen für Stück 1
        stueck1.Messungen.Add(new MessEintrag
        {
            Datum = DateTime.Today.AddDays(-10),
            Gewicht = 2500,
            Temperatur = 12.5,
            Luftfeuchte = 75,
            Notiz = "Start"
        });
        stueck1.Messungen.Add(new MessEintrag
        {
            Datum = DateTime.Today.AddDays(-5),
            Gewicht = 2425,
            Temperatur = 12.8,
            Luftfeuchte = 74,
            Notiz = "Guter Verlauf"
        });
        stueck1.Messungen.Add(new MessEintrag
        {
            Datum = DateTime.Today,
            Gewicht = 2350,
            Temperatur = 12.3,
            Luftfeuchte = 76,
            Notiz = "Reifung läuft gut"
        });

        // Messungen für Stück 2
        stueck2.Messungen.Add(new MessEintrag
        {
            Datum = DateTime.Today.AddDays(-10),
            Gewicht = 2300,
            Temperatur = 12.5,
            Luftfeuchte = 75,
            Notiz = "Start"
        });
        stueck2.Messungen.Add(new MessEintrag
        {
            Datum = DateTime.Today.AddDays(-5),
            Gewicht = 2235,
            Temperatur = 12.8,
            Luftfeuchte = 74,
            Notiz = "Normaler Verlauf"
        });
        stueck2.Messungen.Add(new MessEintrag
        {
            Datum = DateTime.Today,
            Gewicht = 2170,
            Temperatur = 12.3,
            Luftfeuchte = 76,
            Notiz = "Schön auf Gewicht"
        });

        demoCharge.Stuecke.Add(stueck1);
        demoCharge.Stuecke.Add(stueck2);

        Chargen.Add(demoCharge);
        AktualisiereCharge(demoCharge);
        Speichern();

        Statusmeldung = "✓ Demo-Daten erstellt — Startklar!";
    }

    private string ErzeugeNaechsteBezeichnung(string profil)
    {
        if (string.IsNullOrWhiteSpace(profil))
        {
            return string.Empty;
        }

        var prefix = profil.Trim();
        var maxNummer = 0;

        foreach (var charge in Chargen)
        {
            var name = charge.Bezeichnung?.Trim();
            if (string.IsNullOrWhiteSpace(name) || !name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var rest = name.Substring(prefix.Length).Trim();
            if (int.TryParse(rest, out var nummer) && nummer > maxNummer)
            {
                maxNummer = nummer;
            }
        }

        var naechsteNummer = maxNummer + 1;
        var kandidat = $"{prefix} {naechsteNummer}";

        while (Chargen.Any(c => string.Equals(c.Bezeichnung?.Trim(), kandidat, StringComparison.OrdinalIgnoreCase)))
        {
            naechsteNummer++;
            kandidat = $"{prefix} {naechsteNummer}";
        }

        return kandidat;
    }

    private void ChargeHinzufuegen()
    {
        if (!TryParseDouble(NeuesZielverlustProzent, out var zielverlustProzent) ||
            !ValidationHelper.IsValidName(NeueBezeichnung) ||
            !ValidationHelper.IsValidLossPercentage(zielverlustProzent))
        {
            Statusmeldung = "❌ Ungültige Eingabe. Bitte prüfen Sie die Chargendaten.";
            return;
        }

        var fleischtyp = string.IsNullOrWhiteSpace(NeuerFleischtyp)
            ? _reifePlanService.HoleEmpfohlenesFleisch(AusgewaehltesProfil)
            : NeuerFleischtyp.Trim();

        var charge = new Charge
        {
            Bezeichnung = NeueBezeichnung.Trim(),
            Fleischtyp = fleischtyp,
            Startdatum = NeuesStartdatum,
            ZielverlustProzent = zielverlustProzent,
            StatusUebersicht = "Keine Stücke"
        };

        _reifePlanService.AnwendenProfilName(charge, AusgewaehltesProfil);

        var rezepte = _rezeptService.HoleRezepteZuProfil(AusgewaehltesProfil);
        if (rezepte.Count > 0)
        {
            var rezeptFenster = new Views.RezeptAuswahlWindow(charge, rezepte)
            {
                Owner = Application.Current.MainWindow
            };
            
            if (rezeptFenster.ShowDialog() == true && rezeptFenster.AusgewaehlitesRezept is not null)
            {
                charge.Rezept = rezeptFenster.AusgewaehlitesRezept;
                charge.PoekelnTage = rezeptFenster.AusgewaehlitesRezept.PoekelnTage;
                charge.AbbrennenTage = rezeptFenster.AusgewaehlitesRezept.AbbrennenTage;
                charge.RaeuchernTage = rezeptFenster.AusgewaehlitesRezept.RaeuchernTage;
                charge.ReifenTage = rezeptFenster.AusgewaehlitesRezept.ReifenTage;
                Statusmeldung = $"✓ Rezept '{charge.Rezept.Name}' ausgewählt.";
            }
            else
            {
                Statusmeldung = "ℹ️ Kein Rezept ausgewählt — Standardwerte verwendet.";
            }
        }

        Chargen.Add(charge);
        SelectedCharge = charge;

        NeueBezeichnung = ErzeugeNaechsteBezeichnung(AusgewaehltesProfil);
        NeuerFleischtyp = ProfilEmpfehlung;
        NeuesStartdatum = DateTime.Today;
        NeuesZielverlustProzent = _reifePlanService.HoleEmpfohlenenZielverlust(AusgewaehltesProfil).ToString("F0", CultureInfo.CurrentCulture);

        Statusmeldung = $"✓ Charge '{charge.Bezeichnung}' angelegt ({AusgewaehltesProfil}).";
        Speichern();
    }

    private void ChargeLoeschen()
    {
        if (SelectedCharge is null)
        {
            Statusmeldung = "❌ Keine Charge ausgewählt.";
            return;
        }

        var zuEntfernen = SelectedCharge;
        var name = zuEntfernen.Bezeichnung;
        Chargen.Remove(zuEntfernen);
        SelectedCharge = Chargen.FirstOrDefault();
        Statusmeldung = $"✓ Charge '{name}' gelöscht.";
        Speichern();
    }

    private void StueckHinzufuegen()
    {
        if (SelectedCharge is null)
        {
            Statusmeldung = "❌ Keine Charge ausgewählt.";
            return;
        }

        if (!TryParseDouble(NeuesStartgewicht, out var startgewicht) || !ValidationHelper.IsValidWeight(startgewicht))
        {
            Statusmeldung = "❌ Startgewicht ungültig (0-1.000.000g).";
            return;
        }

        var stueck = new Fleischstueck
        {
            Startgewicht = startgewicht
        };

        AktualisiereStueck(SelectedCharge, stueck, DateTime.Today);
        SelectedCharge.Stuecke.Add(stueck);
        AktualisiereChargeStatus(SelectedCharge);

        SelectedStueck = stueck;
        NeuesStartgewicht = string.Empty;

        Statusmeldung = $"✓ Stück {SelectedCharge.Stuecke.Count} ({startgewicht:F0}g) angelegt.";
        Speichern();
    }

    private void StueckBearbeiten()
    {
        if (SelectedCharge is null || SelectedStueck is null)
        {
            Statusmeldung = "❌ Kein Stück ausgewählt.";
            return;
        }

        if (!TryParseDouble(NeuesStartgewicht, out var startgewicht) || !ValidationHelper.IsValidWeight(startgewicht))
        {
            Statusmeldung = "❌ Startgewicht ungültig (0-1.000.000g).";
            return;
        }

        SelectedStueck.Startgewicht = startgewicht;
        var bezug = SelectedStueck.Messungen.OrderByDescending(m => m.Datum).FirstOrDefault()?.Datum ?? DateTime.Today;
        AktualisiereStueck(SelectedCharge, SelectedStueck, bezug);
        AktualisiereChargeStatus(SelectedCharge);
        OnPropertyChanged(nameof(AktiveStuecke));

        Statusmeldung = $"✓ Stück aktualisiert auf {startgewicht:F0}g.";
        Speichern();
        AktualisiereDiagramm();
    }

    private void StueckLoeschen()
    {
        if (SelectedCharge is null || SelectedStueck is null)
        {
            Statusmeldung = "❌ Kein Stück ausgewählt.";
            return;
        }

        var zuEntfernen = SelectedStueck;
        SelectedCharge.Stuecke.Remove(zuEntfernen);
        SelectedStueck = SelectedCharge.Stuecke.FirstOrDefault();
        AktualisiereChargeStatus(SelectedCharge);
        OnPropertyChanged(nameof(AktiveStuecke));

        Statusmeldung = "✓ Stück gelöscht.";
        Speichern();
        AktualisiereDiagramm();
    }

    private void MessungHinzufuegen()
    {
        if (SelectedCharge is null || SelectedStueck is null)
        {
            Statusmeldung = "❌ Keine Charge oder Stück ausgewählt.";
            return;
        }

        if (!TryParseDouble(NeuesMessgewicht, out var messgewicht) ||
            !TryParseDouble(NeueTemperatur, out var temperatur) ||
            !TryParseDouble(NeueLuftfeuchte, out var luftfeuchte) ||
            !ValidationHelper.IsValidWeight(messgewicht) ||
            !ValidationHelper.IsValidTemperature(temperatur) ||
            !ValidationHelper.IsValidHumidity(luftfeuchte))
        {
            Statusmeldung = "❌ Messwerte ungültig. Prüfen Sie Gewicht, Temperatur und Luftfeuchte.";
            return;
        }

        var messung = new MessEintrag
        {
            Datum = NeuesMessdatum,
            Gewicht = messgewicht,
            Temperatur = temperatur,
            Luftfeuchte = luftfeuchte,
            Notiz = NeueNotiz.Trim()
        };

        SelectedStueck.Messungen.Add(messung);
        AktualisiereStueck(SelectedCharge, SelectedStueck, NeuesMessdatum);
        AktualisiereChargeStatus(SelectedCharge);
        OnPropertyChanged(nameof(AktiveMessungen));

        NeuesMessgewicht = string.Empty;
        NeueTemperatur = string.Empty;
        NeueLuftfeuchte = string.Empty;
        NeueNotiz = string.Empty;
        SelectedMessung = messung;

        Statusmeldung = $"✓ Messung vom {NeuesMessdatum:dd.MM.yyyy} gespeichert ({messgewicht:F0}g).";
        Speichern();
        AktualisiereDiagramm();
    }

    private void MessungBearbeiten()
    {
        if (SelectedCharge is null || SelectedStueck is null || SelectedMessung is null)
        {
            Statusmeldung = "❌ Keine Messung ausgewählt.";
            return;
        }

        if (!TryParseDouble(NeuesMessgewicht, out var messgewicht) ||
            !TryParseDouble(NeueTemperatur, out var temperatur) ||
            !TryParseDouble(NeueLuftfeuchte, out var luftfeuchte) ||
            !ValidationHelper.IsValidWeight(messgewicht) ||
            !ValidationHelper.IsValidTemperature(temperatur) ||
            !ValidationHelper.IsValidHumidity(luftfeuchte))
        {
            Statusmeldung = "❌ Messwerte ungültig. Prüfen Sie Gewicht, Temperatur und Luftfeuchte.";
            return;
        }

        SelectedMessung.Datum = NeuesMessdatum;
        SelectedMessung.Gewicht = messgewicht;
        SelectedMessung.Temperatur = temperatur;
        SelectedMessung.Luftfeuchte = luftfeuchte;
        SelectedMessung.Notiz = NeueNotiz.Trim();

        var bezug = SelectedStueck.Messungen.OrderByDescending(m => m.Datum).FirstOrDefault()?.Datum ?? DateTime.Today;
        AktualisiereStueck(SelectedCharge, SelectedStueck, bezug);
        AktualisiereChargeStatus(SelectedCharge);
        OnPropertyChanged(nameof(AktiveMessungen));

        Statusmeldung = "✓ Messung aktualisiert.";
        Speichern();
        AktualisiereDiagramm();
    }

    private void MessungLoeschen()
    {
        if (SelectedCharge is null || SelectedStueck is null || SelectedMessung is null)
        {
            Statusmeldung = "❌ Keine Messung ausgewählt.";
            return;
        }

        SelectedStueck.Messungen.Remove(SelectedMessung);

        var bezug = SelectedStueck.Messungen.OrderByDescending(m => m.Datum).FirstOrDefault()?.Datum ?? DateTime.Today;
        AktualisiereStueck(SelectedCharge, SelectedStueck, bezug);
        AktualisiereChargeStatus(SelectedCharge);

        SelectedMessung = SelectedStueck.Messungen.OrderByDescending(m => m.Datum).FirstOrDefault();
        OnPropertyChanged(nameof(AktiveMessungen));

        Statusmeldung = "✓ Messung gelöscht.";
        Speichern();
        AktualisiereDiagramm();
    }

    private void VerlaufExportieren()
    {
        if (SelectedCharge is null)
        {
            Statusmeldung = "❌ Keine Charge zum Exportieren ausgewählt.";
            return;
        }

        try
        {
            var pfad = _csvExportService.ExportiereCharge(SelectedCharge);
            FuegeExportZurHistorieHinzu("CSV", pfad);
            OeffneExportDatei(pfad);
            Statusmeldung = $"✓ CSV exportiert: {Path.GetFileName(pfad)}";
        }
        catch (Exception ex)
        {
            Statusmeldung = $"❌ CSV-Export fehlgeschlagen: {ex.Message}";
        }
    }

    private void BerichtExportieren()
    {
        if (SelectedCharge is null)
        {
            Statusmeldung = "❌ Keine Charge zum Exportieren ausgewählt.";
            return;
        }

        try
        {
            var pfad = _pdfExportService.ExportierePdf(SelectedCharge);
            FuegeExportZurHistorieHinzu("Bericht", pfad);
            OeffneExportDatei(pfad);
            Statusmeldung = $"✓ Bericht exportiert: {Path.GetFileName(pfad)}";
        }
        catch (Exception ex)
        {
            Statusmeldung = $"❌ Bericht-Export fehlgeschlagen: {ex.Message}";
        }
    }

    private void FuegeExportZurHistorieHinzu(string typ, string pfad)
    {
        ExportHistorie.Insert(0, new ExportEintrag
        {
            Zeitpunkt = DateTime.Now,
            Typ = typ,
            Dateiname = Path.GetFileName(pfad),
            Pfad = pfad
        });

        while (ExportHistorie.Count > 20)
        {
            ExportHistorie.RemoveAt(ExportHistorie.Count - 1);
        }
    }

    private void OeffneExportDateiAusHistorie(object? parameter)
    {
        if (parameter is not string pfad || string.IsNullOrWhiteSpace(pfad))
        {
            return;
        }

        if (!File.Exists(pfad))
        {
            Statusmeldung = "❌ Exportdatei nicht gefunden.";
            return;
        }

        OeffneExportDatei(pfad);
    }

    private void OeffneExportOrdnerAusHistorie(object? parameter)
    {
        if (parameter is not string pfad || string.IsNullOrWhiteSpace(pfad))
        {
            return;
        }

        var ordner = Path.GetDirectoryName(pfad);
        if (string.IsNullOrWhiteSpace(ordner) || !Directory.Exists(ordner))
        {
            Statusmeldung = "❌ Exportordner nicht gefunden.";
            return;
        }

        Process.Start(new ProcessStartInfo(ordner)
        {
            UseShellExecute = true
        });
    }

    private static void OeffneExportDatei(string pfad)
    {
        Process.Start(new ProcessStartInfo(pfad)
        {
            UseShellExecute = true
        });
    }

    private void AktualisiereCharge(Charge charge)
    {
        foreach (var stueck in charge.Stuecke)
        {
            var bezug = stueck.Messungen.OrderByDescending(m => m.Datum).FirstOrDefault()?.Datum ?? DateTime.Today;
            AktualisiereStueck(charge, stueck, bezug);
        }

        AktualisiereChargeStatus(charge);
    }

    private void AktualisiereStueck(Charge charge, Fleischstueck stueck, DateTime standDatum)
    {
        var aktuell = stueck.Messungen.OrderByDescending(m => m.Datum).FirstOrDefault()?.Gewicht ?? stueck.Startgewicht;
        var verlust = _berechnungService.BerechneGewichtsverlust(stueck.Startgewicht, aktuell);
        var reifetage = _berechnungService.BerechneReifetage(charge.Startdatum, standDatum);
        var tagesverlust = _berechnungService.BerechneDurchschnittlichenTagesverlust(verlust, reifetage);
        var status = _berechnungService.BerechneStatus(stueck.Startgewicht, aktuell, charge.ZielverlustProzent, charge.Startdatum, standDatum);

        stueck.SetBerechneteWerte(aktuell, verlust, reifetage, tagesverlust, status);
    }

    private void AktualisiereChargeStatus(Charge charge)
    {
        if (charge.Stuecke.Count == 0)
        {
            charge.StatusUebersicht = "Keine Stücke";
            return;
        }

        if (charge.Stuecke.Any(s => s.Status == ReifeStatus.Kritisch))
        {
            charge.StatusUebersicht = "🔴 Kritisch";
            return;
        }

        if (charge.Stuecke.Any(s => s.Status == ReifeStatus.Warnung))
        {
            charge.StatusUebersicht = "🟡 Warnung";
            return;
        }

        if (charge.Stuecke.All(s => s.Status == ReifeStatus.Fertig))
        {
            charge.StatusUebersicht = "🟢 Fertig";
            return;
        }

        charge.StatusUebersicht = "🟠 Reift";
    }

    private void Speichern()
    {
        _storageService.Speichern(Chargen);
    }

    public void StueckAusGridAktualisieren()
    {
        if (SelectedCharge is null || SelectedStueck is null)
        {
            return;
        }

        if (!ValidationHelper.IsValidWeight(SelectedStueck.Startgewicht))
        {
            Statusmeldung = "❌ Ungültiges Startgewicht im Stück.";
            return;
        }

        var bezug = SelectedStueck.Messungen.OrderByDescending(m => m.Datum).FirstOrDefault()?.Datum ?? DateTime.Today;
        AktualisiereStueck(SelectedCharge, SelectedStueck, bezug);
        AktualisiereChargeStatus(SelectedCharge);
        NeuesStartgewicht = SelectedStueck.Startgewicht.ToString("F0", CultureInfo.CurrentCulture);

        Statusmeldung = "✓ Stück per Doppelklick bearbeitet und gespeichert.";
        Speichern();
        AktualisiereDiagramm();
    }

    public void MessungAusGridAktualisieren()
    {
        if (SelectedCharge is null || SelectedStueck is null || SelectedMessung is null)
        {
            return;
        }

        if (!ValidationHelper.IsValidWeight(SelectedMessung.Gewicht) ||
            !ValidationHelper.IsValidTemperature(SelectedMessung.Temperatur) ||
            !ValidationHelper.IsValidHumidity(SelectedMessung.Luftfeuchte))
        {
            Statusmeldung = "❌ Ungültige Messwerte in der Tabelle.";
            return;
        }

        var bezug = SelectedStueck.Messungen.OrderByDescending(m => m.Datum).FirstOrDefault()?.Datum ?? DateTime.Today;
        AktualisiereStueck(SelectedCharge, SelectedStueck, bezug);
        AktualisiereChargeStatus(SelectedCharge);
        OnPropertyChanged(nameof(AktiveMessungen));

        NeuesMessdatum = SelectedMessung.Datum;
        NeuesMessgewicht = SelectedMessung.Gewicht.ToString("F0", CultureInfo.CurrentCulture);
        NeueTemperatur = SelectedMessung.Temperatur.ToString("F1", CultureInfo.CurrentCulture);
        NeueLuftfeuchte = SelectedMessung.Luftfeuchte.ToString("F1", CultureInfo.CurrentCulture);
        NeueNotiz = SelectedMessung.Notiz;

        Statusmeldung = "✓ Messung per Doppelklick bearbeitet und gespeichert.";
        Speichern();
        AktualisiereDiagramm();
    }

    private void AktualisiereDiagramm()
    {
        ChartXAxisLabels.Clear();
        ChartYAxisLabels.Clear();

        if (SelectedStueck is null || SelectedStueck.Messungen.Count == 0)
        {
            GewichtChartPoints = new PointCollection();
            VerlustChartPoints = new PointCollection();
            ZielverlustChartPoints = new PointCollection();
            ZielgewichtChartPoints = new PointCollection();
            return;
        }

        var messungen = SelectedStueck.Messungen.OrderBy(m => m.Datum).ToList();
        var count = messungen.Count;

        var gewichte = messungen.Select(m => m.Gewicht).ToList();
        var verluste = messungen.Select(m => ((SelectedStueck.Startgewicht - m.Gewicht) / SelectedStueck.Startgewicht) * 100).ToList();

        var zielverlust = SelectedCharge?.ZielverlustProzent ?? 0;
        var zielgewicht = SelectedStueck.Startgewicht * (1 - (zielverlust / 100.0));

        var zielverlustReihe = Enumerable.Repeat(zielverlust, count).ToList();
        var zielgewichtReihe = Enumerable.Repeat(zielgewicht, count).ToList();

        var all = new List<double>();
        all.AddRange(gewichte);
        all.AddRange(verluste);
        all.AddRange(zielverlustReihe);
        all.AddRange(zielgewichtReihe);

        var min = Math.Min(-50, all.Min());
        var max = all.Max();
        if (Math.Abs(max - min) < 0.0001)
        {
            max = min + 1;
        }

        GewichtChartPoints = ErzeugePunkte(gewichte, count, min, max);
        VerlustChartPoints = ErzeugePunkte(verluste, count, min, max);
        ZielverlustChartPoints = ErzeugePunkte(zielverlustReihe, count, min, max);
        ZielgewichtChartPoints = ErzeugePunkte(zielgewichtReihe, count, min, max);

        ErzeugeAchsenLabels(messungen, min, max);
    }

    private static PointCollection ErzeugePunkte(IReadOnlyList<double> werte, int anzahl, double min, double max)
    {
        var punkte = new PointCollection();
        if (anzahl == 0)
        {
            return punkte;
        }

        var nutzBreite = ChartWidth - (ChartLeft * 2);
        var nutzHoehe = ChartHeight - (ChartTop * 2);

        for (int i = 0; i < anzahl; i++)
        {
            var x = ChartLeft + (anzahl == 1 ? 0 : i * (nutzBreite / (anzahl - 1)));
            var norm = (werte[i] - min) / (max - min);
            var y = ChartTop + (nutzHoehe - (norm * nutzHoehe));
            punkte.Add(new Point(x, y));
        }

        return punkte;
    }

    private void ErzeugeAchsenLabels(IReadOnlyList<MessEintrag> messungen, double min, double max)
    {
        const int yTicks = 6;
        for (int i = 0; i <= yTicks; i++)
        {
            var ratio = (double)i / yTicks;
            var value = max - ((max - min) * ratio);
            var y = ChartTop + ((ChartHeight - ChartTop) * ratio) - 8;
            ChartYAxisLabels.Add(new ChartAxisLabel
            {
                X = 2,
                Y = y,
                Text = value.ToString("F0", CultureInfo.CurrentCulture)
            });
        }

        var usableWidth = ChartWidth - ChartLeft - ChartRight;

        for (int i = 0; i < messungen.Count; i++)
        {
            var x = ChartLeft + (messungen.Count == 1 ? 0 : i * (usableWidth / (messungen.Count - 1))) - 16;
            ChartXAxisLabels.Add(new ChartAxisLabel
            {
                X = x,
                Y = ChartHeight - ChartBottom + 2,
                Text = messungen[i].Datum.ToString("dd.MM")
            });
        }
    }

    private void ProfilInfoAnzeigen()
    {
        var empfohlen = _reifePlanService.HoleEmpfohlenesFleisch(AusgewaehltesProfil);

        MessageBox.Show(
            $"Profil: {AusgewaehltesProfil}\n\nEmpfohlenes Fleisch:\n{empfohlen}\n\nAblauf wird beim Anlegen automatisch gesetzt (Pökeln, Abbrennen, Räuchern, Reifen).",
            "Profil-Empfehlung",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private async Task PruefeAufUpdateAsync(bool manuell = false)
    {
        try
        {
            Debug.WriteLine($"🔍 [ViewModel] Prüfung gestartet (manuell={manuell})");
            var update = await _updateService.PruefeAufUpdateAsync();
            if (update is null)
            {
                Debug.WriteLine("ℹ️ [ViewModel] Kein Update verfügbar");
                UpdateVerfuegbar = false;
                UpdateHinweis = "✓ Anwendung ist aktuell";

                if (manuell)
                {
                    Statusmeldung = "✓ Keine neue Version gefunden.";
                }

                return;
            }

            Debug.WriteLine($"✅ [ViewModel] Update verfügbar: v{update.Version}");
            _verfuegbaresUpdate = update;
            UpdateVerfuegbar = true;
            UpdateHinweis = $"⬆ Neue Version verfügbar: {update.Version}";

            var result = MessageBox.Show(
                $"Es ist eine neue Version verfügbar ({update.Version}).\n\nJetzt herunterladen und installieren?",
                "Update verfügbar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                await UpdateStartenAsync();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ [ViewModel] Update-Fehler: {ex.GetType().Name} - {ex.Message}");
            UpdateVerfuegbar = false;
            var grund = string.IsNullOrWhiteSpace(ex.Message) ? "Unbekannter Fehler" : ex.Message;
            UpdateHinweis = $"⚠ Update-Prüfung fehlgeschlagen: {grund}";

            if (manuell)
            {
                Statusmeldung = $"❌ Update-Prüfung fehlgeschlagen: {grund}";
            }
        }
    }

    private async Task UpdateStartenAsync()
    {
        Debug.WriteLine("📥 [ViewModel] UpdateStartenAsync aufgerufen");
        
        if (_verfuegbaresUpdate is null)
        {
            Debug.WriteLine("⚠️ [ViewModel] Kein Update gespeichert, öffne GitHub Releases");
            Process.Start(new ProcessStartInfo(UpdateService.ReleasePageUrl)
            {
                UseShellExecute = true
            });

            Statusmeldung = "ℹ️ Keine installierbare Update-Datei verfügbar. GitHub-Releases wurden geöffnet.";
            return;
        }

        try
        {
            Debug.WriteLine($"⏳ [ViewModel] Lade Update herunter: {_verfuegbaresUpdate.DownloadUrl}");
            Statusmeldung = "⏳ Update wird heruntergeladen...";

            var datenPfad = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ReifeManager",
                "chargen.json");

            if (File.Exists(datenPfad))
            {
                var backupOrdner = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ReifeManager", "Backups");
                Directory.CreateDirectory(backupOrdner);
                var backupPfad = Path.Combine(backupOrdner, $"chargen_backup_{DateTime.Now:yyyyMMdd_HHmmss}.json");
                File.Copy(datenPfad, backupPfad, overwrite: true);
                Debug.WriteLine($"✅ [ViewModel] Backup erstellt: {backupPfad}");
            }

            var installerPfad = await _updateService.LadeUpdateHerunterAsync(_verfuegbaresUpdate);
            Debug.WriteLine($"✅ [ViewModel] Update heruntergeladen: {installerPfad}");
            
            Debug.WriteLine($"🚀 [ViewModel] Starte Installer: {installerPfad}");
            _updateService.StarteInstaller(installerPfad);

            Statusmeldung = "✓ Update gestartet (automatische Installation). Anwendung wird beendet...";
            Application.Current.Shutdown();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ [ViewModel] Update fehlgeschlagen: {ex.GetType().Name} - {ex.Message}");
            Statusmeldung = $"❌ Update fehlgeschlagen: {ex.Message}";
        }
    }

    public void AktualisiereStueckPublic(Charge charge, Fleischstueck stueck, DateTime standDatum)
    {
        AktualisiereStueck(charge, stueck, standDatum);
    }

    public void AktualisiereChargeStatusPublic(Charge charge)
    {
        AktualisiereChargeStatus(charge);
    }

    public void AktualisiereStueckUiPublic()
    {
        OnPropertyChanged(nameof(AktiveStuecke));
    }

    public void SpeichernPublic()
    {
        Speichern();
    }

    public void AktualisiereDiagrammPublic()
    {
        AktualisiereDiagramm();
    }

    private static bool TryParseDouble(string input, out double value)
    {
        input = input.Trim();

        if (double.TryParse(input, NumberStyles.Float, CultureInfo.CurrentCulture, out value))
        {
            return true;
        }

        return double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }
}
