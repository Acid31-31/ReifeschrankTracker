using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using ReifeManager_R01.Models;
using ReifeManager_R01.Services;

namespace ReifeManager_R01.Views;

public partial class NeueChargeWindow : Window
{
    private readonly ReifePlanService _reifePlanService = new();

    public string AusgewaehltesProfil { get; private set; } = string.Empty;
    public string Bezeichnung { get; private set; } = string.Empty;
    public string Fleischtyp { get; private set; } = string.Empty;
    public DateTime Startdatum { get; private set; } = DateTime.Today;
    public string ZielverlustProzent { get; private set; } = "30";
    public string PoekelnTage { get; private set; } = "10";
    public string AbbrennenTage { get; private set; } = "1";
    public string RaeuchernTage { get; private set; } = "0";
    public string ReifenTage { get; private set; } = "30";

    public NeueChargeWindow(
        IEnumerable<string> profile,
        string ausgewaehltesProfil,
        string bezeichnung,
        string fleischtyp,
        DateTime startdatum,
        string zielverlust,
        string poekeln,
        string abbrennen,
        string raeuchern,
        string reifen)
    {
        InitializeComponent();

        ProfilComboBox.ItemsSource = profile;
        ProfilComboBox.SelectedItem = string.IsNullOrWhiteSpace(ausgewaehltesProfil) ? "Standard" : ausgewaehltesProfil;

        BezeichnungTextBox.Text = bezeichnung;
        FleischtypTextBox.Text = fleischtyp;
        StartdatumPicker.SelectedDate = startdatum;
        ZielverlustTextBox.Text = zielverlust;
        PoekelnTextBox.Text = poekeln;
        AbbrennenTextBox.Text = abbrennen;
        RaeuchernTextBox.Text = raeuchern;
        ReifenTextBox.Text = reifen;
    }

    private void ProfilComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ProfilComboBox.SelectedItem is not string profil || string.IsNullOrWhiteSpace(profil))
        {
            return;
        }

        var charge = new Charge();
        _reifePlanService.AnwendenProfilName(charge, profil);

        FleischtypTextBox.Text = _reifePlanService.HoleEmpfohlenesFleisch(profil);
        ZielverlustTextBox.Text = _reifePlanService.HoleEmpfohlenenZielverlust(profil).ToString("F0", CultureInfo.CurrentCulture);
        PoekelnTextBox.Text = charge.PoekelnTage.ToString(CultureInfo.CurrentCulture);
        AbbrennenTextBox.Text = charge.AbbrennenTage.ToString(CultureInfo.CurrentCulture);
        RaeuchernTextBox.Text = charge.RaeuchernTage.ToString(CultureInfo.CurrentCulture);
        ReifenTextBox.Text = charge.ReifenTage.ToString(CultureInfo.CurrentCulture);
    }

    private void Anlegen_Click(object sender, RoutedEventArgs e)
    {
        if (ProfilComboBox.SelectedItem is not string profil || string.IsNullOrWhiteSpace(profil))
        {
            MessageBox.Show("Bitte ein Profil auswählen.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (string.IsNullOrWhiteSpace(BezeichnungTextBox.Text))
        {
            MessageBox.Show("Bitte eine Bezeichnung eingeben.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        AusgewaehltesProfil = profil;
        Bezeichnung = BezeichnungTextBox.Text.Trim();
        Fleischtyp = FleischtypTextBox.Text.Trim();
        Startdatum = StartdatumPicker.SelectedDate ?? DateTime.Today;
        ZielverlustProzent = ZielverlustTextBox.Text.Trim();
        PoekelnTage = PoekelnTextBox.Text.Trim();
        AbbrennenTage = AbbrennenTextBox.Text.Trim();
        RaeuchernTage = RaeuchernTextBox.Text.Trim();
        ReifenTage = ReifenTextBox.Text.Trim();

        DialogResult = true;
    }

    private void Abbrechen_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
