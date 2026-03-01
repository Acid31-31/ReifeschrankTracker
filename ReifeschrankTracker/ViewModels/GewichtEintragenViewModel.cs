using ReifeschrankTracker.Models;
using System;

namespace ReifeschrankTracker.ViewModels;

public class GewichtEintragenViewModel : ViewModelBase
{
    private string _gewicht = string.Empty;
    private DateTime _zeitpunkt = DateTime.Now;
    private string _notiz = string.Empty;
    private string _fehler = string.Empty;

    public string GewichtText { get => _gewicht; set => SetField(ref _gewicht, value); }
    public DateTime Zeitpunkt { get => _zeitpunkt; set => SetField(ref _zeitpunkt, value); }
    public string Notiz { get => _notiz; set => SetField(ref _notiz, value); }
    public string Fehler { get => _fehler; set => SetField(ref _fehler, value); }

    public bool Validieren()
    {
        Fehler = string.Empty;
        if (!int.TryParse(GewichtText, out var g) || g <= 0) { Fehler = "Gewicht muss eine positive Zahl (in Gramm) sein."; return false; }
        return true;
    }

    public Messung ToMessung(Guid chargeId)
    {
        int.TryParse(GewichtText, out var g);
        return new Messung
        {
            ChargeId = chargeId,
            Zeitpunkt = Zeitpunkt,
            GewichtG = g,
            Notiz = string.IsNullOrWhiteSpace(Notiz) ? null : Notiz.Trim()
        };
    }
}
