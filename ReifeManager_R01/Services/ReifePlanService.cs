using ReifeManager_R01.Models;

namespace ReifeManager_R01.Services;

public class ReifePlanService
{
    private static readonly string[] Profile =
    {
        "Coppa",
        "Pancetta",
        "Räucherschinken",
        "Schinken",
        "Standard"
    };

    public IReadOnlyList<string> HoleProfile() => Profile;

    public string HoleEmpfohlenesFleisch(string profil)
    {
        return profil switch
        {
            "Coppa" => "Nacken / Schweinekamm",
            "Pancetta" => "Schweinebauch",
            "Räucherschinken" => "Schinkenkeule / Oberschale / Unterschale",
            "Schinken" => "Keule / Oberschale / Nuss",
            _ => "Schwein oder Rind"
        };
    }

    public double HoleEmpfohlenenZielverlust(string profil)
    {
        return profil switch
        {
            "Coppa" => 30.0,
            "Pancetta" => 28.0,
            "Räucherschinken" => 32.0,
            "Schinken" => 35.0,
            _ => 30.0
        };
    }

    public void AnwendenProfil(Charge charge)
    {
        var typ = (charge.Fleischtyp ?? string.Empty).ToLowerInvariant();

        if (typ.Contains("coppa"))
        {
            AnwendenProfilName(charge, "Coppa");
            return;
        }

        if (typ.Contains("pancetta") || typ.Contains("panzeta"))
        {
            AnwendenProfilName(charge, "Pancetta");
            return;
        }

        if (typ.Contains("räucher") || typ.Contains("raeucher"))
        {
            AnwendenProfilName(charge, "Räucherschinken");
            return;
        }

        if (typ.Contains("schinken"))
        {
            AnwendenProfilName(charge, "Schinken");
            return;
        }

        AnwendenProfilName(charge, "Standard");
    }

    public void AnwendenProfilName(Charge charge, string profil)
    {
        charge.HerstellungsProfil = profil;

        switch (profil)
        {
            case "Coppa":
                charge.PoekelnTage = 14;
                charge.AbbrennenTage = 2;
                charge.RaeuchernTage = 0;
                charge.ReifenTage = 45;
                break;
            case "Pancetta":
                charge.PoekelnTage = 10;
                charge.AbbrennenTage = 2;
                charge.RaeuchernTage = 0;
                charge.ReifenTage = 30;
                break;
            case "Räucherschinken":
                charge.PoekelnTage = 21;
                charge.AbbrennenTage = 2;
                charge.RaeuchernTage = 3;
                charge.ReifenTage = 60;
                break;
            case "Schinken":
                charge.PoekelnTage = 21;
                charge.AbbrennenTage = 2;
                charge.RaeuchernTage = 0;
                charge.ReifenTage = 60;
                break;
            default:
                charge.PoekelnTage = 10;
                charge.AbbrennenTage = 1;
                charge.RaeuchernTage = 0;
                charge.ReifenTage = 30;
                break;
        }
    }
}
