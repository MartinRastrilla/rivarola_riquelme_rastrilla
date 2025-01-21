using rivarola_riquelme_rastrilla.Models;

public class InmuebleViewModel
{
    public List<Inmueble>? Inmuebles { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}