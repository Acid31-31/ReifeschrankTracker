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
}
