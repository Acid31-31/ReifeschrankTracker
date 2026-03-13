using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        var version = Assembly.GetExecutingAssembly().GetName().Version;
        var displayVersion = version is null
            ? "unbekannt"
            : $"{version.Major}.{version.Minor}.{version.Revision}";
        VersionMenuItem.Header = $"Aktuelle Software-Version: {displayVersion}";
    }

    private void MenuDateiBeenden_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void MenuBearbeitenUpdates_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm && vm.UpdatesPruefenCommand.CanExecute(null))
        {
            vm.UpdatesPruefenCommand.Execute(null);
        }
    }

    private void MenuAnsichtVollbild_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    private void DashboardScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (sender is not ScrollViewer scrollViewer)
        {
            return;
        }

        if (e.OriginalSource is DependencyObject source)
        {
            var inDataGrid = FindAncestor<DataGrid>(source) is not null;
            var inComboBox = FindAncestor<ComboBox>(source) is not null;
            if (inDataGrid || inComboBox)
            {
                return;
            }
        }

        var newOffset = scrollViewer.VerticalOffset - (e.Delta / 3.0);
        if (newOffset < 0)
        {
            newOffset = 0;
        }

        if (newOffset > scrollViewer.ScrollableHeight)
        {
            newOffset = scrollViewer.ScrollableHeight;
        }

        scrollViewer.ScrollToVerticalOffset(newOffset);
        e.Handled = true;
    }

    private static T? FindAncestor<T>(DependencyObject? current) where T : DependencyObject
    {
        while (current is not null)
        {
            if (current is T match)
            {
                return match;
            }

            current = System.Windows.Media.VisualTreeHelper.GetParent(current);
        }

        return null;
    }

    private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (sender is not DataGrid dataGrid)
        {
            return;
        }

        var scrollViewer = FindDescendant<ScrollViewer>(dataGrid);
        if (scrollViewer is null)
        {
            return;
        }

        var newOffset = scrollViewer.VerticalOffset - (e.Delta / 3.0);
        if (newOffset < 0)
        {
            newOffset = 0;
        }

        if (newOffset > scrollViewer.ScrollableHeight)
        {
            newOffset = scrollViewer.ScrollableHeight;
        }

        scrollViewer.ScrollToVerticalOffset(newOffset);
        e.Handled = true;
    }

    private static T? FindDescendant<T>(DependencyObject? root) where T : DependencyObject
    {
        if (root is null)
        {
            return null;
        }

        for (var i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(root); i++)
        {
            var child = System.Windows.Media.VisualTreeHelper.GetChild(root, i);
            if (child is T match)
            {
                return match;
            }

            var nested = FindDescendant<T>(child);
            if (nested is not null)
            {
                return nested;
            }
        }

        return null;
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

            if (DataContext is not MainViewModel vm || vm.SelectedCharge is null)
                return;

            var dialog = new Views.StueckDetailWindow(stueck, vm.SelectedCharge)
            {
                Owner = this
            };

            if (dialog.ShowDialog() == true && dialog.EditiertesStueck is not null)
            {
                // Startdatum in Charge aktualisieren wenn geändert
                if (dialog.NeuesStartdatum.HasValue)
                {
                    vm.SelectedCharge.Startdatum = dialog.NeuesStartdatum.Value;
                }

                // Zielverlust in Charge aktualisieren wenn geändert
                if (dialog.NeuesZielverlust.HasValue)
                {
                    vm.SelectedCharge.ZielverlustProzent = dialog.NeuesZielverlust.Value;
                }

                var bezug = stueck.Messungen.OrderByDescending(m => m.Datum).FirstOrDefault()?.Datum ?? DateTime.Today;
                vm.AktualisiereStueckPublic(vm.SelectedCharge, stueck, bezug);
                vm.AktualisiereChargeStatusPublic(vm.SelectedCharge);
                vm.AktualisiereStueckUiPublic();
                vm.SpeichernPublic();
                vm.AktualisiereDiagrammPublic();
                
                vm.Statusmeldung = $"✓ Stück + Charge aktualisiert. Zielverlust: {vm.SelectedCharge.ZielverlustProzent:F1}%";
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
        OpenRezeptDetailsFromSelection();
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

            var stueck = vm.SelectedCharge.Stuecke.FirstOrDefault();
            var charge = vm.SelectedCharge;
            
            if (stueck != null && charge != null)
            {
                var dialog = new Views.StueckDetailWindow(stueck, charge)
                {
                    Owner = this
                };

                if (dialog.ShowDialog() == true && dialog.EditiertesStueck is not null)
                {
                    // Startdatum in Charge aktualisieren wenn geändert
                    if (dialog.NeuesStartdatum.HasValue)
                    {
                        charge.Startdatum = dialog.NeuesStartdatum.Value;
                    }

                    // Zielverlust in Charge aktualisieren wenn geändert
                    if (dialog.NeuesZielverlust.HasValue)
                    {
                        charge.ZielverlustProzent = dialog.NeuesZielverlust.Value;
                    }

                    var bezug = stueck.Messungen.OrderByDescending(m => m.Datum).FirstOrDefault()?.Datum ?? DateTime.Today;
                    vm.AktualisiereStueckPublic(charge, stueck, bezug);
                    vm.AktualisiereChargeStatusPublic(charge);
                    vm.AktualisiereStueckUiPublic();
                    vm.SpeichernPublic();
                    vm.AktualisiereDiagrammPublic();
                    
                    vm.Statusmeldung = $"✓ Stück + Charge aktualisiert. Zielverlust: {charge.ZielverlustProzent:F1}%";
                }
            }
            e.Handled = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Bearbeiten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void MenuAufgabenFaelligeMessungen_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm)
        {
            return;
        }

        var heute = DateTime.Today;
        var faellige = vm.Chargen
            .SelectMany(c => c.Stuecke.Select(s => new
            {
                Charge = c.Bezeichnung,
                LetzteMessung = s.Messungen.OrderByDescending(m => m.Datum).FirstOrDefault()?.Datum.Date
            }))
            .Where(x => x.LetzteMessung is null || x.LetzteMessung < heute)
            .ToList();

        if (faellige.Count == 0)
        {
            MessageBox.Show("Heute sind keine Messungen fällig.", "Aufgaben", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var details = string.Join(Environment.NewLine, faellige.Take(12).Select(x => $"• {x.Charge}"));
        var suffix = faellige.Count > 12 ? $"{Environment.NewLine}... und {faellige.Count - 12} weitere" : string.Empty;
        MessageBox.Show($"Fällige Messungen heute: {faellige.Count}{Environment.NewLine}{Environment.NewLine}{details}{suffix}", "Aufgaben", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void MenuAufgabenMessungEintragen_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm && vm.MessungHinzufuegenCommand.CanExecute(null))
        {
            vm.MessungHinzufuegenCommand.Execute(null);
        }
    }

    private void MenuAufgabenChargeAnlegen_Click(object sender, RoutedEventArgs e)
    {
        OeffneNeueChargeDialog();
    }

    private void NeueChargeButton_Click(object sender, RoutedEventArgs e)
    {
        OeffneNeueChargeDialog();
    }

    private void OeffneNeueChargeDialog()
    {
        if (DataContext is not MainViewModel vm)
        {
            return;
        }

        var dialog = new Views.NeueChargeWindow(
            vm.ProfilOptionen,
            vm.AusgewaehltesProfil,
            vm.NeueBezeichnung,
            vm.NeuerFleischtyp,
            vm.NeuesStartdatum,
            vm.NeuesZielverlustProzent,
            vm.NeuePoekelnTage,
            vm.NeueAbbrennenTage,
            vm.NeueRaeuchernTage,
            vm.NeueReifenTage)
        {
            Owner = this
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        vm.AusgewaehltesProfil = dialog.AusgewaehltesProfil;
        vm.NeueBezeichnung = dialog.Bezeichnung;
        vm.NeuerFleischtyp = dialog.Fleischtyp;
        vm.NeuesStartdatum = dialog.Startdatum;
        vm.NeuesZielverlustProzent = dialog.ZielverlustProzent;
        vm.NeuePoekelnTage = dialog.PoekelnTage;
        vm.NeueAbbrennenTage = dialog.AbbrennenTage;
        vm.NeueRaeuchernTage = dialog.RaeuchernTage;
        vm.NeueReifenTage = dialog.ReifenTage;

        if (vm.ChargeHinzufuegenCommand.CanExecute(null))
        {
            vm.ChargeHinzufuegenCommand.Execute(null);
        }
    }

    private void MenuAufgabenStueckBearbeiten_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm && vm.StueckBearbeitenCommand.CanExecute(null))
        {
            vm.StueckBearbeitenCommand.Execute(null);
        }
    }

    private void MenuAufgabenRezeptOeffnen_Click(object sender, RoutedEventArgs e)
    {
        OpenRezeptDetailsFromSelection();
    }

    private void MenuReifungStatusCheck_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm)
        {
            return;
        }

        var alle = vm.Chargen.ToList();
        var kritisch = alle.Count(c => c.StatusUebersicht.Contains("krit", StringComparison.OrdinalIgnoreCase));
        var warnung = alle.Count(c => c.StatusUebersicht.Contains("warn", StringComparison.OrdinalIgnoreCase));
        var ok = alle.Count - kritisch - warnung;

        MessageBox.Show($"Status-Check abgeschlossen:{Environment.NewLine}Gesamt: {alle.Count}{Environment.NewLine}Kritisch: {kritisch}{Environment.NewLine}Warnung: {warnung}{Environment.NewLine}Unauffällig: {ok}",
            "Reifung", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void MenuReifungWarnungen_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm)
        {
            return;
        }

        var warnungen = vm.Chargen
            .Where(c => c.StatusUebersicht.Contains("warn", StringComparison.OrdinalIgnoreCase) ||
                        c.StatusUebersicht.Contains("krit", StringComparison.OrdinalIgnoreCase))
            .Select(c => $"• {c.Bezeichnung}: {c.StatusUebersicht}")
            .ToList();

        if (warnungen.Count == 0)
        {
            MessageBox.Show("Keine Warnungen oder kritischen Chargen vorhanden.", "Reifung", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        MessageBox.Show(string.Join(Environment.NewLine, warnungen), "Warnungen/Kritische Chargen", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    private void MenuReifungZielverlustNeuBerechnen_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm)
        {
            return;
        }

        foreach (var charge in vm.Chargen)
        {
            foreach (var stueck in charge.Stuecke)
            {
                var bezug = stueck.Messungen.OrderByDescending(m => m.Datum).FirstOrDefault()?.Datum ?? DateTime.Today;
                vm.AktualisiereStueckPublic(charge, stueck, bezug);
            }

            vm.AktualisiereChargeStatusPublic(charge);
        }

        vm.AktualisiereStueckUiPublic();
        vm.AktualisiereDiagrammPublic();
        vm.SpeichernPublic();
        vm.Statusmeldung = "✓ Zielverlust und Status wurden neu berechnet.";
    }

    private void MenuExportCsv_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm && vm.VerlaufExportierenCommand.CanExecute(null))
        {
            vm.VerlaufExportierenCommand.Execute(null);
        }
    }

    private void MenuExportPdf_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm && vm.BerichtExportierenCommand.CanExecute(null))
        {
            vm.BerichtExportierenCommand.Execute(null);
        }
    }

    private void MenuExportHistorieOeffnen_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm)
        {
            return;
        }

        var pfad = vm.ExportHistorie.FirstOrDefault()?.Pfad;
        if (string.IsNullOrWhiteSpace(pfad))
        {
            MessageBox.Show("Es ist noch kein Export vorhanden.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var ordner = Path.GetDirectoryName(pfad);
        if (string.IsNullOrWhiteSpace(ordner) || !Directory.Exists(ordner))
        {
            MessageBox.Show("Export-Ordner wurde nicht gefunden.", "Export", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{pfad}\"") { UseShellExecute = true });
    }

    private void MenuSystemBackupJetztErstellen_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ReifeManager");
            var datenDatei = Path.Combine(appData, "chargen.json");
            var backupOrdner = Path.Combine(appData, "Backups");
            Directory.CreateDirectory(backupOrdner);

            if (!File.Exists(datenDatei))
            {
                MessageBox.Show("Keine Daten-Datei gefunden (chargen.json).", "Backup", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var ziel = Path.Combine(backupOrdner, $"chargen_manual_{DateTime.Now:yyyyMMdd_HHmmss}.json");
            File.Copy(datenDatei, ziel, true);
            MessageBox.Show($"Backup erstellt:{Environment.NewLine}{ziel}", "Backup", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Backup fehlgeschlagen:{Environment.NewLine}{ex.Message}", "Backup", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void MenuSystemLogsAnzeigen_Click(object sender, RoutedEventArgs e)
    {
        var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ReifeManager");
        Directory.CreateDirectory(appData);
        Process.Start(new ProcessStartInfo("explorer.exe", appData) { UseShellExecute = true });
    }

    private void OpenRezeptDetailsFromSelection()
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