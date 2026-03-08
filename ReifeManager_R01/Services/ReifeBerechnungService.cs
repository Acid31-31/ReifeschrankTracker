using ReifeManager_R01.Models;

namespace ReifeManager_R01.Services;

public class ReifeBerechnungService
{
    public double BerechneGewichtsverlust(double startgewicht, double aktuellesGewicht)
    {
        if (startgewicht <= 0)
        {
            return 0;
        }

        return Math.Max(0, ((startgewicht - aktuellesGewicht) / startgewicht) * 100);
    }

    public int BerechneReifetage(DateTime startdatum, DateTime datum)
    {
        var tage = (datum.Date - startdatum.Date).Days;
        return Math.Max(0, tage);
    }

    public double BerechneDurchschnittlichenTagesverlust(double gewichtsverlustProzent, int reifetage)
    {
        if (reifetage <= 0)
        {
            return 0;
        }

        return gewichtsverlustProzent / reifetage;
    }

    public ReifeStatus BerechneStatus(
        double startgewicht,
        double aktuellesGewicht,
        double zielverlustProzent,
        DateTime startdatum,
        DateTime standDatum)
    {
        var verlust = BerechneGewichtsverlust(startgewicht, aktuellesGewicht);
        var tage = BerechneReifetage(startdatum, standDatum);
        var durchschnitt = BerechneDurchschnittlichenTagesverlust(verlust, tage);

        if (durchschnitt > 1)
        {
            return ReifeStatus.Kritisch;
        }

        if (verlust >= 35)
        {
            return ReifeStatus.Warnung;
        }

        if (verlust >= zielverlustProzent)
        {
            return ReifeStatus.Fertig;
        }

        return ReifeStatus.Reift;
    }
}
