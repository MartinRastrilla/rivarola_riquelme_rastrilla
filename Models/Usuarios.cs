using System.ComponentModel.DataAnnotations.Schema;
namespace rivarola_riquelme_rastrilla.Models;
public class Usuarios
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
    public string Contrasenia { get; set; }
    public string Rol { get; set; }
    public string Avatar { get; set; }
    public IFormFile AvatarFile { get; set; }
}