using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rivarola_riquelme_rastrilla.Models;

namespace rivarola_riquelme_rastrilla.Controllers;

public class InquilinoController : Controller
{
    private readonly ILogger<InquilinoController> _logger;
    private RepositorioInquilino repo = new RepositorioInquilino();

    public InquilinoController(ILogger<InquilinoController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Index()
    {

        if (User.Identity.IsAuthenticated)
        {
            var lista = repo.ObtenerInquilinos();
            return View(lista);
        }
        else
        {
            return RedirectToAction("Login", "Home");
        }
        
        
    }
    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult AltaInquilino()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Policy = "Empleado")]
    public IActionResult AltaInquilino(Inquilino inquilino)
    {
        int r = repo.AltaInquilino(inquilino);
        return RedirectToAction(nameof(Index));

    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Editar(long Id)
    {
        var inquilino = repo.Obtener(Id);
        return View(inquilino);
    }

    [HttpPost]
    [Authorize(Policy = "Empleado")]
    public IActionResult Guardar(Inquilino inquilino)
    {
        repo.EditarInquilino(inquilino);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Policy = "Administrador")]
    public IActionResult Baja(long Dni)
    {

        var result = repo.BorrarInquilino(Dni);
        if (result > 0)
        {
            return RedirectToAction(nameof(Index));
        }
        else
        {
            ModelState.AddModelError("", "No se pudo eliminar el inquilino.");
            return View();
        }
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Details(long Id)
    {
        var inquilino = repo.Obtener(Id);
        if (inquilino == null)
        {
            return NotFound();
        }
        return View(inquilino);
    }

    [HttpGet]
    [Authorize(Policy = "Administrador")]
    public IActionResult Delete(long Id)
    {
        var inquilino = repo.Obtener(Id);
        if (inquilino == null)
        {
            return NotFound();
        }
        return View(inquilino);
    }
}

