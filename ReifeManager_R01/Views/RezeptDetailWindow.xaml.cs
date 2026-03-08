using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using ReifeManager_R01.Models;

namespace ReifeManager_R01.Views;

public partial class RezeptDetailWindow : Window
{
    private readonly Rezept _rezept;
    private readonly double _gewicht;

    public RezeptDetailWindow(Rezept rezept, double gewichtInGramm = 1000)
    {
        InitializeComponent();

        _rezept = rezept;
        _gewicht = gewichtInGramm;

        RezeptName.Text = rezept.Name;
        DruckRezeptName.Text = rezept.Name;
        DruckBeschreibung.Text = rezept.Beschreibung;
        DruckPoekeln.Text = $"{rezept.PoekelnTage} Tage";
        DruckAbbrennen.Text = $"{rezept.AbbrennenTage} Tage";
        DruckRaeuchern.Text = $"{rezept.RaeuchernTage} Tage";
        DruckReifen.Text = $"{rezept.ReifenTage} Tage";

        var zutatenBerechnet = rezept.BerechneZutatenFuerGewicht(gewichtInGramm);
        DruckZutaten.Text = zutatenBerechnet;
        DruckAnleitung.Text = rezept.Anleitung;
    }

    private void Drucken_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                var fixedDoc = new FixedDocument();
                var pageContent = new PageContent();
                var fixedPage = new FixedPage
                {
                    Width = printDialog.PrintableAreaWidth,
                    Height = printDialog.PrintableAreaHeight
                };

                var border = new Border
                {
                    Background = Brushes.White,
                    Padding = new Thickness(40),
                    Width = fixedPage.Width,
                    Height = fixedPage.Height
                };

                var stack = new StackPanel();

                stack.Children.Add(new TextBlock
                {
                    Text = DruckRezeptName.Text,
                    FontSize = 24,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.Black,
                    Margin = new Thickness(0, 0, 0, 20)
                });

                stack.Children.Add(new TextBlock
                {
                    Text = DruckBeschreibung.Text,
                    FontSize = 14,
                    Foreground = Brushes.Black,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 20)
                });

                stack.Children.Add(new TextBlock
                {
                    Text = $"Pökeln: {DruckPoekeln.Text} | Abbrennen: {DruckAbbrennen.Text} | Räuchern: {DruckRaeuchern.Text} | Reifen: {DruckReifen.Text}",
                    FontSize = 13,
                    Foreground = Brushes.Black,
                    Margin = new Thickness(0, 0, 0, 20)
                });

                stack.Children.Add(new TextBlock
                {
                    Text = "Zutaten:",
                    FontSize = 16,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = Brushes.Black,
                    Margin = new Thickness(0, 0, 0, 8)
                });

                stack.Children.Add(new TextBlock
                {
                    Text = DruckZutaten.Text,
                    FontSize = 12,
                    Foreground = Brushes.Black,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 20)
                });

                stack.Children.Add(new TextBlock
                {
                    Text = "Anleitung:",
                    FontSize = 16,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = Brushes.Black,
                    Margin = new Thickness(0, 0, 0, 8)
                });

                stack.Children.Add(new TextBlock
                {
                    Text = DruckAnleitung.Text,
                    FontSize = 12,
                    Foreground = Brushes.Black,
                    TextWrapping = TextWrapping.Wrap
                });

                border.Child = stack;
                fixedPage.Children.Add(border);
                ((IAddChild)pageContent).AddChild(fixedPage);
                fixedDoc.Pages.Add(pageContent);

                printDialog.PrintDocument(fixedDoc.DocumentPaginator, "ReifeManager Rezept");

                MessageBox.Show("Rezept wurde gedruckt.", "Drucken", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Drucken: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Schliessen_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
