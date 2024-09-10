using System.ComponentModel.DataAnnotations.Schema;
namespace rivarola_riquelme_rastrilla.Models;
public class UserRoles
{
    public Usuarios Usuario { get; set; }
    public Roles Rol { get; set; }
}