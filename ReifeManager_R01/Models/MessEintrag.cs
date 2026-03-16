namespace ReifeManager_R01.Models;

public class MessEintrag
{
    public DateTime Datum { get; set; } = DateTime.Today;
    public double Gewicht { get; set; }
    public double Temperatur { get; set; }
    public double Luftfeuchte { get; set; }
    public string SollProzess { get; set; } = string.Empty;
    public string Prozess { get; set; } = string.Empty;
    public string Notiz { get; set; } = string.Empty;

    public double VorherigesGewicht { get; set; }
    public DateTime? VorherigesDatum { get; set; }

    public int TageSeit => VorherigesDatum.HasValue
        ? Math.Max(0, (Datum.Date - VorherigesDatum.Value.Date).Days)
        : 0;

    public string TageSeitAnzeige => TageSeit > 0 ? $"{TageSeit} Tage" : "–";

    public bool IstKritisch
    {
        get
        {
            if (VorherigesGewicht <= 0 || Gewicht <= 0)
            {
                return false;
            }
            var wochenVerlust = ((VorherigesGewicht - Gewicht) / VorherigesGewicht) * 100.0;
            return wochenVerlust > 8.0;
        }
    }
}
