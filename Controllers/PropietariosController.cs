using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rivarola_riquelme_rastrilla.Models;

namespace rivarola_riquelme_rastrilla.Controllers;

public class PropietariosController : Controller
{
    private readonly ILogger<PropietariosController> _logger;
    private RepositorioPropietario repo;

    public PropietariosController(ILogger<PropietariosController> logger)
    {
        _logger = logger;
        repo = new RepositorioPropietario();
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Index()
    {
        var lista = repo.ObtenerTodos();
        return View(lista);
    }

    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult Inmuebles(int dni)
    {
        var repoInmueble = new RepositorioInmueble();
        var inmuebles = repoInmueble.ObtenerInmueblesPorPropietario(dni);
        return View(inmuebles);
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
    public IActionResult Edit(int id, Propietarios propietario)
    {
        if (id != propietario.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            repo.Editar(propietario);
            return RedirectToAction(nameof(Index));
        }
        return View(propietario);
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
        if (ModelState.IsValid)
        {
            repo.Crear(propietario);
            return RedirectToAction(nameof(Index));
        }
        return View(propietario);
    }
}