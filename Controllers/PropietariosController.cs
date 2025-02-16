using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rivarola_riquelme_rastrilla.Models;

namespace rivarola_riquelme_rastrilla.Controllers;

public class PropietariosController : Controller
{
    private readonly ILogger<PropietariosController> _logger;
    private RepositorioPropietario repo;
    private RepositorioInmueble repoInmueble = new RepositorioInmueble();

    public PropietariosController(ILogger<PropietariosController> logger)
    {
        _logger = logger;
        repo = new RepositorioPropietario();
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Index(int pagina = 1)
    {
        const int pageSize = 10;
        int totalPropietarios = repo.ObtenerTotalPropietarios();
        int totalPages = (int)Math.Ceiling((double)totalPropietarios / pageSize);

        // Asegurarse de que la página no sea mayor que el número total de páginas
        pagina = Math.Max(1, Math.Min(pagina, totalPages));

        var propietarios = repo.ObtenerPaginado(pagina, pageSize);

        var model = new PropietarioViewModel
        {
            Propietarios = propietarios,
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
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Inmuebles(int dni, int pagina = 1)
    {
        const int pageSize = 10;
        int totalInmuebles = repoInmueble.ObtenerCantInmueblesPorPropietario(dni);
        int totalPages = (int)Math.Ceiling((double)totalInmuebles / pageSize);

        // Asegurarse de que la página no sea mayor que el número total de páginas
        pagina = Math.Max(1, Math.Min(pagina, totalPages));

        var inmuebles = repoInmueble.ObtenerInmueblesPorPropietario(dni, pagina, pageSize);

        var model = new InmuebleViewModel
        {
            Inmuebles = inmuebles,
            CurrentPage = pagina,
            TotalPages = totalPages,
            Propietario_dni = dni
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
    public IActionResult Details(int id)
    {
        var propietario = repo.ObtenerPorId(id);
        if (propietario == null)
        {
            return NotFound();
        }
        return View(propietario);
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Edit(int id)
    {
        var propietario = repo.ObtenerPorId(id);
        if (propietario == null)
        {
            return NotFound();
        }
        return View(propietario);
    }

    [HttpPost]
    [Authorize(Policy = "Empleado")]
    public IActionResult Edit(Propietarios propietario)
    {

            Propietarios? propietarioExistente = repo.ObtenerPorDni(propietario.Dni);
            if (propietarioExistente != null && propietarioExistente.Id != propietario.Id)
            {
                ViewBag.Error = "Ya existe un propietario con el Dni ingresado.";
                return View(propietario);
            }
            repo.Editar(propietario);
            TempData["ToastMessage"] = "Propietario editado con éxito.";
            TempData["ToastType"] = "success";
            return RedirectToAction(nameof(Index));

    }

    [HttpGet]
    [Authorize(Policy = "Administrador")]
    public IActionResult Delete(int id)
    {
        var propietario = repo.ObtenerPorId(id);
        if (propietario == null)
        {
            return NotFound();
        }
        return View(propietario);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Policy = "Administrador")]
    public IActionResult DeleteConfirmed(int id)
    {
        repo.Eliminar(id);
        TempData["ToastMessage"] = "Propietario eliminado con éxito.";
        TempData["ToastType"] = "danger";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Crear()
    {
        return View();
    }


    [HttpPost]
    [Authorize(Policy = "Empleado")]
    public IActionResult Crear(Propietarios propietario)
    {

        Propietarios? propietarioExistente = repo.ObtenerPorDni(propietario.Dni);
        if (propietarioExistente != null)
        {
            ViewBag.Error = "Ya existe un propietario con el DNI ingresado.";
            return View(propietario);
        }

        repo.Crear(propietario);
        TempData["ToastMessage"] = "Propietario creado con éxito.";
        TempData["ToastType"] = "success";
        return RedirectToAction(nameof(Index));
    }
}