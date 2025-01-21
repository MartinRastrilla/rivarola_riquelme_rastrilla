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
        return View(lista);
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
    public IActionResult AltaInmueble(Inmueble Inmueble, int propietario_dni)
    {
        Propietarios? propietario = repoPropietario.ObtenerPorDni(propietario_dni);
        if (propietario == null)
        {
            ViewBag.Error = "No se encontró el propietario con el DNI ingresado.";
            ViewBag.propietarios = repoPropietario.ObtenerTodos();
            ViewBag.tipos = repoTipo.ObtenerTipos();
            return View(Inmueble);
        }
        bool estado = Request.Form["Estado"] == "true";
        Inmueble.Estado = estado;
        int r = repoInmueble.AltaInmueble(Inmueble);
        TempData["ToastMessage"] = "Inmueble agregado con exito";
        TempData["ToastType"] = "success";
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
        TempData["ToastMessage"] = "Inmueble eliminado con exito";
        TempData["ToastType"] = "danger";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = "Administrador")]
    public IActionResult Baja(int Id)
    {
        var result = repoInmueble.DesactivarInmueble(Id);
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
        ViewBag.inmueble = inmueble;
        return View(inmueble);
    }


    [HttpPost]
    [Authorize(Policy = "Empleado")]
    public IActionResult Guardar(Inmueble inmueble, int propietario_dni)
    {
        Propietarios? propietario = repoPropietario.ObtenerPorDni(propietario_dni);
        if (propietario == null)
        {
            ViewBag.Error = "No se encontró el propietario con el DNI ingresado.";
            ViewBag.propietarios = repoPropietario.ObtenerTodos();
            ViewBag.tipos = repoTipo.ObtenerTipos();
            ViewBag.inmueble = inmueble;
            return View("Editar",inmueble);
        }
        repoInmueble.GuardarInmueble(inmueble);
        TempData["ToastMessage"] = "Inmueble editado con exito";
        TempData["ToastType"] = "success";
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



}
