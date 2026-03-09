using System.Windows;
using ReifeManager_R01.Models;

namespace ReifeManager_R01.Views;

public partial class StueckDetailWindow : Window
{
    public Fleischstueck? EditiertesStueck { get; private set; }

    private readonly Fleischstueck _originalStueck;

    public StueckDetailWindow(Fleischstueck stueck)
    {
        InitializeComponent();
        _originalStueck = stueck;
        
        DataContext = new
        {
            StuckId = stueck.Id,
            AktuellesGewicht = stueck.AktuellesGewicht,
            Gewichtsverlust = stueck.GewichtsverlustProzent,
            Reifetage = stueck.Reifetage,
            Status = stueck.Status,
            MessungenCount = stueck.Messungen.Count
        };

        StartgewichtBox.Text = stueck.Startgewicht.ToString("F0");
    }

    private void Speichern_Click(object sender, RoutedEventArgs e)
    {
        if (!double.TryParse(StartgewichtBox.Text, out var neuesGewicht) || neuesGewicht <= 0)
        {
            MessageBox.Show("Startgewicht muss eine positive Zahl sein.", "Ungültige Eingabe", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _originalStueck.Startgewicht = neuesGewicht;
        EditiertesStueck = _originalStueck;
        
        DialogResult = true;
        Close();
    }

    private void Abbrechen_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
