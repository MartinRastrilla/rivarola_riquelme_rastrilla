
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rivarola_riquelme_rastrilla.Models;

namespace rivarola_riquelme_rastrilla.Controllers;

public class AuditoriaController : Controller
{
    private RepositorioRegistroContratos repositorioRegistroContratos = new RepositorioRegistroContratos();
    private RepositorioRegistroPagos repositorioRegistroPagos = new RepositorioRegistroPagos();
    private readonly ILogger<AuditoriaController> _logger;
    public AuditoriaController(ILogger<AuditoriaController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = "Administrador")]
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Contratos(int pagina = 1)
    {
        const int pageSize = 10;
        int totalRegistros = repositorioRegistroContratos.ObtenerCantRegistrosContratos();
        int totalPages = (int)Math.Ceiling((double)totalRegistros / pageSize);

        // Asegurarse de que la página no sea mayor que el número total de páginas
        pagina = Math.Max(1, Math.Min(pagina, totalPages));

        var registros = repositorioRegistroContratos.ObtenerRegistrosContratosPaginado(pagina, pageSize);


        var model = new RegistroContratosViewModel
        {
            RegistrosContratos = registros,
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

    public IActionResult Pagos(int pagina = 1)
    {
        const int pageSize = 10;
        int totalRegistros = repositorioRegistroPagos.ObtenerTotalRegistrosPago();
        int totalPages = (int)Math.Ceiling((double)totalRegistros / pageSize);

        // Asegurarse de que la página no sea mayor que el número total de páginas        
        pagina = Math.Max(1, Math.Min(pagina, totalPages));

        var registros = repositorioRegistroPagos.ObtenerRegistroPagosPaginado(pagina, pageSize);

        var model = new RegistroPagosViewModel
        {
            RegistrosPagos = registros,
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
}