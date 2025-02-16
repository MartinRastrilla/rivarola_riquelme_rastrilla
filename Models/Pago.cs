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

        public int? Num_pago { get; set; }

        public DateTime? Fecha_pago { get; set; }

        public string? Detalle { get; set; }

        public bool Activo { get; set; } = true;

        public decimal? Importe { get; set; }
    }
}
