namespace rivarola_riquelme_rastrilla.Controllers;

public class Propietario
{
    //si una variable puede tener null se pone string? Nombre 

    public long Dni { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public long Telefono { get; set; }
    public string Email { get; set; }
}