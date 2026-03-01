using ReifeschrankTracker.ViewModels;
using System.Windows;

namespace ReifeschrankTracker.Views;

public partial class GewichtEintragenDialog : Window
{
    public GewichtEintragenDialog()
    {
        InitializeComponent();
    }

    private void Speichern_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is GewichtEintragenViewModel vm && vm.Validieren())
            DialogResult = true;
    }
}
