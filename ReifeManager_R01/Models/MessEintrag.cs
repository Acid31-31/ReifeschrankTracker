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

    private double VerlustSeitLetzterMessung
    {
        get
        {
            if (VorherigesGewicht <= 0 || Gewicht <= 0)
            {
                return 0;
            }
            return ((VorherigesGewicht - Gewicht) / VorherigesGewicht) * 100.0;
        }
    }

    public bool IstKritisch => VerlustSeitLetzterMessung > 8.0;

    public string StatusAnzeige
    {
        get
        {
            var verlust = VerlustSeitLetzterMessung;

            if (verlust > 8.0)
            {
                return "⚠️ Zu schnell getrocknet";
            }

            if (TageSeit >= 7 && verlust < 0.5)
            {
                return "⏳ Zu langsam";
            }

            if (verlust > 3.5)
            {
                return "⚠️ Verlust erhöht";
            }

            if (VorherigesGewicht <= 0)
            {
                return "–";
            }

            return "✅ OK";
        }
    }
}
