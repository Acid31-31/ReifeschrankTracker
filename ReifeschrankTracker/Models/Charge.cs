using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReifeschrankTracker.Models;

public class Charge
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string Produktname { get; set; } = string.Empty;

    public string? ChargeCode { get; set; }

    public Methode Methode { get; set; }

    public DateTime Startdatum { get; set; } = DateTime.Now;

    public int StartgewichtG { get; set; }

    public ZielTyp ZielTyp { get; set; }

    public decimal? ZielProzent { get; set; }

    public int? ZielGewichtG { get; set; }

    public string? Notizen { get; set; }

    public ChargeStatus Status { get; set; } = ChargeStatus.Aktiv;

    public DateTime ErstelltAm { get; set; } = DateTime.Now;

    public DateTime GeaendertAm { get; set; } = DateTime.Now;

    public List<Messung> Messungen { get; set; } = new();
}
