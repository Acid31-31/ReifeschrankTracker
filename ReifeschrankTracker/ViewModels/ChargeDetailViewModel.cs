using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Defaults;
using ReifeschrankTracker.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ReifeschrankTracker.ViewModels;

public class ChargeDetailViewModel : ViewModelBase
{
    private Charge _charge;

    public ChargeDetailViewModel(Charge charge)
    {
        _charge = charge;
        Refresh();
    }

    public Charge Charge => _charge;

    public string Produktname => _charge.Produktname;
    public string? ChargeCode => _charge.ChargeCode;
    public int StartgewichtG => _charge.StartgewichtG;
    public DateTime Startdatum => _charge.Startdatum;
    public ChargeStatus Status => _charge.Status;
    public Methode Methode => _charge.Methode;
    public string? Notizen => _charge.Notizen;

    public int? LetztesGewichtG
    {
        get
        {
            var last = _charge.Messungen.OrderBy(m => m.Zeitpunkt).LastOrDefault();
            return last?.GewichtG;
        }
    }

    public int VerlustG => StartgewichtG - (LetztesGewichtG ?? StartgewichtG);

    public decimal VerlustProzent => StartgewichtG > 0
        ? Math.Round((decimal)VerlustG / StartgewichtG * 100, 1)
        : 0m;

    public int TageSeitStart => (int)(DateTime.Now - Startdatum).TotalDays;

    public string WarnBadge
    {
        get
        {
            var v = VerlustProzent;
            if (v >= 40) return "🔴 Verlust ≥ 40%";
            if (v >= 35) return "🟠 Verlust ≥ 35%";
            if (v >= 30) return "🟡 Verlust ≥ 30%";
            return string.Empty;
        }
    }

    public bool HatWarnung => VerlustProzent >= 30;

    public string ZielAnzeige
    {
        get
        {
            if (_charge.ZielTyp == ZielTyp.Prozent)
                return $"{_charge.ZielProzent:0.#} %";
            return $"{_charge.ZielGewichtG} g";
        }
    }

    public bool ZielErreicht
    {
        get
        {
            if (!LetztesGewichtG.HasValue) return false;
            if (_charge.ZielTyp == ZielTyp.Prozent)
                return _charge.ZielProzent.HasValue && VerlustProzent >= _charge.ZielProzent.Value;
            return _charge.ZielGewichtG.HasValue && LetztesGewichtG.Value <= _charge.ZielGewichtG.Value;
        }
    }

    public ObservableCollection<MessungViewModel> Messungen { get; } = new();

    public ISeries[] DiagrammReihen { get; private set; } = Array.Empty<ISeries>();

    public Axis[] XAchsen { get; private set; } = Array.Empty<Axis>();

    public void Refresh()
    {
        Messungen.Clear();
        foreach (var m in _charge.Messungen.OrderBy(m => m.Zeitpunkt))
            Messungen.Add(new MessungViewModel(m, _charge.StartgewichtG));

        var punkte = _charge.Messungen
            .OrderBy(m => m.Zeitpunkt)
            .Select(m => new DateTimePoint(m.Zeitpunkt, m.GewichtG))
            .ToList();

        DiagrammReihen = new ISeries[]
        {
            new LineSeries<DateTimePoint>
            {
                Values = punkte,
                Name = "Gewicht (g)",
                GeometrySize = 8
            }
        };

        XAchsen = new Axis[]
        {
            new DateTimeAxis(TimeSpan.FromDays(1), d => d.ToString("dd.MM"))
        };

        OnPropertyChanged(nameof(LetztesGewichtG));
        OnPropertyChanged(nameof(VerlustG));
        OnPropertyChanged(nameof(VerlustProzent));
        OnPropertyChanged(nameof(WarnBadge));
        OnPropertyChanged(nameof(HatWarnung));
        OnPropertyChanged(nameof(TageSeitStart));
        OnPropertyChanged(nameof(Status));
        OnPropertyChanged(nameof(ZielErreicht));
        OnPropertyChanged(nameof(Messungen));
        OnPropertyChanged(nameof(DiagrammReihen));
        OnPropertyChanged(nameof(XAchsen));
    }
}
