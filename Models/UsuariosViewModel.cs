using rivarola_riquelme_rastrilla.Models;

public class UsuariosViewModel
{
    public IEnumerable<Usuarios>? Usuarios { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}