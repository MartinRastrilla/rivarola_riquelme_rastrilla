

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rivarola_riquelme_rastrilla.Models;

namespace rivarola_riquelme_rastrilla.Controllers;
public class MultaController : Controller
{

    private readonly ILogger<MultaController> _logger;
    private RepositorioMulta repositorioMulta = new RepositorioMulta();
    private RepositorioContrato repositorioContrato = new RepositorioContrato();
    private RepositorioRegistroContratos repositorioRegistroContratos = new RepositorioRegistroContratos();
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
            RegistroContratos registroContrato = repositorioRegistroContratos.ObtenerRegistroContratosPorContrato(multa.Contrato_id);
            if (registroContrato == null)
            {
                TempData["ToastMessage"] = "Error al aplicar la multa";
                TempData["ToastType"] = "danger";
                return RedirectToAction("Index", "Contrato");
            }
            repositorioMulta.AltaMulta(multa);
            repositorioContrato.CancelarContrato(multa.Contrato_id);
            repositorioRegistroContratos.CrearCancelacionRegistro(registroContrato);

            return RedirectToAction("Index", "Contrato");
        }
        return View(multa);
    }
}