namespace rivarola_riquelme_rastrilla.Models
{
    public class Pago
    {
        public required int Id { get; set; }
        public int? Contrato_id { get; set; }
        public DateTime? Fecha_pago { get; set; }
        public string? Detalle { get; set; }
        public decimal? Importe { get; set; }
    }
}
