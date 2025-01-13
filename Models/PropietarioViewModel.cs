using rivarola_riquelme_rastrilla.Models;

public class PropietarioViewModel
{
    public List<Propietarios>? Propietarios { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}