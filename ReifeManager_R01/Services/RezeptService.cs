using ReifeManager_R01.Models;

namespace ReifeManager_R01.Services;

public class RezeptService
{
    private static readonly List<Rezept> AlleRezepte = new()
    {
        new Rezept
        {
            Name = "Italienische Coppa",
            Profil = "Coppa",
            Beschreibung = "Klassische italienische Coppa nach traditionellem Rezept",
            Zutaten = "Pro kg Fleisch:\n- 30g Meersalz\n- 3g Pökelsalz\n- 5g schwarzer Pfeffer\n- 2g Knoblauch\n- 1g Lorbeer\n- 0,5g Wacholderbeeren",
            Anleitung = "1. Fleisch zuschneiden und trimmen\n2. Trockenpökelung 14 Tage\n3. Abbrennen 2 Tage\n4. Reifen bei 12-15°C, 75-80% Luftfeuchte\n5. Ziel: 30% Gewichtsverlust",
            PoekelnTage = 14,
            AbbrennenTage = 2,
            RaeuchernTage = 0,
            ReifenTage = 45
        },
        new Rezept
        {
            Name = "Würzige Coppa mit Chili",
            Profil = "Coppa",
            Beschreibung = "Scharfe Variante mit Chili und Paprika",
            Zutaten = "Pro kg Fleisch:\n- 30g Meersalz\n- 3g Pökelsalz\n- 8g Paprika edelsüß\n- 3g Chili\n- 4g schwarzer Pfeffer\n- 2g Knoblauch",
            Anleitung = "1. Fleisch mit Gewürzmischung einreiben\n2. Vakuumpökeln 12 Tage\n3. Abbrennen 2 Tage\n4. Reifen bei 13°C, 75% Luftfeuchte\n5. Ziel: 30-35% Gewichtsverlust",
            PoekelnTage = 12,
            AbbrennenTage = 2,
            RaeuchernTage = 0,
            ReifenTage = 50
        },
        new Rezept
        {
            Name = "Klassische Pancetta",
            Profil = "Pancetta",
            Beschreibung = "Italienische Pancetta - luftgetrockneter Bauchspeck",
            Zutaten = "Pro kg Fleisch:\n- 35g Meersalz\n- 3g Pökelsalz\n- 5g schwarzer Pfeffer\n- 3g Knoblauch\n- 1g Thymian\n- 1g Rosmarin",
            Anleitung = "1. Bauch flach ausbreiten\n2. Trockenpökelung 10 Tage\n3. Abbrennen 2 Tage\n4. Optional aufrollen und binden\n5. Reifen bei 12-14°C, 70-75% Luftfeuchte",
            PoekelnTage = 10,
            AbbrennenTage = 2,
            RaeuchernTage = 0,
            ReifenTage = 30
        },
        new Rezept
        {
            Name = "Deutscher Räucherschinken",
            Profil = "Räucherschinken",
            Beschreibung = "Klassischer geräucherter Schinken nach deutscher Art",
            Zutaten = "Pro kg Fleisch:\n- 25g Meersalz\n- 3g Pökelsalz\n- 4g Wacholderbeeren\n- 3g schwarzer Pfeffer\n- 2g Lorbeer\n- 1g Koriander",
            Anleitung = "1. Nasspökelung in Lake 21 Tage\n2. Abbrennen 2 Tage\n3. Kalträuchern bei 15-20°C, 3 Tage\n4. Reifen bei 12°C, 75% Luftfeuchte\n5. Ziel: 30% Gewichtsverlust",
            PoekelnTage = 21,
            AbbrennenTage = 2,
            RaeuchernTage = 3,
            ReifenTage = 60
        },
        new Rezept
        {
            Name = "Bauernschinken",
            Profil = "Schinken",
            Beschreibung = "Luftgetrockneter Schinken ohne Räuchern",
            Zutaten = "Pro kg Fleisch:\n- 30g Meersalz\n- 3g Pökelsalz\n- 5g schwarzer Pfeffer\n- 3g Knoblauch\n- 2g Lorbeer",
            Anleitung = "1. Trockenpökelung 21 Tage\n2. Abbrennen 2 Tage\n3. Reifen bei 10-13°C, 70-75% Luftfeuchte\n4. Ziel: 35% Gewichtsverlust",
            PoekelnTage = 21,
            AbbrennenTage = 2,
            RaeuchernTage = 0,
            ReifenTage = 60
        },
        new Rezept
        {
            Name = "Kräuter-Coppa",
            Profil = "Coppa",
            Beschreibung = "Coppa mit Kräuternote und etwas Fenchel",
            Zutaten = "Pro kg Fleisch:\n- 30g Meersalz\n- 3g Pökelsalz\n- 4g schwarzer Pfeffer\n- 2g Knoblauch\n- 1g Fenchelsamen\n- 1g Rosmarin",
            Anleitung = "1. Gewürzmischung herstellen\n2. Fleisch gleichmäßig einreiben\n3. Vakuumpökeln 14 Tage\n4. Abbrennen 2 Tage\n5. Reifen bei 12-14°C und 75% Luftfeuchte",
            PoekelnTage = 14,
            AbbrennenTage = 2,
            RaeuchernTage = 0,
            ReifenTage = 48
        },
        new Rezept
        {
            Name = "Gerollte Pancetta (Arrotolata)",
            Profil = "Pancetta",
            Beschreibung = "Klassisch gerollte Pancetta mit Muskat und Pfeffer",
            Zutaten = "Pro kg Fleisch:\n- 35g Meersalz\n- 3g Pökelsalz\n- 4g schwarzer Pfeffer\n- 2g Knoblauch\n- 1g Muskat\n- 1g Thymian",
            Anleitung = "1. Bauch vorbereiten und würzen\n2. Trockenpökeln 10 Tage\n3. Abbrennen 2 Tage\n4. Aufrollen und fest binden\n5. Reifen bei 12-14°C und 70-75% Luftfeuchte",
            PoekelnTage = 10,
            AbbrennenTage = 2,
            RaeuchernTage = 0,
            ReifenTage = 35
        },
        new Rezept
        {
            Name = "Räucherschinken mit Knoblauch",
            Profil = "Räucherschinken",
            Beschreibung = "Milder Räucherschinken mit Knoblauch und Majoran",
            Zutaten = "Pro kg Fleisch:\n- 26g Meersalz\n- 3g Pökelsalz\n- 3g schwarzer Pfeffer\n- 2g Knoblauch\n- 1g Majoran\n- 1g Wacholderbeeren",
            Anleitung = "1. Fleisch trocken pökeln 18 Tage\n2. Abbrennen 2 Tage\n3. Kalträuchern 4 Tage\n4. Reifen bei 11-13°C und 75% Luftfeuchte",
            PoekelnTage = 18,
            AbbrennenTage = 2,
            RaeuchernTage = 4,
            ReifenTage = 58
        },
        new Rezept
        {
            Name = "Pfefferschinken",
            Profil = "Schinken",
            Beschreibung = "Luftgetrockneter Schinken mit kräftiger Pfeffernote",
            Zutaten = "Pro kg Fleisch:\n- 30g Meersalz\n- 3g Pökelsalz\n- 7g schwarzer Pfeffer\n- 2g Knoblauch\n- 1g Koriander",
            Anleitung = "1. Fleisch mit Gewürzen einreiben\n2. Trockenpökelung 20 Tage\n3. Abbrennen 2 Tage\n4. Reifen bei 10-12°C und 70-75% Luftfeuchte\n5. Ziel: 35% Gewichtsverlust",
            PoekelnTage = 20,
            AbbrennenTage = 2,
            RaeuchernTage = 0,
            ReifenTage = 62
        },
        new Rezept
        {
            Name = "Standard-Würzmix",
            Profil = "Standard",
            Beschreibung = "Einfaches Allround-Rezept für den Einstieg",
            Zutaten = "Pro kg Fleisch:\n- 28g Meersalz\n- 3g Pökelsalz\n- 4g schwarzer Pfeffer\n- 2g Knoblauch",
            Anleitung = "1. Fleisch vorbereiten\n2. Trockenpökeln 12 Tage\n3. Abbrennen 1-2 Tage\n4. Reifen bei 12°C und 75% Luftfeuchte",
            PoekelnTage = 12,
            AbbrennenTage = 2,
            RaeuchernTage = 0,
            ReifenTage = 40
        },
        new Rezept
        {
            Name = "Winter-Coppa mit Wacholder",
            Profil = "Coppa",
            Beschreibung = "Kräftige Wintervariante mit Wacholder und Lorbeer",
            Zutaten = "Pro kg Fleisch:\n- 31g Meersalz\n- 3g Pökelsalz\n- 5g schwarzer Pfeffer\n- 2g Knoblauch\n- 2g Wacholderbeeren\n- 1g Lorbeer",
            Anleitung = "1. Gewürze mörsern und mischen\n2. Fleisch einreiben\n3. Trockenpökeln 15 Tage\n4. Abbrennen 2 Tage\n5. Reifen bei 11-13°C und 76% Luftfeuchte",
            PoekelnTage = 15,
            AbbrennenTage = 2,
            RaeuchernTage = 0,
            ReifenTage = 52
        },
        new Rezept
        {
            Name = "Sommer-Pancetta mit Zitrone",
            Profil = "Pancetta",
            Beschreibung = "Leichte Sommernote mit Zitronenschale und Thymian",
            Zutaten = "Pro kg Fleisch:\n- 34g Meersalz\n- 3g Pökelsalz\n- 4g schwarzer Pfeffer\n- 2g Knoblauch\n- 1g Zitronenschale\n- 1g Thymian",
            Anleitung = "1. Bauch würzen\n2. Trockenpökeln 9 Tage\n3. Abbrennen 2 Tage\n4. Optional rollen\n5. Reifen bei 13-14°C und 70-74% Luftfeuchte",
            PoekelnTage = 9,
            AbbrennenTage = 2,
            RaeuchernTage = 0,
            ReifenTage = 32
        },
        new Rezept
        {
            Name = "Winter-Räucherschinken",
            Profil = "Räucherschinken",
            Beschreibung = "Aromatischer Räucherschinken für kalte Jahreszeit",
            Zutaten = "Pro kg Fleisch:\n- 26g Meersalz\n- 3g Pökelsalz\n- 4g Wacholderbeeren\n- 3g schwarzer Pfeffer\n- 1g Majoran\n- 1g Lorbeer",
            Anleitung = "1. Trockenpökeln 20 Tage\n2. Abbrennen 2 Tage\n3. Kalträuchern 5 Tage\n4. Reifen bei 10-12°C und 75-78% Luftfeuchte",
            PoekelnTage = 20,
            AbbrennenTage = 2,
            RaeuchernTage = 5,
            ReifenTage = 65
        },
        new Rezept
        {
            Name = "Sommer-Schinken mild",
            Profil = "Schinken",
            Beschreibung = "Milde Sommer-Variante mit weniger Pfeffer",
            Zutaten = "Pro kg Fleisch:\n- 29g Meersalz\n- 3g Pökelsalz\n- 3g schwarzer Pfeffer\n- 2g Knoblauch\n- 1g Koriander",
            Anleitung = "1. Fleisch einreiben\n2. Trockenpökeln 18 Tage\n3. Abbrennen 2 Tage\n4. Reifen bei 12-14°C und 70-74% Luftfeuchte",
            PoekelnTage = 18,
            AbbrennenTage = 2,
            RaeuchernTage = 0,
            ReifenTage = 55
        }
    };

    public List<Rezept> HoleRezepteZuProfil(string profil)
    {
        return AlleRezepte
            .Where(r => r.Profil.Equals(profil, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public Rezept? HoleStandardRezept(string profil)
    {
        return AlleRezepte.FirstOrDefault(r => r.Profil.Equals(profil, StringComparison.OrdinalIgnoreCase));
    }
}
