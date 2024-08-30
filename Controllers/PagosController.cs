using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using rivarola_riquelme_rastrilla.Models;

namespace rivarola_riquelme_rastrilla.Controllers;

public class PagosController : Controller
{
    private readonly ILogger<PagosController> _logger;
    private RepositorioPagos repo;

    public PagosController(ILogger<PagosController> logger)
    {
        _logger = logger;
        repo = new RepositorioPagos();
    }

    public IActionResult Index()
    {
        var lista = repo.ObtenerTodos();
        return View(lista);
    }

    public IActionResult Details(int id)
    {
        var pago = repo.ObtenerPorId(id);
        if (pago == null)
        {
            return NotFound();
        }
        return View(pago);
    }

    public IActionResult Edit(int id)
    {
        var pago = repo.ObtenerPorId(id);
        if (pago == null)
        {
            return NotFound();
        }
        return View(pago);
    }

    [HttpPost]
    public IActionResult Edit(int id, Pagos pago)
    {
        if (id != pago.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            repo.Editar(pago);
            return RedirectToAction(nameof(Index));
        }
        return View(pago);
    }

    public IActionResult Delete(int id)
    {
        var pago = repo.ObtenerPorId(id);
        if (pago == null)
        {
            return NotFound();
        }
        return View(pago);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        repo.Eliminar(id);
        return RedirectToAction(nameof(Index));
    }
}