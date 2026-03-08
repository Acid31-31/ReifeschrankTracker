using System.Collections.ObjectModel;
using ReifeManager_R01.Infrastructure;

namespace ReifeManager_R01.Models;

public class Charge : ObservableObject
{
    public Guid Id { get; set; } = Guid.NewGuid();

    private string _bezeichnung = string.Empty;
    public string Bezeichnung
    {
        get => _bezeichnung;
        set => SetProperty(ref _bezeichnung, value);
    }

    private string _fleischtyp = string.Empty;
    public string Fleischtyp
    {
        get => _fleischtyp;
        set => SetProperty(ref _fleischtyp, value);
    }

    private DateTime _startdatum = DateTime.Today;
    public DateTime Startdatum
    {
        get => _startdatum;
        set => SetProperty(ref _startdatum, value);
    }

    private double _zielverlustProzent = 30;
    public double ZielverlustProzent
    {
        get => _zielverlustProzent;
        set => SetProperty(ref _zielverlustProzent, value);
    }

    private string _herstellungsProfil = "Standard";
    public string HerstellungsProfil
    {
        get => _herstellungsProfil;
        set => SetProperty(ref _herstellungsProfil, value);
    }

    private int _poekelnTage;
    public int PoekelnTage
    {
        get => _poekelnTage;
        set => SetProperty(ref _poekelnTage, value);
    }

    private int _abbrennenTage;
    public int AbbrennenTage
    {
        get => _abbrennenTage;
        set => SetProperty(ref _abbrennenTage, value);
    }

    private int _raeuchernTage;
    public int RaeuchernTage
    {
        get => _raeuchernTage;
        set => SetProperty(ref _raeuchernTage, value);
    }

    private int _reifenTage;
    public int ReifenTage
    {
        get => _reifenTage;
        set => SetProperty(ref _reifenTage, value);
    }

    private Rezept? _rezept;
    public Rezept? Rezept
    {
        get => _rezept;
        set => SetProperty(ref _rezept, value);
    }

    public ObservableCollection<Fleischstueck> Stuecke { get; set; } = new();

    private string _statusUebersicht = "Keine Stücke";
    public string StatusUebersicht
    {
        get => _statusUebersicht;
        set => SetProperty(ref _statusUebersicht, value);
    }
}
