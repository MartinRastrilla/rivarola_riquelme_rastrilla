using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rivarola_riquelme_rastrilla.Models;

namespace rivarola_riquelme_rastrilla.Controllers
{
    [Authorize(Policy = "Empleado")]
    public class TipoController : Controller
    {
        private RepositorioTipo repoTipo = new RepositorioTipo();

        [HttpGet]
        public IActionResult Index()
        {
            var lista = repoTipo.ObtenerTipos();
            return View(lista);
        }

        [HttpGet]
        public IActionResult AltaTipo()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AltaTipo(Tipo tipo)
        {
            if (ModelState.IsValid)
            {
                repoTipo.AgregarTipo(tipo);
                return RedirectToAction(nameof(Index));
            }
            return View(tipo);
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var tipo = repoTipo.ObtenerTipo(id);
            if (tipo == null)
            {
                return NotFound();
            }
            return View(tipo);
        }

        [HttpPost]
        public IActionResult Editar(Tipo tipo)
        {
            if (ModelState.IsValid)
            {
                repoTipo.EditarTipo(tipo);
                return RedirectToAction(nameof(Index));
            }
            return View(tipo);
        }

        [HttpGet]
        [Authorize(Policy = "Administrador")]
        public IActionResult Delete(int id)
        {
            var tipo = repoTipo.ObtenerTipo(id);
            if (tipo == null)
            {
                return NotFound();
            }
            return View(tipo);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Policy = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            repoTipo.BorrarTipo(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var tipo = repoTipo.ObtenerTipo(id);
            if (tipo == null)
            {
                return NotFound();
            }
            return View(tipo);
        }
    }
}
