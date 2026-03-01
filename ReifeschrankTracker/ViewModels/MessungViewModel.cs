using ReifeschrankTracker.Models;
using System;

namespace ReifeschrankTracker.ViewModels;

public class MessungViewModel : ViewModelBase
{
    private readonly Messung _messung;
    private readonly int _startgewicht;

    public MessungViewModel(Messung messung, int startgewicht)
    {
        _messung = messung;
        _startgewicht = startgewicht;
    }

    public Guid Id => _messung.Id;
    public DateTime Zeitpunkt => _messung.Zeitpunkt;
    public int GewichtG => _messung.GewichtG;
    public string? Notiz => _messung.Notiz;
    public int VerlustG => _startgewicht - _messung.GewichtG;
    public decimal VerlustProzent => _startgewicht > 0
        ? Math.Round((decimal)VerlustG / _startgewicht * 100, 1)
        : 0m;
}
