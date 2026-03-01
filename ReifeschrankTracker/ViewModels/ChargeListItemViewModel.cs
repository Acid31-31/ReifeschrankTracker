using ReifeschrankTracker.Models;
using System;
using System.Linq;

namespace ReifeschrankTracker.ViewModels;

public class ChargeListItemViewModel : ViewModelBase
{
    private Charge _charge;

    public ChargeListItemViewModel(Charge charge)
    {
        _charge = charge;
    }

    public Charge Charge => _charge;
    public Guid Id => _charge.Id;
    public string Produktname => _charge.Produktname;
    public string? ChargeCode => _charge.ChargeCode;
    public ChargeStatus Status => _charge.Status;

    public decimal VerlustProzent
    {
        get
        {
            if (_charge.Messungen.Count == 0 || _charge.StartgewichtG == 0) return 0;
            var letztes = _charge.Messungen.OrderByDescending(m => m.Zeitpunkt).First().GewichtG;
            return Math.Round((decimal)(_charge.StartgewichtG - letztes) / _charge.StartgewichtG * 100, 1);
        }
    }

    public string WarnBadge
    {
        get
        {
            var v = VerlustProzent;
            if (v >= 40) return "🔴 ≥40%";
            if (v >= 35) return "🟠 ≥35%";
            if (v >= 30) return "🟡 ≥30%";
            return string.Empty;
        }
    }

    public string DisplayName => string.IsNullOrWhiteSpace(ChargeCode)
        ? Produktname
        : $"{Produktname} [{ChargeCode}]";

    public void Refresh()
    {
        OnPropertyChanged(nameof(VerlustProzent));
        OnPropertyChanged(nameof(WarnBadge));
        OnPropertyChanged(nameof(Status));
        OnPropertyChanged(nameof(Produktname));
        OnPropertyChanged(nameof(DisplayName));
    }
}
