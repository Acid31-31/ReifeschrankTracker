using System;
using System.Windows;
using ReifeManager_R01.Models;

namespace ReifeManager_R01.Views;

public partial class StueckDetailWindow : Window
{
    public Fleischstueck? EditiertesStueck { get; private set; }
    public DateTime? NeuesStartdatum { get; private set; }
    
    private Fleischstueck _originalStueck;
    private Charge _charge;

    public StueckDetailWindow(Fleischstueck stueck, Charge charge)
    {
        try
        {
            if (stueck == null)
                throw new ArgumentNullException(nameof(stueck));
            if (charge == null)
                throw new ArgumentNullException(nameof(charge));

            _originalStueck = stueck;
            _charge = charge;
            InitializeComponent();
            
            // Felder füllen NACH InitializeComponent
            IdBox.Text = stueck.Id.ToString();
            StartgewichtBox.Text = stueck.Startgewicht.ToString("F0");
            AktuellesBox.Text = stueck.AktuellesGewicht.ToString("F0");
            VerlustBox.Text = stueck.GewichtsverlustProzent.ToString("F2");
            ReifetageBox.Text = stueck.Reifetage.ToString();
            StatusBox.Text = stueck.Status.ToString();
            MessungenBox.Text = stueck.Messungen.Count.ToString();
            
            // Startdatum aus Charge laden
            StartdatumPicker.SelectedDate = charge.Startdatum;
            
            StartgewichtBox.Focus();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Öffnen: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            this.Close();
        }
    }

    private void Speichern_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string gewichtText = StartgewichtBox.Text?.Trim() ?? "";
            
            if (string.IsNullOrWhiteSpace(gewichtText))
            {
                MessageBox.Show("Startgewicht eingeben!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                StartgewichtBox.Focus();
                return;
            }

            if (!double.TryParse(gewichtText, out double neuesGewicht))
            {
                MessageBox.Show("Ungültiges Gewicht (z.B. 2500)", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                StartgewichtBox.Focus();
                return;
            }

            if (neuesGewicht <= 0 || neuesGewicht > 100000)
            {
                MessageBox.Show("Gewicht muss zwischen 0 und 100000 sein", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!StartdatumPicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Bitte Startdatum wählen!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _originalStueck.Startgewicht = neuesGewicht;
            EditiertesStueck = _originalStueck;
            NeuesStartdatum = StartdatumPicker.SelectedDate.Value;
            
            DialogResult = true;
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Speichern: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Abbrechen_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        this.Close();
    }
}
