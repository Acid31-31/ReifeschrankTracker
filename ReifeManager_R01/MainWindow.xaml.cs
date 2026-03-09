using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ReifeManager_R01.ViewModels;

namespace ReifeManager_R01;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }

    private void StueckeGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        try
        {
            var grid = sender as DataGrid;
            if (grid == null || grid.SelectedItem == null)
                return;

            var stueck = grid.SelectedItem as ReifeManager_R01.Models.Fleischstueck;
            if (stueck == null)
                return;

            var dialog = new Views.StueckDetailWindow(stueck)
            {
                Owner = this
            };

            if (dialog.ShowDialog() == true && dialog.EditiertesStueck is not null)
            {
                if (DataContext is MainViewModel vm && vm.SelectedCharge is not null)
                {
                    var bezug = stueck.Messungen.OrderByDescending(m => m.Datum).FirstOrDefault()?.Datum ?? DateTime.Today;
                    vm.AktualisiereStueckPublic(vm.SelectedCharge, stueck, bezug);
                    vm.AktualisiereChargeStatusPublic(vm.SelectedCharge);
                    vm.AktualisiereStueckUiPublic();
                    vm.SpeichernPublic();
                    vm.AktualisiereDiagrammPublic();
                    
                    vm.Statusmeldung = $"✓ Stück aktualisiert auf {stueck.Startgewicht:F0}g.";
                }
            }
            e.Handled = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Bearbeiten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void StueckeGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
    {
        if (e.EditAction != DataGridEditAction.Commit)
        {
            return;
        }

        Dispatcher.BeginInvoke(new Action(() =>
        {
            if (DataContext is MainViewModel vm)
            {
                vm.StueckAusGridAktualisieren();
            }
        }), DispatcherPriority.Background);
    }

    private void MessungenGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
    {
        if (e.EditAction != DataGridEditAction.Commit)
        {
            return;
        }

        Dispatcher.BeginInvoke(new Action(() =>
        {
            if (DataContext is MainViewModel vm)
            {
                vm.MessungAusGridAktualisieren();
            }
        }), DispatcherPriority.Background);
    }

    private void Rezept_Doppelklick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        var viewModel = (MainViewModel)DataContext;
        if (viewModel.SelectedCharge?.Rezept is not null)
        {
            var gewicht = 1000.0;
            if (viewModel.SelectedCharge.Stuecke.Count > 0)
            {
                gewicht = viewModel.SelectedCharge.Stuecke.Sum(s => s.Startgewicht);
            }

            var detailFenster = new Views.RezeptDetailWindow(viewModel.SelectedCharge.Rezept, gewicht)
            {
                Owner = this
            };
            detailFenster.ShowDialog();
        }
        else
        {
            MessageBox.Show("Für diese Charge wurde kein Rezept ausgewählt.", "Kein Rezept", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void ChargenListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        try
        {
            if (DataContext is not MainViewModel vm || vm.SelectedCharge is null)
                return;

            if (vm.SelectedCharge.Stuecke.Count == 0)
            {
                MessageBox.Show("Diese Charge hat noch keine Stücke.", "Kein Stück vorhanden", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Nimm das erste Stück zur Bearbeitung
            var stueck = vm.SelectedCharge.Stuecke.FirstOrDefault();
            if (stueck != null)
            {
                var dialog = new Views.StueckDetailWindow(stueck)
                {
                    Owner = this
                };

                if (dialog.ShowDialog() == true && dialog.EditiertesStueck is not null)
                {
                    var bezug = stueck.Messungen.OrderByDescending(m => m.Datum).FirstOrDefault()?.Datum ?? DateTime.Today;
                    vm.AktualisiereStueckPublic(vm.SelectedCharge, stueck, bezug);
                    vm.AktualisiereChargeStatusPublic(vm.SelectedCharge);
                    vm.AktualisiereStueckUiPublic();
                    vm.SpeichernPublic();
                    vm.AktualisiereDiagrammPublic();
                    
                    vm.Statusmeldung = $"✓ Stück aktualisiert auf {stueck.Startgewicht:F0}g.";
                }
            }
            e.Handled = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Bearbeiten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}