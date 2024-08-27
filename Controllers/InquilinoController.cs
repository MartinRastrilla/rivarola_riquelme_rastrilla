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
        if (Id == 0)
        {
            return View();
        }
        else
        {
            var inquilino = repo.Obtener(Id);
            return View(inquilino);
        }
    }

    [HttpPost]
    public IActionResult Guardar(Inquilino inquilino)
    {
        repo.EditarInquilino(inquilino);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Baja(long Dni)
    {
        if (Dni == 0)
        {
            return View();
        }
        else
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
    }
}
