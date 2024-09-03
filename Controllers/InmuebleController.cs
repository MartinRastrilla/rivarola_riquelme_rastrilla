using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using rivarola_riquelme_rastrilla.Models;

namespace rivarola_riquelme_rastrilla.Controllers;
public class InmuebleController : Controller
{
    private readonly ILogger<InmuebleController> _logger;

    private RepositorioPropietario repoPropietario = new RepositorioPropietario();
    private RepositorioInmueble repoInmueble = new RepositorioInmueble();

    public InmuebleController(ILogger<InmuebleController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var lista = repoInmueble.ObtenerInmueble();
        return View(lista);
    }

    [HttpGet]
    public IActionResult AltaInmueble()
    {
        ViewBag.propietarios = repoPropietario.ObtenerTodos();
        return View();
    }

    [HttpPost]
    public IActionResult AltaInmueble(Inmueble Inmueble)
    {
        bool estado = Request.Form["Estado"] == "true";
        Inmueble.Estado = estado;
        int r = repoInmueble.AltaInmueble(Inmueble);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Baja(int Id)
    {
        var result = repoInmueble.BajaInmueble(Id);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Activar(int Id)
    {
        var result = repoInmueble.ActivarInmueble(Id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Editar(int Id)
    {
        var inmueble = repoInmueble.Obtener(Id);
        ViewBag.propietarios = repoPropietario.ObtenerTodos();
        return View(inmueble);
    }

    [HttpPost]
    public IActionResult Guardar(Inmueble inmueble)
    {

        repoInmueble.GuardarInmueble(inmueble);
        return RedirectToAction(nameof(Index));
    }

}
