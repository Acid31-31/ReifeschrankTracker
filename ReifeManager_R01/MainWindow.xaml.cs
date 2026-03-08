using System;
using System.Linq;
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
        
        PreviewMouseWheel += MainWindow_PreviewMouseWheel;
    }

    private void MainWindow_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var scrollViewer = FindScrollViewer((DependencyObject)sender);
        if (scrollViewer != null)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta / 3.0);
            e.Handled = true;
        }
    }

    private ScrollViewer FindScrollViewer(DependencyObject obj)
    {
        if (obj is ScrollViewer sv)
            return sv;

        for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            var child = System.Windows.Media.VisualTreeHelper.GetChild(obj, i);
            var result = FindScrollViewer(child);
            if (result != null)
                return result;
        }
        return null;
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