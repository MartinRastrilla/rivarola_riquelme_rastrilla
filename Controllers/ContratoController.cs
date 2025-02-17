using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rivarola_riquelme_rastrilla.Models;

namespace rivarola_riquelme_rastrilla.Controllers;

public class ContratoController : Controller
{
    private readonly ILogger<ContratoController> _logger;
    private RepositorioContrato repo = new RepositorioContrato();
    private RepositorioInquilino repoInquilino = new RepositorioInquilino();
    private RepositorioInmueble repoInmueble = new RepositorioInmueble();
    private RepositorioPago repoPago = new RepositorioPago();
    private RepositorioMulta repoMulta = new RepositorioMulta();
    private RepositorioRegistroContratos repoRegistroContratos = new RepositorioRegistroContratos();

    public ContratoController(ILogger<ContratoController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Index(int pagina = 1,string search = "")
    {
        ViewBag.Search = search; 
        const int pageSize = 10;
        int totalContratos = repo.ObtenerTotalContratos(search);
        int totalPages = (int)Math.Ceiling((double)totalContratos / pageSize);

        // Asegurarse de que la página no sea mayor que el número total de páginas
        pagina = Math.Max(1, Math.Min(pagina, totalPages));

        var contratos = repo.ObtenerPaginado(pagina, pageSize,search);
        var multas = repoMulta.ObtenerMultas();

        var model = new ContratoViewModel
        {
            Contratos = contratos,
            CurrentPage = pagina,
            TotalPages = totalPages,
            Multas = multas
        };

        if (User?.Identity?.IsAuthenticated == true)
        {
            return View(model);
        }
        else
        {
            return RedirectToAction("Login", "Home");
        }
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Editar(long Id)
    {
        var contrato = repo.Obtener(Id);
        ViewBag.inquilinos = repoInquilino.ObtenerInquilinos();
        ViewBag.inmuebles = repoInmueble.ObtenerInmueble();
        return View(contrato);
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Details(long Id)
    {
        var contrato = repo.Obtener(Id);
        if (contrato == null)
        {
            return NotFound();
        }
        return View(contrato);
    }


    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult AltaContrato()
    {
        ViewBag.inquilinos = repoInquilino.ObtenerInquilinos();
        ViewBag.inmuebles = repoInmueble.ObtenerInmueble();
        return View();
    }

    [HttpPost]
    [Authorize(Policy = "Empleado")]
    public IActionResult AltaContrato(Contratos contrato)
    {
        int contratoId = repo.AltaContrato(contrato);

        //Agregamos el registro del nuevo contrato
        RegistroContratos registroContratos = new RegistroContratos();
        registroContratos.Contrato_id = contratoId;
        registroContratos.Fecha_creacion = DateTime.Now;
        registroContratos.Creado_por = Convert.ToInt32(User.FindFirstValue("Id"));
        repoRegistroContratos.CrearRegistroContratos(registroContratos);

        TempData["ToastMessage"] = "Contrato creado con exito";
        TempData["ToastType"] = "success";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = "Administrador")]
    public IActionResult Delete(long Id)
    {
        var contrato = repo.Obtener(Id);
        if (contrato == null)
        {
            return NotFound();
        }
        return View(contrato);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Policy = "Administrador")]
    public IActionResult DeleteConfirmed(long Id)
    {
        var result = repo.BorrarContrato(Id);
        if (result > 0)
        {
            TempData["ToastMessage"] = "Contrato eliminado con exito";
            TempData["ToastType"] = "danger";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            ModelState.AddModelError("", "No se pudo eliminar el Contrato.");
            return View();
        }
    }

    [HttpPost]
    [Authorize(Policy = "Empleado")]
    public IActionResult Guardar(Contratos contrato)
    {

        repo.Guardar(contrato);
        TempData["ToastMessage"] = "Contrato editado con exito";
        TempData["ToastType"] = "success";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult FiltrarFecha(DateTime? fechaInicio, DateTime? fechaFin, int pagina = 1)
    {

        if (!fechaInicio.HasValue && !fechaFin.HasValue)
        {
            TempData["Error"] = "Debe seleccionar al menos una fecha de inicio o fecha de finalización.";
            return RedirectToAction("index");

        }

        const int pageSize = 10;
        var contratos = repo.ObtenerContratosPorFecha(fechaInicio, fechaFin);
        int totalContratos = contratos.Count();
        int totalPages = (int)Math.Ceiling((double)totalContratos / pageSize);

        // Asegurarse de que la página no sea mayor que el número total de páginas
        pagina = Math.Max(1, Math.Min(pagina, totalPages));

        ViewData["fechaInicio"] = fechaInicio?.ToString("dd-MM-yyyy");
        ViewData["fechaFin"] = fechaFin?.ToString("dd-MM-yyyy");

        var model = new ContratoViewModel
        {
            Contratos = contratos,
            CurrentPage = pagina,
            TotalPages = totalPages
        };

        if (User?.Identity?.IsAuthenticated == true)
        {
            return View("Index", model);
        }
        else
        {
            return RedirectToAction("Login", "Home");
        }

    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Pagos(int contratoId, int pagina = 1)
    {
        const int pageSize = 10;
        var totalPagos = repoPago.ObtenerTotalPagosPorContrato(contratoId);
        var totalPages = (int)Math.Ceiling((double)totalPagos / pageSize);

        // Asegurarse de que la página no sea mayor que el número total de páginas
        pagina = Math.Max(1, Math.Min(pagina, totalPages));

        var pagos = repoPago.ObtenerPagosPorContrato(contratoId, pagina, pageSize);
        var contrato = repo.Obtener(contratoId);
        var multa = repoMulta.ObtenerMultaPorContrato(contratoId);
        var model = new PagoViewModel
        {
            Pagos = pagos,
            CurrentPage = pagina,
            TotalPages = totalPages,
            ContratoId = contratoId,
            Contrato = contrato,
            Multa = multa
        };

        if (User?.Identity?.IsAuthenticated == true)
        {
            return View(model);
        }
        else
        {
            return RedirectToAction("Login", "Home");
        }
    }

    public IActionResult ObtenerContrato(long id)
    {
        var contrato = repo.Obtener(id);
        return Json(contrato);
    }
}