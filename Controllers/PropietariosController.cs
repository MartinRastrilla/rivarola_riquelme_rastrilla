using System.Diagnostics;
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

    public IActionResult Index()
    {
        var lista = repo.ObtenerTodos();
        return View(lista);
    }

    public IActionResult Details(int id)
    {
        var propietario = repo.ObtenerPorId(id);
        if (propietario == null)
        {
            return NotFound();
        }
        return View(propietario);
    }

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