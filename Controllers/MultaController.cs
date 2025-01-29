using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rivarola_riquelme_rastrilla.Models;

namespace rivarola_riquelme_rastrilla.Controllers;

public class MultaController : Controller
{
    private readonly ILogger<MultaController> _logger;
    private RepositorioMulta repo = new RepositorioMulta();
    private RepositorioContrato repoContrato = new RepositorioContrato();
    private RepositorioInquilino repoInquilino = new RepositorioInquilino();
    private RepositorioInmueble repoInmueble = new RepositorioInmueble();
    private RepositorioPago repoPago = new RepositorioPago();

    public MultaController(ILogger<MultaController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult AltaMulta(int id)
    {
        var contrato = repoContrato.Obtener(id);
        ViewBag.inquilinos = repoInquilino.ObtenerInquilinos();
        ViewBag.inmuebles = repoInmueble.ObtenerInmueble();
        return View("AltaMulta",contrato);
    }

    [HttpPost]
    [Authorize(Policy = "Empleado")]
    public IActionResult Guardar(Multa multa)
    {
        repo.AgregarMulta(multa);
        return RedirectToAction("index","Contrato");
    }
}