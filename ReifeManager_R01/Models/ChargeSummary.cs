namespace ReifeManager_R01.Models;

public class ChargeSummary
{
    public Guid Id { get; set; }
    public string Bezeichnung { get; set; } = string.Empty;
    public string Fleischtyp { get; set; } = string.Empty;
    public DateTime Startdatum { get; set; }
    public double ZielverlustProzent { get; set; }
    public int AnzahlStuecke { get; set; }
    public int ReifetageGesamt { get; set; }
    public double DurchschnittlicherVerlust { get; set; }
    public ReifeStatus Status { get; set; }
    public DateTime ExportiertAm { get; set; }
}
