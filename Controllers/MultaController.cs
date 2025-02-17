

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rivarola_riquelme_rastrilla.Models;

namespace rivarola_riquelme_rastrilla.Controllers;
public class MultaController : Controller
{

    private readonly ILogger<MultaController> _logger;
    private RepositorioMulta repositorioMulta = new RepositorioMulta();
    private RepositorioContrato repositorioContrato = new RepositorioContrato();
    private RepositorioPago repositorioPago = new RepositorioPago();
    private RepositorioRegistroContratos repositorioRegistroContratos = new RepositorioRegistroContratos();
    public MultaController(ILogger<MultaController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Policy = "Empleado")]
    public IActionResult AplicarMulta(Multa multa)
    {
        int contrato_id = multa.Contrato_id;
        if (ModelState.IsValid)
        {
            if (contrato_id == 0)
            {
                TempData["ToastMessage"] = "Error al aplicar la multa";
                TempData["ToastType"] = "danger";
                return RedirectToAction("Index", "Contrato");
            }

            RegistroContratos registroContrato = repositorioRegistroContratos.ObtenerRegistroContratosPorContrato(contrato_id);
            if (registroContrato == null)
            {
                TempData["ToastMessage"] = "Error al aplicar la multa";
                TempData["ToastType"] = "danger";
                return RedirectToAction("Index", "Contrato");
            }
            registroContrato.Cancelado_por = Convert.ToInt32(User.FindFirstValue("Id"));
            registroContrato.Fecha_cancelacion = DateTime.Now;



            var contrato = repositorioContrato.Obtener(contrato_id);
            if (contrato == null)
            {
                TempData["ToastMessage"] = "Error al aplicar la multa. No se pudo obtener el contrato";
                TempData["ToastType"] = "danger";
                return RedirectToAction("Index", "Contrato");
            }
            else
            {
                //Recalcular el monto del contrato
                var pagosDelContrato = repositorioPago.ObtenerPagosActivosPorContrato(contrato_id);
                //Sumamos los importes de los pagos
                decimal? totalPagado = pagosDelContrato.Sum(p => p.Importe);

                contrato.Monto = (decimal)totalPagado + multa.Monto;
                repositorioContrato.Guardar(contrato);
                TempData["ToastMessage"] = "Monto del contrato recalculado con éxito";
                TempData["ToastType"] = "success";
            }


            repositorioMulta.AltaMulta(multa);
            repositorioContrato.CancelarContrato(multa.Contrato_id);
            repositorioRegistroContratos.CrearCancelacionRegistro(registroContrato);

            TempData["ToastMessage"] += "||Multa aplicada con éxito";
            TempData["ToastType"] += "||success";

            return RedirectToAction("Index", "Contrato");
        }
        return View(multa);
    }
}