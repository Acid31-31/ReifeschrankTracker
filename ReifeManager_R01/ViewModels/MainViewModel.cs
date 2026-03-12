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

    public ObservableCollection<string> ProzessOptionen { get; } = new() { "Pökeln", "Abbrennen", "Räuchern", "Reifen" };

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
            
            var profilVorschau = new Charge();
            _reifePlanService.AnwendenProfilName(profilVorschau, _ausgewaehltesProfil);
            NeuePoekelnTage = profilVorschau.PoekelnTage.ToString(CultureInfo.CurrentCulture);
            NeueAbbrennenTage = profilVorschau.AbbrennenTage.ToString(CultureInfo.CurrentCulture);
            NeueRaeuchernTage = profilVorschau.RaeuchernTage.ToString(CultureInfo.CurrentCulture);
            NeueReifenTage = profilVorschau.ReifenTage.ToString(CultureInfo.CurrentCulture);

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
            AktualisiereWochenReport();
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
            if (string.IsNullOrWhiteSpace(_selectedMessung.SollProzess))
            {
                _selectedMessung.SollProzess = ErmittleSollProzess(_selectedMessung.Datum);
            }
            NeuerMessProzess = string.IsNullOrWhiteSpace(_selectedMessung.Prozess)
                ? _selectedMessung.SollProzess
                : _selectedMessung.Prozess;
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

    private string _neuePoekelnTage = "10";
    public string NeuePoekelnTage
    {
        get => _neuePoekelnTage;
        set => SetProperty(ref _neuePoekelnTage, value);
    }

    private string _neueAbbrennenTage = "1";
    public string NeueAbbrennenTage
    {
        get => _neueAbbrennenTage;
        set => SetProperty(ref _neueAbbrennenTage, value);
    }

    private string _neueRaeuchernTage = "0";
    public string NeueRaeuchernTage
    {
        get => _neueRaeuchernTage;
        set => SetProperty(ref _neueRaeuchernTage, value);
    }

    private string _neueReifenTage = "30";
    public string NeueReifenTage
    {
        get => _neueReifenTage;
        set => SetProperty(ref _neueReifenTage, value);
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
        set
        {
            if (!SetProperty(ref _neuesMessdatum, value))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(NeuerMessProzess))
            {
                NeuerMessProzess = ErmittleSollProzess(_neuesMessdatum);
            }
        }
    }

    private string _neuesMessgewicht = string.Empty;
    public string NeuesMessgewicht
    {
        get => _neuesMessgewicht;
        set => SetProperty(ref _neuesMessgewicht, value);
    }

    private string _neuerMessProzess = string.Empty;
    public string NeuerMessProzess
    {
        get => _neuerMessProzess;
        set => SetProperty(ref _neuerMessProzess, value);
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
    public ObservableCollection<WochenReportEintrag> WochenReport { get; } = new();

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

        NeuerMessProzess = ErmittleSollProzess(NeuesMessdatum);

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
            !TryParseInt(NeuePoekelnTage, out var poekelnTage) ||
            !TryParseInt(NeueAbbrennenTage, out var abbrennenTage) ||
            !TryParseInt(NeueRaeuchernTage, out var raeuchernTage) ||
            !TryParseInt(NeueReifenTage, out var reifenTage) ||
            poekelnTage < 0 || abbrennenTage < 0 || raeuchernTage < 0 || reifenTage < 0 ||
            !ValidationHelper.IsValidName(NeueBezeichnung) ||
            !ValidationHelper.IsValidLossPercentage(zielverlustProzent))
        {
            Statusmeldung = "❌ Ungültige Eingabe. Bitte Chargendaten und Zeiten prüfen.";
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
            HerstellungsProfil = AusgewaehltesProfil,
            PoekelnTage = poekelnTage,
            AbbrennenTage = abbrennenTage,
            RaeuchernTage = raeuchernTage,
            ReifenTage = reifenTage,
            StatusUebersicht = "Keine Stücke"
        };

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
                Statusmeldung = $"✓ Rezept '{charge.Rezept.Name}' ausgewählt.";
            }
            else
            {
                Statusmeldung = "ℹ️ Kein Rezept ausgewählt — manuelle Zeiten werden verwendet.";
            }
        }

        Chargen.Add(charge);
        SelectedCharge = charge;

        NeueBezeichnung = ErzeugeNaechsteBezeichnung(AusgewaehltesProfil);
        NeuerFleischtyp = ProfilEmpfehlung;
        NeuesStartdatum = DateTime.Today;
        NeuesZielverlustProzent = _reifePlanService.HoleEmpfohlenenZielverlust(AusgewaehltesProfil).ToString("F0", CultureInfo.CurrentCulture);

        var profilVorschau = new Charge();
        _reifePlanService.AnwendenProfilName(profilVorschau, AusgewaehltesProfil);
        NeuePoekelnTage = profilVorschau.PoekelnTage.ToString(CultureInfo.CurrentCulture);
        NeueAbbrennenTage = profilVorschau.AbbrennenTage.ToString(CultureInfo.CurrentCulture);
        NeueRaeuchernTage = profilVorschau.RaeuchernTage.ToString(CultureInfo.CurrentCulture);
        NeueReifenTage = profilVorschau.ReifenTage.ToString(CultureInfo.CurrentCulture);

        Statusmeldung = $"✓ Charge '{charge.Bezeichnung}' angelegt ({AusgewaehltesProfil}).";
        Speichern();
        AktualisiereWochenReport();
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

        var sollProzess = ErmittleSollProzess(NeuesMessdatum);
        var istProzess = NormalisiereProzess(NeuerMessProzess, sollProzess);

        var messung = new MessEintrag
        {
            Datum = NeuesMessdatum,
            Gewicht = messgewicht,
            Temperatur = temperatur,
            Luftfeuchte = luftfeuchte,
            SollProzess = sollProzess,
            Prozess = istProzess,
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
        NeuerMessProzess = ErmittleSollProzess(DateTime.Today);
        SelectedMessung = messung;

        Statusmeldung = $"✓ Messung vom {NeuesMessdatum:dd.MM.yyyy} gespeichert ({messgewicht:F0}g, {istProzess}).";
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
        SelectedMessung.SollProzess = ErmittleSollProzess(NeuesMessdatum);
        SelectedMessung.Prozess = NormalisiereProzess(NeuerMessProzess, SelectedMessung.SollProzess);
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

        SelectedMessung.SollProzess = ErmittleSollProzess(SelectedMessung.Datum);
        SelectedMessung.Prozess = NormalisiereProzess(SelectedMessung.Prozess, SelectedMessung.SollProzess);

        var bezug = SelectedStueck.Messungen.OrderByDescending(m => m.Datum).FirstOrDefault()?.Datum ?? DateTime.Today;
        AktualisiereStueck(SelectedCharge, SelectedStueck, bezug);
        AktualisiereChargeStatus(SelectedCharge);
        OnPropertyChanged(nameof(AktiveMessungen));

        NeuesMessdatum = SelectedMessung.Datum;
        NeuesMessgewicht = SelectedMessung.Gewicht.ToString("F0", CultureInfo.CurrentCulture);
        NeueTemperatur = SelectedMessung.Temperatur.ToString("F1", CultureInfo.CurrentCulture);
        NeueLuftfeuchte = SelectedMessung.Luftfeuchte.ToString("F1", CultureInfo.CurrentCulture);
        NeuerMessProzess = SelectedMessung.Prozess;
        NeueNotiz = SelectedMessung.Notiz;

        Statusmeldung = "✓ Messung per Doppelklick bearbeitet und gespeichert.";
        Speichern();
        AktualisiereDiagramm();
    }

    private void AktualisiereWochenReport()
    {
        WochenReport.Clear();

        if (SelectedCharge is null)
        {
            return;
        }

        var charge = SelectedCharge;
        var alleMessungen = charge.Stuecke.SelectMany(s => s.Messungen).OrderBy(m => m.Datum).ToList();

        var planTage = Math.Max(1, charge.PoekelnTage + charge.AbbrennenTage + charge.RaeuchernTage + charge.ReifenTage);
        var maxMessTag = alleMessungen.Count == 0 ? 0 : Math.Max(0, (alleMessungen.Max(m => m.Datum).Date - charge.Startdatum.Date).Days + 1);
        var gesamtTage = Math.Max(planTage, maxMessTag);
        var anzahlWochen = Math.Max(1, (int)Math.Ceiling(gesamtTage / 7.0));

        for (var woche = 1; woche <= anzahlWochen; woche++)
        {
            var von = charge.Startdatum.Date.AddDays((woche - 1) * 7);
            var bis = von.AddDays(6);

            var wochenMessungen = alleMessungen
                .Where(m => m.Datum.Date >= von && m.Datum.Date <= bis)
                .ToList();

            var verlustListe = new List<double>();
            foreach (var stueck in charge.Stuecke)
            {
                var sortiert = stueck.Messungen.OrderBy(m => m.Datum).ToList();
                if (sortiert.Count == 0)
                {
                    continue;
                }

                var startMessung = sortiert.LastOrDefault(m => m.Datum.Date <= von) ?? sortiert.FirstOrDefault(m => m.Datum.Date >= von && m.Datum.Date <= bis);
                var endMessung = sortiert.LastOrDefault(m => m.Datum.Date <= bis);

                if (startMessung is null || endMessung is null || startMessung.Gewicht <= 0 || endMessung.Datum < startMessung.Datum)
                {
                    continue;
                }

                var wochenVerlust = ((startMessung.Gewicht - endMessung.Gewicht) / startMessung.Gewicht) * 100.0;
                verlustListe.Add(wochenVerlust);
            }

            var verlust = verlustListe.Count > 0 ? verlustListe.Average() : 0.0;
            var temp = wochenMessungen.Count > 0 ? wochenMessungen.Average(m => m.Temperatur) : (double?)null;
            var luft = wochenMessungen.Count > 0 ? wochenMessungen.Average(m => m.Luftfeuchte) : (double?)null;

            var phase = ErmittlePhase(charge, (woche - 1) * 7);
            var sollProzess = phase;
            var istProzess = wochenMessungen
                .Select(m => string.IsNullOrWhiteSpace(m.Prozess) ? m.SollProzess : m.Prozess)
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .GroupBy(p => p)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? sollProzess;

            var warnung = string.Equals(sollProzess, istProzess, StringComparison.OrdinalIgnoreCase)
                ? "OK"
                : $"Soll: {sollProzess}, Ist: {istProzess}";

            var (bewertung, empfehlung) = BewerteWochenVerlauf(verlust, phase);

            WochenReport.Add(new WochenReportEintrag
            {
                Woche = $"W{woche}",
                Zeitraum = $"{von:dd.MM} - {bis:dd.MM}",
                Phase = phase,
                SollProzess = sollProzess,
                IstProzess = istProzess,
                ProzessWarnung = warnung,
                GewichtsverlustProzent = verlust,
                TemperaturDurchschnitt = temp,
                LuftfeuchteDurchschnitt = luft,
                Bewertung = bewertung,
                Empfehlung = empfehlung
            });
        }
    }

    private static string ErmittlePhase(Charge charge, int tageSeitStart)
    {
        if (tageSeitStart < charge.PoekelnTage)
        {
            return "Pökeln";
        }

        tageSeitStart -= charge.PoekelnTage;
        if (tageSeitStart < charge.AbbrennenTage)
        {
            return "Abbrennen";
        }

        tageSeitStart -= charge.AbbrennenTage;
        if (tageSeitStart < charge.RaeuchernTage)
        {
            return "Räuchern";
        }

        return "Reifen";
    }

    private static (string Bewertung, string Empfehlung) BewerteWochenVerlauf(double verlustProzent, string phase)
    {
        if (verlustProzent >= 4.0)
        {
            return ("Zu schnell", "Luftfeuchte erhöhen (+3-5%) / Temperatur leicht senken");
        }

        if (phase == "Reifen" && verlustProzent <= 0.5)
        {
            return ("Zu langsam", "Luftfeuchte leicht senken oder Luftzirkulation erhöhen");
        }

        return ("Gut", "Verlauf im Zielbereich");
    }

    private void AktualisiereDiagramm()
    {
        AktualisiereWochenReport();
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

        for ( int i = 0; i < messungen.Count; i++)
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
            
            if (manuell)
            {
                Statusmeldung = "⏳ Prüfe auf Updates...";
            }

            var currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";
            var update = await _updateService.PruefeAufUpdateAsync();
            
            // Zeige Update-Status-Fenster wenn manuell geprüft
            if (manuell)
            {
                var statusWindow = new Views.UpdateCheckWindow(currentVersion, update)
                {
                    Owner = Application.Current.MainWindow
                };
                statusWindow.ShowDialog();
                
                if (update is not null)
                {
                    var result = MessageBox.Show(
                        $"Neue Version v{update.Version} verfügbar!\n\nJetzt herunterladen und installieren?",
                        "Update verfügbar",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);

                    if (result == MessageBoxResult.Yes)
                    {
                        _verfuegbaresUpdate = update;
                        await UpdateStartenAsync();
                    }
                }
                return;
            }

            // Automatische Prüfung im Hintergrund
            if (update is null)
            {
                Debug.WriteLine("ℹ️ [ViewModel] Kein Update verfügbar");
                UpdateVerfuegbar = false;
                UpdateHinweis = "✓ Anwendung ist aktuell";
                return;
            }

            Debug.WriteLine($"✅ [ViewModel] Update verfügbar: v{update.Version}");
            _verfuegbaresUpdate = update;
            UpdateVerfuegbar = true;
            UpdateHinweis = $"⬆ Neue Version verfügbar: {update.Version}";

            var autoResult = MessageBox.Show(
                $"Es ist eine neue Version verfügbar ({update.Version}).\n\nJetzt herunterladen und installieren?",
                "Update verfügbar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information);

            if (autoResult == MessageBoxResult.Yes)
            {
                await UpdateStartenAsync();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ [ViewModel] Update-Fehler: {ex.GetType().Name} - {ex.Message}");
            UpdateVerfuegbar = false;
            var grund = string.IsNullOrWhiteSpace(ex.Message) ? "Unbekannter Fehler" : ex.Message;
            UpdateHinweis = $"⚠ Update-Prüfung fehlgeschlagen";

            if (manuell)
            {
                Statusmeldung = $"❌ Update-Fehler: {grund}";
                MessageBox.Show(
                    $"Update-Prüfung fehlgeschlagen:\n\n{grund}\n\nBitte überprüfen Sie Ihre Internetverbindung.",
                    "Update-Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
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

    private static bool TryParseInt(string input, out int value)
    {
        input = input.Trim();

        if (int.TryParse(input, NumberStyles.Integer, CultureInfo.CurrentCulture, out value))
        {
            return true;
        }

        return int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
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

    private string ErmittleSollProzess(DateTime datum)
    {
        if (SelectedCharge is null)
        {
            return "Reifen";
        }

        var tageSeitStart = Math.Max(0, (datum.Date - SelectedCharge.Startdatum.Date).Days);
        return ErmittlePhase(SelectedCharge, tageSeitStart);
    }

    private string NormalisiereProzess(string? eingabe, string fallback)
    {
        var kandidat = (eingabe ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(kandidat))
        {
            return fallback;
        }

        return ProzessOptionen.Any(p => string.Equals(p, kandidat, StringComparison.OrdinalIgnoreCase))
            ? ProzessOptionen.First(p => string.Equals(p, kandidat, StringComparison.OrdinalIgnoreCase))
            : fallback;
    }
}
