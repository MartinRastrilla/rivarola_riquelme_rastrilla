using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Iana;
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

    [Authorize(Policy = "Empleado")]
    public IActionResult Index()
    {
        var lista = repo.ObtenerTodos();
        return View(lista);
    }

    [Authorize(Policy = "Empleado")]
    public IActionResult Details(int id)
    {
        var pago = repo.ObtenerPorId(id);
        if (pago == null)
        {
            return NotFound();
        }
        return View(pago);
    }

    [Authorize(Policy = "Administrador")]
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
    [Authorize(Policy = "Administrador")]
    public IActionResult DeleteConfirmed(int id)
    {
        repo.Eliminar(id);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = "Empleado")]
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
    [Authorize(Policy = "Empleado")]
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
    [Authorize(Policy = "Empleado")]
    public IActionResult Crear(Pago pago)
    {
        if (ModelState.IsValid)
        {
            repo.Agregar(pago);
            return RedirectToAction(nameof(Index));
        }
        return View(pago);
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult NuevoPago(int id)
    {
        var pago = repo.ObtenerPorId(id);
        if (pago == null)
        {
            return NotFound("pago No encontrado");
        }

        var NuevoPago = new Pago();
        NuevoPago.Contrato_id = pago.Contrato_id;
        return View("NuevoPago", NuevoPago);
    }

    [HttpPost]
    [Authorize(Policy = "Empleado")]
    public IActionResult NuevoPago(Pago pago)
    {
        // Console.WriteLine($"Contrato_id: {pago.Contrato_id}, Fecha_pago: {pago.Fecha_pago}, Detalle: {pago.Detalle}, Importe: {pago.Importe}");
        if (!ModelState.IsValid || pago == null)
        {
            return View(pago);
        }
        repo.NuevoPago(pago);
        return RedirectToAction(nameof(Index));
    }
}
