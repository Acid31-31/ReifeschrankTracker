using System.IO;
using ReifeManager_R01.Models;

namespace ReifeManager_R01.Services;

public class PdfExportService
{
    public string ExportierePdf(Charge charge)
    {
        var exportOrdner = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ReifeManager",
            "Exports");
        Directory.CreateDirectory(exportOrdner);

        var dateiname = $"{charge.Bezeichnung.Replace(' ', '_')}_{DateTime.Now:yyyyMMdd_HHmms}.txt";
        var pfad = Path.Combine(exportOrdner, dateiname);

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("═══════════════════════════════════════════════════════");
        sb.AppendLine("               REIFESCHRANK-CHARGENBERICHT");
        sb.AppendLine("═══════════════════════════════════════════════════════");
        sb.AppendLine();

        sb.AppendLine($"CHARGE: {charge.Bezeichnung}");
        sb.AppendLine($"Fleischtyp: {charge.Fleischtyp}");
        sb.AppendLine($"Startdatum: {charge.Startdatum:dd.MM.yyyy}");
        sb.AppendLine($"Reifedauer: {(DateTime.Today - charge.Startdatum).Days} Tage");
        sb.AppendLine($"Zielverlust: {charge.ZielverlustProzent:F1}%");
        sb.AppendLine($"Status: {charge.StatusUebersicht}");
        sb.AppendLine($"Anzahl Stücke: {charge.Stuecke.Count}");
        sb.AppendLine();

        sb.AppendLine("───────────────────────────────────────────────────────");
        sb.AppendLine("STÜCKE:");
        sb.AppendLine("───────────────────────────────────────────────────────");

        for (int i = 0; i < charge.Stuecke.Count; i++)
        {
            var stueck = charge.Stuecke[i];
            sb.AppendLine();
            sb.AppendLine($"Stück {i + 1}:");
            sb.AppendLine($"  ID: {stueck.Id}");
            sb.AppendLine($"  Startgewicht: {stueck.Startgewicht:F0}g");
            sb.AppendLine($"  Aktuelles Gewicht: {stueck.AktuellesGewicht:F0}g");
            sb.AppendLine($"  Gewichtsverlust: {stueck.GewichtsverlustProzent:F2}%");
            sb.AppendLine($"  Reifetage: {stueck.Reifetage}");
            sb.AppendLine($"  Ø Tagesverlust: {stueck.DurchschnittlicherTagesverlust:F2}%");
            sb.AppendLine($"  Status: {stueck.Status}");
            sb.AppendLine($"  Messungen: {stueck.Messungen.Count}");

            if (stueck.Messungen.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("  Messverlauf:");
                foreach (var messung in stueck.Messungen.OrderBy(m => m.Datum))
                {
                    sb.AppendLine($"    {messung.Datum:dd.MM.yyyy}: {messung.Gewicht:F0}g | {messung.Temperatur:F1}°C | {messung.Luftfeuchte:F1}% | {messung.Notiz}");
                }
            }
        }

        sb.AppendLine();
        sb.AppendLine("═══════════════════════════════════════════════════════");
        sb.AppendLine($"Bericht erstellt: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
        sb.AppendLine("═══════════════════════════════════════════════════════");

        File.WriteAllText(pfad, sb.ToString(), System.Text.Encoding.UTF8);
        return pfad;
    }
}
