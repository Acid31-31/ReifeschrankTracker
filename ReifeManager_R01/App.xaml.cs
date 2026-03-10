using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ReifeManager_R01;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        EventManager.RegisterClassHandler(
            typeof(DatePicker),
            FrameworkElement.LoadedEvent,
            new RoutedEventHandler(OnDatePickerLoaded));
    }

    private static void OnDatePickerLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not DatePicker datePicker)
        {
            return;
        }

        datePicker.CalendarOpened -= OnDatePickerCalendarOpened;
        datePicker.CalendarOpened += OnDatePickerCalendarOpened;
    }

    private static void OnDatePickerCalendarOpened(object? sender, RoutedEventArgs e)
    {
        if (sender is not DatePicker datePicker)
        {
            return;
        }

        datePicker.ApplyTemplate();

        if (datePicker.Template.FindName("PART_Popup", datePicker) is not Popup popup)
        {
            return;
        }

        if (popup.Child is not FrameworkElement popupRoot)
        {
            return;
        }

        var darkBackground = (Brush)new BrushConverter().ConvertFromString("#1E1E1E")!;
        var midBackground = (Brush)new BrushConverter().ConvertFromString("#2A2A2A")!;
        var lightText = (Brush)new BrushConverter().ConvertFromString("#F3F3F3")!;
        var border = (Brush)new BrushConverter().ConvertFromString("#555555")!;

        // Rekursiv ALLE Elemente dunkel einfärben
        ApplyDarkThemeRecursive(popupRoot, darkBackground, midBackground, lightText, border);

        var calendar = FindVisualChild<Calendar>(popupRoot);
        if (calendar is null)
        {
            return;
        }

        calendar.Background = darkBackground;
        calendar.Foreground = lightText;
        calendar.BorderBrush = border;

        var dayStyle = new Style(typeof(CalendarDayButton));
        dayStyle.Setters.Add(new Setter(Control.BackgroundProperty, midBackground));
        dayStyle.Setters.Add(new Setter(Control.ForegroundProperty, lightText));
        dayStyle.Setters.Add(new Setter(Control.BorderBrushProperty, border));
        dayStyle.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(1)));
        calendar.Resources[typeof(CalendarDayButton)] = dayStyle;

        var monthStyle = new Style(typeof(CalendarButton));
        monthStyle.Setters.Add(new Setter(Control.BackgroundProperty, midBackground));
        monthStyle.Setters.Add(new Setter(Control.ForegroundProperty, lightText));
        monthStyle.Setters.Add(new Setter(Control.BorderBrushProperty, border));
        monthStyle.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(1)));
        calendar.Resources[typeof(CalendarButton)] = monthStyle;

        var itemStyle = new Style(typeof(CalendarItem));
        itemStyle.Setters.Add(new Setter(Control.BackgroundProperty, darkBackground));
        itemStyle.Setters.Add(new Setter(Control.ForegroundProperty, lightText));
        calendar.Resources[typeof(CalendarItem)] = itemStyle;
    }

    private static void ApplyDarkThemeRecursive(DependencyObject root, Brush darkBackground, Brush midBackground, Brush lightText, Brush border)
    {
        switch (root)
        {
            case Border b:
                b.Background = darkBackground;
                b.BorderBrush = border;
                break;
            case Panel p:
                p.Background = darkBackground;
                break;
            case Control c:
                c.Background = midBackground;
                c.Foreground = lightText;
                c.BorderBrush = border;
                break;
            case TextBlock t:
                t.Foreground = lightText;
                break;
        }

        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
        {
            ApplyDarkThemeRecursive(VisualTreeHelper.GetChild(root, i), darkBackground, midBackground, lightText, border);
        }
    }

    private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T typedChild)
            {
                return typedChild;
            }

            var result = FindVisualChild<T>(child);
            if (result is not null)
            {
                return result;
            }
        }

        return null;
    }
}


