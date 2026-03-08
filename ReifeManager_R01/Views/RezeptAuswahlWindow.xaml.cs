using System.Windows;
using System.Windows.Input;
using ReifeManager_R01.Models;

namespace ReifeManager_R01.Views;

public partial class RezeptAuswahlWindow : Window
{
    public Rezept? AusgewaehlitesRezept { get; private set; }

    public RezeptAuswahlWindow(Charge charge, List<Rezept> rezepte)
    {
        InitializeComponent();

        ChargeBezeichnung.Text = charge.Bezeichnung;
        ChargeFleischtyp.Text = charge.Fleischtyp;
        ChargeProfil.Text = charge.HerstellungsProfil;

        RezeptListe.ItemsSource = rezepte;
    }

    private void Rezept_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: Rezept rezept })
        {
            AusgewaehlitesRezept = rezept;
            DialogResult = true;
            Close();
        }
    }

    private void Abbrechen_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
