using rivarola_riquelme_rastrilla.Models;

public class ContratoViewModel
{
    public List<Contratos>? Contratos { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int InmuebleId { get; set; }
    public List<Multa>? Multas { get; set; }
}
