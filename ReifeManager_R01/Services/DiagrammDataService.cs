using ReifeManager_R01.Models;

namespace ReifeManager_R01.Services;

public class DiagrammDataService
{
    public List<(DateTime Datum, double Gewicht)> GetGewichtverlauf(Fleischstueck stueck)
    {
        var result = new List<(DateTime, double)>();

        if (stueck.Messungen.Count == 0)
        {
            return result;
        }

        result.Add((DateTime.Today.AddDays(-1), stueck.Startgewicht));

        foreach (var messung in stueck.Messungen.OrderBy(m => m.Datum))
        {
            result.Add((messung.Datum, messung.Gewicht));
        }

        return result;
    }

    public List<(DateTime Datum, double Verlust)> GetVerlustprozent(Fleischstueck stueck)
    {
        var result = new List<(DateTime, double)>();

        if (stueck.Messungen.Count == 0)
        {
            return result;
        }

        foreach (var messung in stueck.Messungen.OrderBy(m => m.Datum))
        {
            var verlust = ((stueck.Startgewicht - messung.Gewicht) / stueck.Startgewicht) * 100;
            result.Add((messung.Datum, verlust));
        }

        return result;
    }

    public List<(DateTime Datum, double Temperatur)> GetTemperaturverlauf(Fleischstueck stueck)
    {
        var result = new List<(DateTime, double)>();

        foreach (var messung in stueck.Messungen.OrderBy(m => m.Datum))
        {
            result.Add((messung.Datum, messung.Temperatur));
        }

        return result;
    }

    public List<(DateTime Datum, double Luftfeuchte)> GetLuftfeuchteverlauf(Fleischstueck stueck)
    {
        var result = new List<(DateTime, double)>();

        foreach (var messung in stueck.Messungen.OrderBy(m => m.Datum))
        {
            result.Add((messung.Datum, messung.Luftfeuchte));
        }

        return result;
    }
}
