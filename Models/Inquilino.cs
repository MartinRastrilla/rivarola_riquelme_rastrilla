namespace rivarola_riquelme_rastrilla.Models;
public class Inquilino
{
    public required long Dni { get; set; }
    public required string Nombre { get; set; }
    public required string Apellido { get; set; }
    public required long Telefono { get; set; }
    public required string Email { get; set; }
}