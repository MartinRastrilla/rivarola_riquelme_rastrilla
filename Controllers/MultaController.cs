

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rivarola_riquelme_rastrilla.Models;

namespace rivarola_riquelme_rastrilla.Controllers;
public class MultaController : Controller
{

    private readonly ILogger<MultaController> _logger;
    private RepositorioMulta repositorioMulta = new RepositorioMulta();
    public MultaController(ILogger<MultaController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Policy = "Empleado")]
    public IActionResult AplicarMulta(Multa multa)
    {
        if (ModelState.IsValid)
        {
            TempData["ToastMessage"] = "Multa aplicada con exito";
            TempData["ToastType"] = "success";
            repositorioMulta.AltaMulta(multa);
            return RedirectToAction("Index", "Contrato");
        }
        return View(multa);
    }
}