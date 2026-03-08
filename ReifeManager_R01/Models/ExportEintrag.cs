namespace ReifeManager_R01.Models;

public class ExportEintrag
{
    public DateTime Zeitpunkt { get; init; } = DateTime.Now;
    public string Typ { get; init; } = string.Empty;
    public string Dateiname { get; init; } = string.Empty;
    public string Pfad { get; init; } = string.Empty;
}
