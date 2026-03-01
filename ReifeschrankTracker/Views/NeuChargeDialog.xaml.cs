using ReifeschrankTracker.ViewModels;
using System.Windows;

namespace ReifeschrankTracker.Views;

public partial class NeuChargeDialog : Window
{
    public NeuChargeDialog()
    {
        InitializeComponent();
    }

    private void Speichern_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is NeuChargeViewModel vm && vm.Validieren())
            DialogResult = true;
    }
}
