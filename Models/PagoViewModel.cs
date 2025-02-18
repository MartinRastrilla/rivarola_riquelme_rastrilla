using rivarola_riquelme_rastrilla.Models;

public class PagoViewModel
{
    public List<Pago>? Pagos { get; set; }
    public Pago? Pago { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int ContratoId { get; set; }
    public Contratos? Contrato { get; set; }
    public Multa? Multa { get; set; }
}