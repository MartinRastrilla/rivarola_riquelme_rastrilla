using System.ComponentModel.DataAnnotations;

namespace rivarola_riquelme_rastrilla.Models;

public class Propietarios
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [Range(7,8, ErrorMessage = "El DNI debe tener exactamente 8 caracteres.")]
    public int Dni { get; set; }

    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El apellido no debe contener números ni caracteres especiales.")]
    [Required(ErrorMessage = "El apellido es obligatorio.")]
    public string Apellido { get; set; } = "";

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre no debe contener números ni caracteres especiales.")]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [RegularExpression(@"^\d+$", ErrorMessage = "El teléfono debe contener solo números.")]
    [Phone(ErrorMessage = "El formato del teléfono no es válido.")]
    public string Telefono { get; set; } = "";

    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
    public string Email { get; set; } = "";
}

