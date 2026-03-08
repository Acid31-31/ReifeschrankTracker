using System.Globalization;
using System.IO;
using System.Text;
using ReifeManager_R01.Models;

namespace ReifeManager_R01.Services;

public class CsvExportService
{
    public string ExportiereCharge(Charge charge)
    {
        var exportOrdner = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ReifeManager",
            "Exports");
        Directory.CreateDirectory(exportOrdner);

        var sichereBezeichnung = ErzeugeSicherenDateinamen(charge.Bezeichnung);
        var dateiname = $"{sichereBezeichnung}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
        var pfad = Path.Combine(exportOrdner, dateiname);

        var sb = new StringBuilder();
        sb.AppendLine("sep=;");
        sb.AppendLine("ChargeId;Bezeichnung;Fleischtyp;Startdatum;ZielverlustProzent;StueckId;Messdatum;Gewicht;Temperatur;Luftfeuchte;Notiz");

        foreach (var stueck in charge.Stuecke)
        {
            if (stueck.Messungen.Count == 0)
            {
                sb.AppendLine(string.Join(';',
                    charge.Id,
                    Esc(charge.Bezeichnung),
                    Esc(charge.Fleischtyp),
                    charge.Startdatum.ToString("yyyy-MM-dd"),
                    charge.ZielverlustProzent.ToString(CultureInfo.InvariantCulture),
                    stueck.Id,
                    string.Empty,
                    stueck.AktuellesGewicht.ToString(CultureInfo.InvariantCulture),
                    string.Empty,
                    string.Empty,
                    string.Empty));
                continue;
            }

            foreach (var messung in stueck.Messungen.OrderBy(m => m.Datum))
            {
                sb.AppendLine(string.Join(';',
                    charge.Id,
                    Esc(charge.Bezeichnung),
                    Esc(charge.Fleischtyp),
                    charge.Startdatum.ToString("yyyy-MM-dd"),
                    charge.ZielverlustProzent.ToString(CultureInfo.InvariantCulture),
                    stueck.Id,
                    messung.Datum.ToString("yyyy-MM-dd"),
                    messung.Gewicht.ToString(CultureInfo.InvariantCulture),
                    messung.Temperatur.ToString(CultureInfo.InvariantCulture),
                    messung.Luftfeuchte.ToString(CultureInfo.InvariantCulture),
                    Esc(messung.Notiz)));
            }
        }

        File.WriteAllText(pfad, sb.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
        return pfad;
    }

    private static string ErzeugeSicherenDateinamen(string? name)
    {
        var basis = string.IsNullOrWhiteSpace(name) ? "Charge" : name.Trim();
        foreach (var zeichen in Path.GetInvalidFileNameChars())
        {
            basis = basis.Replace(zeichen, '_');
        }

        return basis.Replace(' ', '_');
    }

    private static string Esc(string text)
    {
        return string.IsNullOrWhiteSpace(text)
            ? string.Empty
            : text.Replace(";", ",").Replace(Environment.NewLine, " ").Trim();
    }
}
