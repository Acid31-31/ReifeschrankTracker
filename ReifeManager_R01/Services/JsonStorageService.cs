using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using ReifeManager_R01.Models;

namespace ReifeManager_R01.Services;

public class JsonStorageService
{
    private readonly string _dateiPfad;
    private readonly JsonSerializerOptions _options;

    public JsonStorageService()
    {
        var appDataVerzeichnis = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ReifeManager");
        Directory.CreateDirectory(appDataVerzeichnis);

        _dateiPfad = Path.Combine(appDataVerzeichnis, "chargen.json");

        var alterPfad = Path.Combine(AppContext.BaseDirectory, "Data", "chargen.json");
        if (!File.Exists(_dateiPfad) && File.Exists(alterPfad))
        {
            File.Copy(alterPfad, _dateiPfad, overwrite: false);
        }

        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public List<Charge> Laden()
    {
        if (!File.Exists(_dateiPfad))
        {
            return new List<Charge>();
        }

        var json = File.ReadAllText(_dateiPfad);
        var chargen = JsonSerializer.Deserialize<List<Charge>>(json, _options) ?? new List<Charge>();

        // Datenmigration: auto-generierte Standard-Notiz "Reifung läuft gut" entfernen
        foreach (var charge in chargen)
        {
            foreach (var stueck in charge.Stuecke)
            {
                foreach (var messung in stueck.Messungen)
                {
                    if (string.Equals(messung.Notiz?.Trim(), "Reifung läuft gut", StringComparison.OrdinalIgnoreCase))
                    {
                        messung.Notiz = string.Empty;
                    }
                }
            }
        }

        return chargen;
    }

    public void Speichern(IEnumerable<Charge> chargen)
    {
        var json = JsonSerializer.Serialize(chargen, _options);
        File.WriteAllText(_dateiPfad, json);
    }
}
