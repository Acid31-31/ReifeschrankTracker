namespace ReifeManager_R01.Models;

public class WochenReportEintrag
{
    public string Woche { get; init; } = string.Empty;
    public string Zeitraum { get; init; } = string.Empty;
    public string Phase { get; init; } = string.Empty;
    public double GewichtsverlustProzent { get; init; }
    public double? TemperaturDurchschnitt { get; init; }
    public double? LuftfeuchteDurchschnitt { get; init; }
    public string Bewertung { get; init; } = string.Empty;
    public string Empfehlung { get; init; } = string.Empty;
}
