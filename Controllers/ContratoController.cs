using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rivarola_riquelme_rastrilla.Models;

namespace rivarola_riquelme_rastrilla.Controllers;

public class ContratoController : Controller
{
    private readonly ILogger<ContratoController> _logger;
    private RepositorioContrato repo = new RepositorioContrato();
    private RepositorioInquilino repoInquilino = new RepositorioInquilino();
    private RepositorioInmueble repoInmueble = new RepositorioInmueble();

    public ContratoController(ILogger<ContratoController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Index()
    {
        var lista = repo.ObtenerContratos();
        return View(lista);
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Editar(long Id)
    {
        var contrato = repo.Obtener(Id);
        ViewBag.inquilinos = repoInquilino.ObtenerInquilinos();
        ViewBag.inmuebles = repoInmueble.ObtenerInmueble();
        return View(contrato);
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Details(long Id)
    {
        var contrato = repo.Obtener(Id);
        if (contrato == null)
        {
            return NotFound();
        }
        return View(contrato);
    }


    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult AltaContrato()
    {
        ViewBag.inquilinos = repoInquilino.ObtenerInquilinos();
        ViewBag.inmuebles = repoInmueble.ObtenerInmueble();
        return View();
    }

    [HttpPost]
    [Authorize(Policy = "Empleado")]
    public IActionResult AltaContrato(Contratos contrato)
    {
        int r = repo.AltaContrato(contrato);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = "Administrador")]
    public IActionResult Delete(long Id)
    {
        var contrato = repo.Obtener(Id);
        if (contrato == null)
        {
            return NotFound();
        }
        return View(contrato);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Policy = "Administrador")]
    public IActionResult DeleteConfirmed(long Id)
    {
        var result = repo.BorrarContrato(Id);
        if (result > 0)
        {
            return RedirectToAction(nameof(Index));
        }
        else
        {
            ModelState.AddModelError("", "No se pudo eliminar el Contrato.");
            return View();
        }
    }

    [HttpPost]
    [Authorize(Policy = "Empleado")]
    public IActionResult Guardar(Contratos contrato)
    {

        repo.Guardar(contrato);
        return RedirectToAction(nameof(Index));
    }
}