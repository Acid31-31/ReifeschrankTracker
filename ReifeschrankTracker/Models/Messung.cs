using System;
using System.ComponentModel.DataAnnotations;

namespace ReifeschrankTracker.Models;

public class Messung
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ChargeId { get; set; }

    public DateTime Zeitpunkt { get; set; } = DateTime.Now;

    public int GewichtG { get; set; }

    public string? Notiz { get; set; }

    public Charge? Charge { get; set; }
}
