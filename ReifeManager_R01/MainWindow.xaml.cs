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

        StueckeGrid.MouseDoubleClick += StueckeGrid_MouseDoubleClick;
    }

    private void StueckeGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is DataGrid grid && grid.SelectedItem is ReifeManager_R01.Models.Fleischstueck stueck)
        {
            var dialog = new Views.StueckDetailWindow(stueck)
            {
                Owner = this
            };

            if (dialog.ShowDialog() == true && dialog.EditiertesStueck is not null)
            {
                if (DataContext is MainViewModel vm)
                {
                    var charge = vm.SelectedCharge;
                    if (charge is not null)
                    {
                        var bezug = stueck.Messungen.OrderByDescending(m => m.Datum).FirstOrDefault()?.Datum ?? DateTime.Today;
                        vm.AktualisiereStueckPublic(charge, stueck, bezug);
                        vm.AktualisiereChargeStatusPublic(charge);
                        vm.AktualisiereStueckUiPublic();
                        vm.SpeichernPublic();
                        vm.AktualisiereDiagrammPublic();
                        
                        vm.Statusmeldung = $"✓ Stück aktualisiert auf {stueck.Startgewicht:F0}g.";
                    }
                }
            }
            e.Handled = true;
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
}