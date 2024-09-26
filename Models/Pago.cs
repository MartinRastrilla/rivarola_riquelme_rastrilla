using System.ComponentModel.DataAnnotations.Schema;

namespace rivarola_riquelme_rastrilla.Models
{
    public class Pago
    {
        public required int Id { get; set; }

        [ForeignKey("Contratos")]
        public int? Contrato_id { get; set; }
        public Contratos? Contrato { get; set; }
        public DateTime? Fecha_pago { get; set; }
        public string? Detalle { get; set; }
        public decimal? Importe { get; set; }
    }
}
