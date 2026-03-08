using System.Collections.ObjectModel;
using ReifeManager_R01.Infrastructure;

namespace ReifeManager_R01.Models;

public class Fleischstueck : ObservableObject
{
    public Guid Id { get; set; } = Guid.NewGuid();

    private double _startgewicht;
    public double Startgewicht
    {
        get => _startgewicht;
        set => SetProperty(ref _startgewicht, value);
    }

    public ObservableCollection<MessEintrag> Messungen { get; set; } = new();

    private double _aktuellesGewicht;
    public double AktuellesGewicht
    {
        get => _aktuellesGewicht;
        private set => SetProperty(ref _aktuellesGewicht, value);
    }

    private double _gewichtsverlustProzent;
    public double GewichtsverlustProzent
    {
        get => _gewichtsverlustProzent;
        private set => SetProperty(ref _gewichtsverlustProzent, value);
    }

    private int _reifetage;
    public int Reifetage
    {
        get => _reifetage;
        private set => SetProperty(ref _reifetage, value);
    }

    private double _durchschnittlicherTagesverlust;
    public double DurchschnittlicherTagesverlust
    {
        get => _durchschnittlicherTagesverlust;
        private set => SetProperty(ref _durchschnittlicherTagesverlust, value);
    }

    private ReifeStatus _status = ReifeStatus.Reift;
    public ReifeStatus Status
    {
        get => _status;
        private set => SetProperty(ref _status, value);
    }

    public void SetBerechneteWerte(
        double aktuellesGewicht,
        double gewichtsverlustProzent,
        int reifetage,
        double durchschnittlicherTagesverlust,
        ReifeStatus status)
    {
        AktuellesGewicht = aktuellesGewicht;
        GewichtsverlustProzent = gewichtsverlustProzent;
        Reifetage = reifetage;
        DurchschnittlicherTagesverlust = durchschnittlicherTagesverlust;
        Status = status;
    }
}
