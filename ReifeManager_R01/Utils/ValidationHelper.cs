namespace ReifeManager_R01.Utils;

public static class ValidationHelper
{
    public static bool IsValidWeight(double weight)
    {
        return weight > 0 && weight < 1000000;
    }

    public static bool IsValidTemperature(double temp)
    {
        return temp >= -50 && temp <= 100;
    }

    public static bool IsValidHumidity(double humidity)
    {
        return humidity >= 0 && humidity <= 100;
    }

    public static bool IsValidLossPercentage(double percentage)
    {
        return percentage > 0 && percentage <= 100;
    }

    public static bool IsValidName(string? name)
    {
        return !string.IsNullOrWhiteSpace(name) && name.Length <= 200;
    }
}
