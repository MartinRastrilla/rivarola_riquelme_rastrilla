using System.Diagnostics;
using System.Security.Claims;
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
    private RepositorioRegistroPagos repositorioRegistroPagos = new RepositorioRegistroPagos();

    public PagoController(ILogger<PagoController> logger)
    {
        _logger = logger;
        repo = new RepositorioPago();
    }

    [Authorize(Policy = "Empleado")]
    public IActionResult Index(int pagina = 1)
    {
        const int pageSize = 10;
        int totalPagos = repo.ObtenerTotalPagos();
        int totalPages = (int)Math.Ceiling((double)totalPagos / pageSize);

        // Asegurarse de que la página no sea mayor que el número total de páginas
        pagina = Math.Max(1, Math.Min(pagina, totalPages));

        var pagos = repo.ObtenerPaginado(pagina, pageSize);

        var viewModel = new PagoViewModel
        {
            Pagos = pagos,
            CurrentPage = pagina,
            TotalPages = totalPages
        };

        if (User?.Identity?.IsAuthenticated == true)
        {
            return View(viewModel);
        }
        else
        {
            return RedirectToAction("Login", "Home");
        }

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
        TempData["ToastMessage"] = "Pago eliminado con éxito";
        TempData["ToastType"] = "danger";
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
            TempData["ToastMessage"] = "Pago editado con éxito";
            TempData["ToastType"] = "success";
            repo.Editar(pago);
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Contratos = repositorioContrato.ObtenerContratos();
        return View(pago);
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Crear()
    {
        ViewBag.Contratos = repositorioContrato.ObtenerContratos();
        return View();
    }

    [HttpPost]
    [Authorize(Policy = "Empleado")]
    public IActionResult Crear(Pago pago)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ToastMessage"] = "Error al crear el Pago: datos inválidos.";
                TempData["ToastType"] = "danger";
                ViewBag.Contratos = repositorioContrato.ObtenerContratos();
                return View(pago);
            }

            var newPago = pago;
            newPago.Fecha_pago = DateTime.Now;

            //Calcular el TOTAL PAGADO por el MONTO DEL CONTRATO para Finalizar el Contrato
            var pagosDelContrato = repo.ObtenerPagosActivosPorContrato(newPago.Contrato_id);
            decimal? totalPagado = pagosDelContrato.Sum(p => p.Importe);
            totalPagado += newPago.Importe;

            var contrato = repositorioContrato.Obtener((int)newPago.Contrato_id);

            if (totalPagado == contrato?.Monto)
            {
                repositorioContrato.FinalizarContrato((int)newPago.Contrato_id);
                TempData["ToastMessage"] = "¡Felicitaciones! El Contrato ha sido pagado en su totalidad.";
                TempData["ToastType"] = "success";
            }

            // Contar cantidad de pagos para contrato
            int numPago = repo.ObtenerTotalPagosPorContrato(newPago.Contrato_id);
            newPago.Num_pago = numPago + 1;

            var pagoId = repo.Agregar(newPago);

            TempData["ToastMessage"] += "||Pago creado con éxito";
            TempData["ToastType"] += "||success";

            RegistroPagos registroPago = new RegistroPagos();
            registroPago.Pago_id = pagoId;

            var userId = User.FindFirstValue("Id");
            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("El usuario no está autenticado o falta el ID.");
            }

            registroPago.Creado_por = Convert.ToInt32(userId);
            registroPago.Fecha_creacion = DateTime.Now;
            repositorioRegistroPagos.CrearRegistroPago(registroPago);

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en la función Crear: {ex.Message}");
            TempData["ToastMessage"] = $"Error al crear el pago: {ex.Message}";
            TempData["ToastType"] = "danger";
            ViewBag.Contratos = repositorioContrato.ObtenerContratos();
            return View(pago);
        }
    }


    [HttpPost]
    [Authorize(Policy = "Empleado")]
    public IActionResult AnularPago(int id)
    {
        if (id == 0)
        {
            TempData["ToastMessage"] = "Error al anular el Pago";
            TempData["ToastType"] = "danger";
            return RedirectToAction(nameof(Index));
        }
        RegistroPagos registroPago = repositorioRegistroPagos.ObtenerRegistroPorPago(id);
        registroPago.Anulado_por = Convert.ToInt32(User.FindFirstValue("Id"));
        registroPago.Fecha_anulacion = DateTime.Now;
        repositorioRegistroPagos.CrearAnulacionRegistroPago(registroPago);
        repo.DesactivarPago(id);
        TempData["ToastMessage"] = "Pago anulado con éxito";
        TempData["ToastType"] = "success";
        return RedirectToAction(nameof(Index));
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

        if (!ModelState.IsValid || pago == null)
        {
            return View(pago);
        }
        TempData["ToastMessage"] = "Pago creado con éxito";
        TempData["ToastType"] = "success";
        repo.Agregar(pago);
        return RedirectToAction(nameof(Index));
    }
}
