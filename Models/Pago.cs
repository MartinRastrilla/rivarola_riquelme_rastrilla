using System.ComponentModel.DataAnnotations.Schema;
//OBLIGATORIO PARA LAS VALIDACIONES
using System.ComponentModel.DataAnnotations;

namespace rivarola_riquelme_rastrilla.Models
{
    public class Pago
    {
        public int Id { get; set; }

        [ForeignKey("Contratos")]
        public int? Contrato_id { get; set; }
        public Contratos? Contrato { get; set; }

        [Required(ErrorMessage = "La fecha de pago es obligatoria.")]
        [DataType(DataType.Date, ErrorMessage = "La fecha de pago debe tener un formato válido.")]
        public DateTime? Fecha_pago { get; set; }

        [Required(ErrorMessage = "El detalle no puede estar vacio.")]
        [StringLength(255, ErrorMessage = "El detalle no puede tener más de 255 caracteres.")]
        public string? Detalle { get; set; }

        public bool Activo { get; set; } = true;

        [Required(ErrorMessage = "El importe es obligatorio.")]
        public decimal? Importe { get; set; }
    }
}
