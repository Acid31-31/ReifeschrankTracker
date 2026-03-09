using System;
using System.Windows;
using ReifeManager_R01.Models;

namespace ReifeManager_R01.Views;

public partial class StueckDetailWindow : Window
{
    public Fleischstueck? EditiertesStueck { get; private set; }

    private readonly Fleischstueck _originalStueck;

    public StueckDetailWindow(Fleischstueck stueck)
    {
        try
        {
            InitializeComponent();
            
            if (stueck == null)
            {
                throw new ArgumentNullException(nameof(stueck));
            }
            
            _originalStueck = stueck;
            
            // DataContext mit Read-Only Daten
            DataContext = new
            {
                StuckId = stueck.Id.ToString(),
                AktuellesGewicht = stueck.AktuellesGewicht,
                Gewichtsverlust = stueck.GewichtsverlustProzent,
                Reifetage = stueck.Reifetage,
                Status = stueck.Status.ToString(),
                MessungenCount = stueck.Messungen?.Count ?? 0
            };

            // Startgewicht Textbox füllen
            if (StartgewichtBox != null)
            {
                StartgewichtBox.Text = stueck.Startgewicht.ToString("F0");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Öffnen des Dialogs: {ex.Message}", "Fehler", 
                MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
        }
    }

    private void Speichern_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (StartgewichtBox == null || string.IsNullOrWhiteSpace(StartgewichtBox.Text))
            {
                MessageBox.Show("Startgewicht ist erforderlich.", "Eingabefehler", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(StartgewichtBox.Text, out var neuesGewicht) || neuesGewicht <= 0)
            {
                MessageBox.Show("Startgewicht muss eine positive Zahl sein (z.B. 2500).", "Ungültige Eingabe", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _originalStueck.Startgewicht = neuesGewicht;
            EditiertesStueck = _originalStueck;
            
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Speichern: {ex.Message}", "Fehler", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Abbrechen_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            DialogResult = false;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Schließen: {ex.Message}", "Fehler");
            Close();
        }
    }
}
