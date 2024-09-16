using System.ComponentModel.DataAnnotations.Schema;

namespace rivarola_riquelme_rastrilla.Models;
public class Contratos
{
    public int Id { get; set; }



    [ForeignKey("Inquilinos")]
    public long Inquilino_dni { get; set; }
    public Inquilino Inquilino { get; set; }


    [ForeignKey("Inmuebles")]
    public int Inmueble_id { get; set; }
    public Inmueble Inmueble { get; set; }


    public enum EstadoContrato
    {
        Activo = 0,
        Finalizado = 1,
        Cancelado = 2
    }


    public EstadoContrato Estado { get; set; }
    public decimal Monto { get; set; }
    public DateTime Fecha_inicio { get; set; }
    public DateTime Fecha_fin { get; set; }
}