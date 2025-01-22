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


    public InmuebleController(ILogger<InmuebleController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Index(int pagina = 1)
    {
        const int pageSize = 10;
        int totalInmuebles = repoInmueble.ObtenerTotalInmuebles();
        int totalPages = (int)Math.Ceiling((double)totalInmuebles / pageSize);

        // Asegurarse de que la página no sea mayor que el número total de páginas
        pagina = Math.Max(1, Math.Min(pagina, totalPages));

        var inmuebles = repoInmueble.ObtenerPaginado(pagina, pageSize);

        var model = new InmuebleViewModel
        {
            Inmuebles = inmuebles,
            CurrentPage = pagina,
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
        var lista = repoInmueble.ObtenerInmueble();
        var tipos = repoTipo.ObtenerTipos();
        ViewBag.Tipos = tipos;
        ViewBag.Inmuebles = lista;
        
        return View();
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
        var result = repoInmueble.DesactivarInmueble(Id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Activar(int Id)
    {
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
            return View("Editar",inmueble);
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
    public IActionResult FiltrarTipo(int tipoInmueble)
    {
        if (tipoInmueble == 0) return RedirectToAction("Index");

        var inmueble = repoInmueble.FiltrarTipo(tipoInmueble);

        if (inmueble == null || !inmueble.Any())
        {
            TempData["Error"] = $"No se encontraron propiedades.";
            return RedirectToAction("Index");
        }
        var tipos = repoTipo.ObtenerTipos();
        ViewBag.Tipos = tipos;
        ViewBag.Inmuebles = inmueble; // Usamos ViewBag para pasar los inmuebles
        return View("Index"); // Volver a la vista principal
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult FiltrarPrecio(decimal? precioMin, decimal? precioMax)
    {
        if (!precioMin.HasValue) precioMin = 0;
        if (!precioMax.HasValue) precioMax = decimal.MaxValue;

        var inmueble = repoInmueble.FiltrarPrecio(precioMin.Value, precioMax.Value);

        if (inmueble == null || !inmueble.Any())
        {
            TempData["Error"] = $"No se encontraron inmuebles en el rango de precios especificado.";
            return RedirectToAction("Index");
        }
        var tipos = repoTipo.ObtenerTipos();
        ViewBag.Tipos = tipos;
        ViewBag.Inmuebles = inmueble; // Usamos ViewBag para pasar los inmuebles
        return View("Index"); // Volver a la vista principal
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult FiltrarUso(string usoInmueble)
    {
        if (string.IsNullOrEmpty(usoInmueble)) return RedirectToAction("Index");

        var inmueble = repoInmueble.FiltrarUso(usoInmueble);

        if (inmueble == null || !inmueble.Any())
        {
            TempData["Error"] = $"No se encontraron inmuebles con el uso especificado.";
            return RedirectToAction("Index");
        }
        
        var tipos = repoTipo.ObtenerTipos();
        ViewBag.Tipos = tipos;

        ViewBag.Inmuebles = inmueble; // Usamos ViewBag para pasar los inmuebles
        return View("Index"); // Volver a la vista principal
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult FiltrarAmbientes(int? ambientes)
    {
        if (!ambientes.HasValue)
        {
            ambientes = 0;
        }

        var inmuebles = repoInmueble.FiltrarAmbientes(ambientes.Value);

        if (inmuebles == null || !inmuebles.Any())
        {
            TempData["Error"] = $"No se encontraron inmuebles con el número de ambientes especificado.";
            return RedirectToAction("Index");
        }
        var tipos = repoTipo.ObtenerTipos();
        ViewBag.Tipos = tipos;
        ViewBag.Inmuebles = inmuebles; // Usamos ViewBag para pasar los inmuebles
        return View("Index"); // Volver a la vista principal
    }
}
