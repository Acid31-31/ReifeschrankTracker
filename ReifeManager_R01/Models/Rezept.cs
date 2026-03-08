using ReifeManager_R01.Infrastructure;

namespace ReifeManager_R01.Models;

public class Rezept : ObservableObject
{
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private string _profil = string.Empty;
    public string Profil
    {
        get => _profil;
        set => SetProperty(ref _profil, value);
    }

    private string _beschreibung = string.Empty;
    public string Beschreibung
    {
        get => _beschreibung;
        set => SetProperty(ref _beschreibung, value);
    }

    private string _zutaten = string.Empty;
    public string Zutaten
    {
        get => _zutaten;
        set => SetProperty(ref _zutaten, value);
    }

    private string _anleitung = string.Empty;
    public string Anleitung
    {
        get => _anleitung;
        set => SetProperty(ref _anleitung, value);
    }

    public string BerechneZutatenFuerGewicht(double gewichtInGramm)
    {
        var kg = gewichtInGramm / 1000.0;
        var zeilen = Zutaten.Split('\n');
        var berechnet = new System.Text.StringBuilder();

        foreach (var zeile in zeilen)
        {
            if (string.IsNullOrWhiteSpace(zeile))
            {
                berechnet.AppendLine();
                continue;
            }

            if (zeile.StartsWith("Pro kg") || zeile.StartsWith("pro kg", StringComparison.OrdinalIgnoreCase))
            {
                berechnet.AppendLine($"Für {gewichtInGramm:F0}g ({kg:F2}kg):");
                continue;
            }

            var match = System.Text.RegularExpressions.Regex.Match(zeile, @"(\d+(?:[.,]\d+)?)\s*(g|kg|ml|l)\s+(.+)");
            if (match.Success)
            {
                var menge = double.Parse(match.Groups[1].Value.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
                var einheit = match.Groups[2].Value;
                var name = match.Groups[3].Value;

                var neueMenge = menge * kg;
                berechnet.AppendLine($"- {neueMenge:F1}{einheit} {name}");
            }
            else
            {
                berechnet.AppendLine(zeile);
            }
        }

        return berechnet.ToString();
    }

    private int _poekelnTage;
    public int PoekelnTage
    {
        get => _poekelnTage;
        set => SetProperty(ref _poekelnTage, value);
    }

    private int _abbrennenTage;
    public int AbbrennenTage
    {
        get => _abbrennenTage;
        set => SetProperty(ref _abbrennenTage, value);
    }

    private int _raeuchernTage;
    public int RaeuchernTage
    {
        get => _raeuchernTage;
        set => SetProperty(ref _raeuchernTage, value);
    }

    private int _reifenTage;
    public int ReifenTage
    {
        get => _reifenTage;
        set => SetProperty(ref _reifenTage, value);
    }
}
