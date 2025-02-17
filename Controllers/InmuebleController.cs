using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rivarola_riquelme_rastrilla.Models;

namespace rivarola_riquelme_rastrilla.Controllers;
public class InmuebleController : Controller
{
    private readonly ILogger<InmuebleController> _logger;

    private RepositorioPropietario repoPropietario = new RepositorioPropietario();
    private RepositorioInmueble repoInmueble = new RepositorioInmueble();
    private RepositorioTipo repoTipo = new RepositorioTipo();
    private RepositorioContrato repoContrato = new RepositorioContrato();


    public InmuebleController(ILogger<InmuebleController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Index(int? tipo, string uso, decimal? precioMin, decimal? precioMax, int? ambientes, int page = 1, int pageSize = 10, string search = "")
    {
        ViewBag.Search = search; 

        int totalInmuebles = repoInmueble.ObtenerTotalInmuebles(tipo, uso, precioMin, precioMax, ambientes,search);
        int totalPages = (int)Math.Ceiling((double)totalInmuebles / pageSize);

        // Asegurarse de que la página no sea mayor que el número total de páginas
        page = Math.Max(1, Math.Min(page, totalPages));

        var inmuebles = repoInmueble.ObtenerInmueblesFiltrados(tipo, uso, precioMin, precioMax, ambientes, page, pageSize,search);
        if (inmuebles == null || inmuebles.Count == 0)
        {
            TempData["Error"] = $"No se encontraron inmuebles con los criterios especificados.";
            return RedirectToAction("Index");
        }
        var lista = repoInmueble.ObtenerInmueble();
        var tipos = repoTipo.ObtenerTipos();
        ViewBag.Tipos = tipos;
        ViewBag.Inmuebles = lista;
        var model = new InmuebleViewModel
        {
            Inmuebles = inmuebles,
            CurrentPage = page,
            TotalPages = totalPages
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
    public IActionResult AltaInmueble()
    {
        ViewBag.propietarios = repoPropietario.ObtenerTodos();
        ViewBag.tipos = repoTipo.ObtenerTipos();
        return View();
    }

    [HttpPost]
    [Authorize(Policy = "Empleado")]
    public IActionResult AltaInmueble(Inmueble Inmueble, int propietario_dni)
    {
        Propietarios? propietario = repoPropietario.ObtenerPorDni(propietario_dni);
        if (propietario == null)
        {
            ViewBag.Error = "No se encontró el propietario con el DNI ingresado.";
            ViewBag.propietarios = repoPropietario.ObtenerTodos();
            ViewBag.tipos = repoTipo.ObtenerTipos();
            return View(Inmueble);
        }
        bool estado = Request.Form["Estado"] == "true";
        Inmueble.Estado = estado;
        int r = repoInmueble.AltaInmueble(Inmueble);
        TempData["ToastMessage"] = "Inmueble agregado con exito";
        TempData["ToastType"] = "success";
        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    [Authorize(Policy = "Administrador")]
    public IActionResult Delete(int Id)
    {
        var inmueble = repoInmueble.Obtener(Id);
        if (inmueble == null)
        {
            return NotFound();
        }
        return View(inmueble);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Policy = "Administrador")]
    public IActionResult DeleteConfirmed(int Id)
    {
        repoInmueble.BajaInmueble(Id);
        TempData["ToastMessage"] = "Inmueble eliminado con exito";
        TempData["ToastType"] = "danger";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = "Administrador")]
    public IActionResult Baja(int Id)
    {
        TempData["ToastMessage"] = "Inmueble dado de baja con éxito";
        TempData["ToastType"] = "danger";
        var result = repoInmueble.DesactivarInmueble(Id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Activar(int Id)
    {
        TempData["ToastMessage"] = "Inmueble activado con éxito";
        TempData["ToastType"] = "success";
        var result = repoInmueble.ActivarInmueble(Id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Editar(int Id)
    {
        var inmueble = repoInmueble.Obtener(Id);
        ViewBag.propietarios = repoPropietario.ObtenerTodos();
        ViewBag.tipos = repoTipo.ObtenerTipos();
        ViewBag.inmueble = inmueble;
        return View(inmueble);
    }


    [HttpPost]
    [Authorize(Policy = "Empleado")]
    public IActionResult Guardar(Inmueble inmueble, int propietario_dni)
    {
        Propietarios? propietario = repoPropietario.ObtenerPorDni(propietario_dni);
        if (propietario == null)
        {
            ViewBag.Error = "No se encontró el propietario con el DNI ingresado.";
            ViewBag.propietarios = repoPropietario.ObtenerTodos();
            ViewBag.tipos = repoTipo.ObtenerTipos();
            ViewBag.inmueble = inmueble;
            return View("Editar", inmueble);
        }
        repoInmueble.GuardarInmueble(inmueble);
        TempData["ToastMessage"] = "Inmueble editado con exito";
        TempData["ToastType"] = "success";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Details(int Id)
    {
        var inmueble = repoInmueble.Obtener(Id);
        if (inmueble == null)
        {
            return NotFound();
        }
        return View(inmueble);
    }
    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Contratos(int inmuebleId, int pagina = 1)
    {
        var repoContrato = new RepositorioContrato();
        const int pageSize = 10;
        int totalContratos = repoContrato.ObtenerTotalContratosPorInmueble(inmuebleId);
        int totalPages = (int)Math.Ceiling((double)totalContratos / pageSize);

        // Asegurarse de que la página no sea mayor que el número total de páginas
        pagina = Math.Max(1, Math.Min(pagina, totalPages));

        var contratos = repoContrato.ObtenerContratosPorInmueble(inmuebleId, pagina, pageSize);

        var model = new ContratoViewModel
        {
            Contratos = contratos,
            CurrentPage = pagina,
            TotalPages = totalPages,
            InmuebleId = inmuebleId
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

    public IActionResult ObtenerInmueble(int Id)
    {
        var inmueble = repoInmueble.Obtener(Id);
        return Json(inmueble);
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult ObtenerInmueblesDisponiblesFechas(DateTime fechaInicio, DateTime fechaFin)
    {
        var inmueblesDisponibles = repoInmueble.ObtenerInmueblesPorFecha(fechaInicio, fechaFin);
        if (inmueblesDisponibles == null || inmueblesDisponibles.Count == 0)
        {
            return Json(repoInmueble.ObtenerInmueble());
        }

        return Json(inmueblesDisponibles);
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult VerificarContratos(int inmuebleId)
    {
        int contratos = repoContrato.ObtenerContratosActivosPorInmueble(inmuebleId);

        if (contratos > 0)
        {
            return Json(true);
        }
        return Json(false);
    }
}
