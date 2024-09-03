using System.Diagnostics;
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
    public IActionResult Index()
    {
        var lista = repo.ObtenerInquilinos();
        return View(lista);
    }
    [HttpGet]
    public IActionResult AltaInquilino()
    {
        return View();
    }

    [HttpPost]
    public IActionResult AltaInquilino(Inquilino inquilino)
    {
        int r = repo.AltaInquilino(inquilino);
        return RedirectToAction(nameof(Index));

    }

    [HttpGet]
    public IActionResult Editar(long Id)
    {
        var inquilino = repo.Obtener(Id);
        return View(inquilino);
    }

    [HttpPost]
    public IActionResult Guardar(Inquilino inquilino)
    {
        repo.EditarInquilino(inquilino);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
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

