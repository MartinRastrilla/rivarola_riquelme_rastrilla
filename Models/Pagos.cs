namespace rivarola_riquelme_rastrilla.Models;

public class Pagos
{
    public int Id { get; set; }

    public int ContratoId { get; set; }

    public DateTime Fecha_pago { get; set; }

    public string Detalle { get; set; }

    public float Importe { get; set; }
}