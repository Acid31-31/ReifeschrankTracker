using Microsoft.EntityFrameworkCore;
using ReifeschrankTracker.Data;
using ReifeschrankTracker.Models;
using ReifeschrankTracker.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ReifeschrankTracker.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly AppDbContext _db;
    private ChargeListItemViewModel? _selectedCharge;
    private ChargeDetailViewModel? _detail;

    public MainViewModel()
    {
        _db = new AppDbContext();
        _db.Database.EnsureCreated();
        Chargen = new ObservableCollection<ChargeListItemViewModel>();
        LadeChargen();

        NeuChargeBefehl = new RelayCommand(NeuCharge);
        GewichtEintragenBefehl = new RelayCommand(GewichtEintragen, _ => _selectedCharge != null);
        ChargeLoeschenBefehl = new RelayCommand(ChargeLöschen, _ => _selectedCharge != null);
    }

    public ObservableCollection<ChargeListItemViewModel> Chargen { get; }

    public ChargeListItemViewModel? AusgewählteCharge
    {
        get => _selectedCharge;
        set
        {
            if (SetField(ref _selectedCharge, value))
            {
                if (value != null)
                {
                    var charge = _db.Chargen
                        .Include(c => c.Messungen)
                        .FirstOrDefault(c => c.Id == value.Id);
                    Detail = charge != null ? new ChargeDetailViewModel(charge) : null;
                }
                else
                {
                    Detail = null;
                }
                OnPropertyChanged(nameof(HatAuswahl));
            }
        }
    }

    public ChargeDetailViewModel? Detail
    {
        get => _detail;
        private set => SetField(ref _detail, value);
    }

    public bool HatAuswahl => _selectedCharge != null;

    public ICommand NeuChargeBefehl { get; }
    public ICommand GewichtEintragenBefehl { get; }
    public ICommand ChargeLoeschenBefehl { get; }

    private void LadeChargen()
    {
        Chargen.Clear();
        var list = _db.Chargen.Include(c => c.Messungen).OrderByDescending(c => c.ErstelltAm).ToList();
        foreach (var c in list)
            Chargen.Add(new ChargeListItemViewModel(c));
    }

    private void NeuCharge(object? _)
    {
        var vm = new NeuChargeViewModel();
        var dlg = new NeuChargeDialog { DataContext = vm };
        dlg.Owner = Application.Current.MainWindow;
        if (dlg.ShowDialog() == true)
        {
            var charge = vm.ToCharge();
            _db.Chargen.Add(charge);
            _db.SaveChanges();
            LadeChargen();
            var item = Chargen.FirstOrDefault(c => c.Id == charge.Id);
            if (item != null) AusgewählteCharge = item;
        }
    }

    private void GewichtEintragen(object? _)
    {
        if (_selectedCharge == null) return;
        var vm = new GewichtEintragenViewModel();
        var dlg = new GewichtEintragenDialog { DataContext = vm };
        dlg.Owner = Application.Current.MainWindow;
        if (dlg.ShowDialog() == true)
        {
            var messung = vm.ToMessung(_selectedCharge.Id);
            _db.Messungen.Add(messung);

            var charge = _db.Chargen.Include(c => c.Messungen).First(c => c.Id == _selectedCharge.Id);
            charge.GeaendertAm = DateTime.Now;

            bool zielErreicht = false;
            if (charge.ZielTyp == ZielTyp.Prozent && charge.ZielProzent.HasValue)
            {
                var verlustP = (decimal)(charge.StartgewichtG - messung.GewichtG) / charge.StartgewichtG * 100;
                zielErreicht = verlustP >= charge.ZielProzent.Value;
            }
            else if (charge.ZielTyp == ZielTyp.Gewicht && charge.ZielGewichtG.HasValue)
            {
                zielErreicht = messung.GewichtG <= charge.ZielGewichtG.Value;
            }

            if (zielErreicht && charge.Status == ChargeStatus.Aktiv)
            {
                var result = MessageBox.Show(
                    $"Ziel erreicht! Charge \"{charge.Produktname}\" als Fertig markieren?",
                    "Ziel erreicht",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                    charge.Status = ChargeStatus.Fertig;
            }

            _db.SaveChanges();
            LadeChargen();
            var item = Chargen.FirstOrDefault(c => c.Id == _selectedCharge.Id);
            if (item != null) AusgewählteCharge = item;
        }
    }

    private void ChargeLöschen(object? _)
    {
        if (_selectedCharge == null) return;
        var result = MessageBox.Show(
            $"Charge \"{_selectedCharge.Produktname}\" wirklich löschen?",
            "Löschen bestätigen",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);
        if (result == MessageBoxResult.Yes)
        {
            var charge = _db.Chargen.Find(_selectedCharge.Id);
            if (charge != null)
            {
                _db.Chargen.Remove(charge);
                _db.SaveChanges();
            }
            LadeChargen();
            AusgewählteCharge = null;
        }
    }
}
