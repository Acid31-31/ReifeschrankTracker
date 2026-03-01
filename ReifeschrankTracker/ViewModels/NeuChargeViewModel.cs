using ReifeschrankTracker.Models;
using System;
using System.Collections.Generic;

namespace ReifeschrankTracker.ViewModels;

public class NeuChargeViewModel : ViewModelBase
{
    private string _produktname = string.Empty;
    private string _chargeCode = string.Empty;
    private Methode _methode = Methode.Lufttrocknen;
    private DateTime _startdatum = DateTime.Now;
    private string _startgewicht = string.Empty;
    private ZielTyp _zielTyp = ZielTyp.Prozent;
    private string _zielProzent = string.Empty;
    private string _zielGewicht = string.Empty;
    private string _notizen = string.Empty;
    private string _fehler = string.Empty;

    public string Produktname { get => _produktname; set => SetField(ref _produktname, value); }
    public string ChargeCode { get => _chargeCode; set => SetField(ref _chargeCode, value); }
    public Methode Methode { get => _methode; set => SetField(ref _methode, value); }
    public DateTime Startdatum { get => _startdatum; set => SetField(ref _startdatum, value); }
    public string StartgewichtText { get => _startgewicht; set => SetField(ref _startgewicht, value); }
    public ZielTyp ZielTyp { get => _zielTyp; set { SetField(ref _zielTyp, value); OnPropertyChanged(nameof(IstZielProzent)); OnPropertyChanged(nameof(IstZielGewicht)); } }
    public string ZielProzentText { get => _zielProzent; set => SetField(ref _zielProzent, value); }
    public string ZielGewichtText { get => _zielGewicht; set => SetField(ref _zielGewicht, value); }
    public string Notizen { get => _notizen; set => SetField(ref _notizen, value); }
    public string Fehler { get => _fehler; set => SetField(ref _fehler, value); }

    public bool IstZielProzent => _zielTyp == ZielTyp.Prozent;
    public bool IstZielGewicht => _zielTyp == ZielTyp.Gewicht;

    public IEnumerable<Methode> MethodenWerte => Enum.GetValues<Methode>();
    public IEnumerable<ZielTyp> ZielTypWerte => Enum.GetValues<ZielTyp>();

    private static bool TryParseDecimal(string text, out decimal value)
        => decimal.TryParse(text.Replace(',', '.'), System.Globalization.NumberStyles.Number,
            System.Globalization.CultureInfo.InvariantCulture, out value);

    public bool Validieren()
    {
        Fehler = string.Empty;
        if (string.IsNullOrWhiteSpace(Produktname)) { Fehler = "Produktname ist erforderlich."; return false; }
        if (!int.TryParse(StartgewichtText, out var g) || g <= 0) { Fehler = "Startgewicht muss eine positive Zahl sein."; return false; }
        if (ZielTyp == ZielTyp.Prozent)
        {
            if (!TryParseDecimal(ZielProzentText, out var p) || p <= 0) { Fehler = "Ziel-% muss eine positive Zahl sein."; return false; }
        }
        else
        {
            if (!int.TryParse(ZielGewichtText, out var zg) || zg <= 0) { Fehler = "Zielgewicht muss eine positive Zahl sein."; return false; }
        }
        return true;
    }

    public Charge ToCharge()
    {
        int.TryParse(StartgewichtText, out var g);
        TryParseDecimal(ZielProzentText, out var p);
        int.TryParse(ZielGewichtText, out var zg);
        return new Charge
        {
            Produktname = Produktname.Trim(),
            ChargeCode = string.IsNullOrWhiteSpace(ChargeCode) ? null : ChargeCode.Trim(),
            Methode = Methode,
            Startdatum = Startdatum,
            StartgewichtG = g,
            ZielTyp = ZielTyp,
            ZielProzent = ZielTyp == ZielTyp.Prozent ? p : null,
            ZielGewichtG = ZielTyp == ZielTyp.Gewicht ? zg : null,
            Notizen = string.IsNullOrWhiteSpace(Notizen) ? null : Notizen.Trim()
        };
    }
}
