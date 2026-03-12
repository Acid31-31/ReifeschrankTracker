using System;
using System.Windows;
using ReifeManager_R01.Models;

namespace ReifeManager_R01.Views;

public partial class UpdateCheckWindow : Window
{
    private string _downloadUrl = "https://github.com/Acid31-31/ReifeschrankTracker/releases";

    public UpdateCheckWindow(string currentVersion, UpdateInfo? available)
    {
        InitializeComponent();
        
        if (available is not null)
        {
            _downloadUrl = available.DownloadUrl ?? "https://github.com/Acid31-31/ReifeschrankTracker/releases";
        }
        
        var currentDisplay = FormatVersionForDisplay(currentVersion);
        var availableDisplay = available is null ? "Keine neue Version verfügbar" : FormatVersionForDisplay(available.Version);

        DataContext = new
        {
            CurrentVersion = currentDisplay,
            AvailableVersion = availableDisplay,
            StatusMessage = available is not null
                ? $"✅ Neue Version v{availableDisplay} ist verfügbar!\n\nBitte klicken Sie auf 'GitHub öffnen' oder laden Sie manual herunter."
                : "✓ Sie verwenden bereits die neueste Version."
        };
    }

    private static string FormatVersionForDisplay(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return "unbekannt";
        }

        var clean = raw.Trim().TrimStart('v', 'V');
        if (!Version.TryParse(clean, out var version))
        {
            return clean;
        }

        if (version.Revision >= 0)
        {
            return $"{version.Major}.{version.Minor}.{version.Revision}";
        }

        if (version.Build >= 0)
        {
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }

        return $"{version.Major}.{version.Minor}";
    }

    private void OnGitHubClick(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_downloadUrl))
            {
                _downloadUrl = "https://github.com/Acid31-31/ReifeschrankTracker/releases";
            }

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(_downloadUrl)
            {
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Fehler beim Öffnen von GitHub:\n\n{ex.Message}",
                "Fehler",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void OnCloseClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
