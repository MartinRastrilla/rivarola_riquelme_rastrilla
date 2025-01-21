using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rivarola_riquelme_rastrilla.Models;

namespace rivarola_riquelme_rastrilla.Controllers;
public class InmuebleController : Controller
{
    private readonly ILogger<InmuebleController> _logger;

    private RepositorioPropietario repoPropietario = new RepositorioPropietario();
    private RepositorioInmueble repoInmueble = new RepositorioInmueble();
    private RepositorioTipo repoTipo = new RepositorioTipo();


    public InmuebleController(ILogger<InmuebleController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Index()
    {
        var lista = repoInmueble.ObtenerInmueble();
        var tipos = repoTipo.ObtenerTipos();
        ViewBag.Tipos = tipos;
        ViewBag.Inmuebles = lista;
        
        return View();
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult AltaInmueble()
    {
        ViewBag.propietarios = repoPropietario.ObtenerTodos();
        ViewBag.tipos = repoTipo.ObtenerTipos();
        return View();
    }

    [HttpPost]
    [Authorize(Policy = "Empleado")]
    public IActionResult AltaInmueble(Inmueble Inmueble)
    {
        bool estado = Request.Form["Estado"] == "true";
        Inmueble.Estado = estado;
        int r = repoInmueble.AltaInmueble(Inmueble);
        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    [Authorize(Policy = "Administrador")]
    public IActionResult Delete(int Id)
    {
        var inmueble = repoInmueble.Obtener(Id);
        if (inmueble == null)
        {
            return NotFound();
        }
        return View(inmueble);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Policy = "Administrador")]
    public IActionResult DeleteConfirmed(int Id)
    {
        repoInmueble.BajaInmueble(Id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = "Administrador")]
    public IActionResult Baja(int Id)
    {
        var result = repoInmueble.BajaInmueble(Id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Activar(int Id)
    {
        var result = repoInmueble.ActivarInmueble(Id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Editar(int Id)
    {
        var inmueble = repoInmueble.Obtener(Id);
        ViewBag.propietarios = repoPropietario.ObtenerTodos();
        ViewBag.tipos = repoTipo.ObtenerTipos();
        return View(inmueble);
    }


    [HttpPost]
    [Authorize(Policy = "Empleado")]
    public IActionResult Guardar(Inmueble inmueble)
    {

        repoInmueble.GuardarInmueble(inmueble);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Details(int Id)
    {
        var inmueble = repoInmueble.Obtener(Id);
        if (inmueble == null)
        {
            return NotFound();
        }
        return View(inmueble);
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult FiltrarTipo(int tipoInmueble)
    {
        if (tipoInmueble == 0) return RedirectToAction("Index");

        var inmueble = repoInmueble.FiltrarTipo(tipoInmueble);

        if (inmueble == null || !inmueble.Any())
        {
            TempData["Error"] = $"No se encontraron propiedades.";
            return RedirectToAction("Index");
        }
        var tipos = repoTipo.ObtenerTipos();
        ViewBag.Tipos = tipos;
        ViewBag.Inmuebles = inmueble; // Usamos ViewBag para pasar los inmuebles
        return View("Index"); // Volver a la vista principal
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult FiltrarPrecio(decimal? precioMin, decimal? precioMax)
    {
        if (!precioMin.HasValue) precioMin = 0;
        if (!precioMax.HasValue) precioMax = decimal.MaxValue;

        var inmueble = repoInmueble.FiltrarPrecio(precioMin.Value, precioMax.Value);

        if (inmueble == null || !inmueble.Any())
        {
            TempData["Error"] = $"No se encontraron inmuebles en el rango de precios especificado.";
            return RedirectToAction("Index");
        }
        var tipos = repoTipo.ObtenerTipos();
        ViewBag.Tipos = tipos;
        ViewBag.Inmuebles = inmueble; // Usamos ViewBag para pasar los inmuebles
        return View("Index"); // Volver a la vista principal
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult FiltrarUso(string usoInmueble)
    {
        if (string.IsNullOrEmpty(usoInmueble)) return RedirectToAction("Index");

        var inmueble = repoInmueble.FiltrarUso(usoInmueble);

        if (inmueble == null || !inmueble.Any())
        {
            TempData["Error"] = $"No se encontraron inmuebles con el uso especificado.";
            return RedirectToAction("Index");
        }
        
        var tipos = repoTipo.ObtenerTipos();
        ViewBag.Tipos = tipos;

        ViewBag.Inmuebles = inmueble; // Usamos ViewBag para pasar los inmuebles
        return View("Index"); // Volver a la vista principal
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult FiltrarAmbientes(int? ambientes)
    {
        if (!ambientes.HasValue)
        {
            ambientes = 0;
        }

        var inmuebles = repoInmueble.FiltrarAmbientes(ambientes.Value);

        if (inmuebles == null || !inmuebles.Any())
        {
            TempData["Error"] = $"No se encontraron inmuebles con el número de ambientes especificado.";
            return RedirectToAction("Index");
        }
        var tipos = repoTipo.ObtenerTipos();
        ViewBag.Tipos = tipos;
        ViewBag.Inmuebles = inmuebles; // Usamos ViewBag para pasar los inmuebles
        return View("Index"); // Volver a la vista principal
    }
}
