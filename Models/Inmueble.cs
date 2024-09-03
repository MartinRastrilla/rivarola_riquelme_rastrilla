using System.ComponentModel.DataAnnotations.Schema;
using rivarola_riquelme_rastrilla.Controllers;

namespace rivarola_riquelme_rastrilla.Models;


public class Inmueble
{
    public int Id { get; set; }
    public string Direccion { get; set; }


    public enum UsoInmueble
    {
        Comercial,
        Residencial
    }


    public UsoInmueble Uso { get; set; }
    public string Tipo { get; set; }
    public int Ambientes { get; set; }
    public string Coordenadas { get; set; }
    public decimal Precio { get; set; }


    [ForeignKey("Propietario")]
    public long Propietario_dni { get; set; }
    public Propietario Propietario { get; set; }


    public bool Estado { get; set; }

}