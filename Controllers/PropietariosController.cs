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
        repo =new RepositorioPropietario();
    }

    public IActionResult Index()
    {
        var lista = repo.ObtenerTodos();
        return View(lista);
    }

}