using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using rivarola_riquelme_rastrilla.Models;

public class RegistroContratos : Controller
{
    public int Id { get; set; }

    [ForeignKey("Contratos")]
    public int Contrato_id { get; set; }

    public Contratos? Contrato { get; set; }
    public DateTime Fecha_creacion { get; set; }

    [ForeignKey("Usuarios")]
    public int Creado_por { get; set; }
    public DateTime? Fecha_cancelacion { get; set; }
    [ForeignKey("Usuarios")]
    public int? Cancelado_por { get; set; }
}