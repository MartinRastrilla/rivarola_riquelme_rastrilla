using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using rivarola_riquelme_rastrilla.Models;

public class RegistroPagos
{
    public int Id { get; set; }
    public int? Pago_id { get; set; }
    [ForeignKey("Pagos")]
    public Pago? Pago { get; set; }
    public int? Creado_por { get; set; }
    [ForeignKey("Usuarios")]
    public Usuarios? Usuario { get; set; }
    public DateTime? Fecha_creacion { get; set; }
    public int? Anulado_por { get; set; }
    [ForeignKey("Usuarios")]
    public Usuarios? Usuario2 { get; set; }
    public DateTime? Fecha_anulacion { get; set; }
}