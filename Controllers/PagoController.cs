using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using rivarola_riquelme_rastrilla.Models;

namespace rivarola_riquelme_rastrilla.Controllers;

public class PagoController : Controller
{
    private readonly ILogger<PagoController> _logger;
    private RepositorioPago repo;

    private RepositorioContrato repositorioContrato = new RepositorioContrato();

    public PagoController(ILogger<PagoController> logger)
    {
        _logger = logger;
        repo = new RepositorioPago();
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


    public IActionResult Edit(int id)
    {
        var pago = repo.ObtenerPorId(id); // Obtener el Pago a editar
        if (pago == null)
        {
            return NotFound();
        }

        ViewBag.Contratos = repositorioContrato.ObtenerContratos();

        return View(pago);
    }

    [HttpPost]
    public IActionResult Edit(Pago pago)
    {
        if (ModelState.IsValid)
        {
            repo.Editar(pago);
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Contratos = repositorioContrato.ObtenerContratos();
        return View(pago);
    }

    public IActionResult Crear()
    {        
        ViewBag.Contratos = repositorioContrato.ObtenerContratos();
        return View();
    }

    [HttpPost]
    public IActionResult Crear(Pago pago)
    {
        if (ModelState.IsValid)
        {
            repo.Agregar(pago);
            return RedirectToAction(nameof(Index));
        }
        return View(pago);
    }

}