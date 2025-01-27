using System.ComponentModel.DataAnnotations.Schema;

namespace rivarola_riquelme_rastrilla.Models;
public class Multa
{
    public int Id { get; set; }
    
    [ForeignKey("Contratos")]
    public int Contrato_id { get; set; }
    public Contratos? Contrato { get; set; }

    public decimal Monto { get; set; }
    public DateTime Fecha_multa { get; set; }

}