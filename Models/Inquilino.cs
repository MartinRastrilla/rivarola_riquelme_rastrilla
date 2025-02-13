namespace rivarola_riquelme_rastrilla.Models;
public class Inquilino
{
    public long Id { get; set; }
    public long Dni { get; set; }
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public long? Telefono { get; set; }
    public string? Email { get; set; }
}