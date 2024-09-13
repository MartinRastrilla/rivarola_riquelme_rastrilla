using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using rivarola_riquelme_rastrilla.Models;

namespace rivarola_riquelme_rastrilla.Controllers;

public class UsuariosController : Controller
{
    private readonly ILogger<UsuariosController> _logger;
    private RepositorioUsuarios repo = new RepositorioUsuarios();

    public UsuariosController(ILogger<UsuariosController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = "Administrador")]
    public IActionResult Index()
    {
        var lista = repo.ObtenerUsuarios();
        return View(lista);
    }

    [HttpGet]
    [Authorize(Policy = "Administrador")]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Policy = "Administrador")]
    public IActionResult Crear(Usuarios usuario)
    {
        repo.Crear(usuario);
        return View();
    }

    [HttpGet]
    [Authorize(Policy = "Administrador")]
    public IActionResult Editar(int Id)
    {
        var usuario = repo.ObtenerById(Id);
        ViewBag.Roles = new List<SelectListItem>
        {
            new SelectListItem { Text = "Administrador", Value = "Administrador" },
            new SelectListItem { Text = "Empleado", Value = "Empleado" }
        };
        return View(usuario);
    }

    [HttpPost]
    [Authorize(Policy = "Administrador")]
    public IActionResult Editar(Usuarios usuario)
    {
        repo.Editar(usuario);
        return RedirectToAction("Index", "Usuarios");
    }


    [HttpGet]
    [Authorize(Policy = "Administrador")]
    public IActionResult Delete(int Id)
    {
        var usuario = repo.ObtenerById(Id);
        return View(usuario);
    }
    [HttpPost]
    [Authorize(Policy = "Administrador")]
    public IActionResult Baja(int Id)
    {
        repo.Borrar(Id);
        return RedirectToAction("Index", "Usuarios");
    }

    [HttpGet]
    [Authorize(Policy = "Administrador")]
    public IActionResult Details(int Id)
    {
        var usuario = repo.ObtenerById(Id);
        return View(usuario);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string Email, string Contrasenia)
    {
        Usuarios? usuario = repo.ObtenerByEmail(Email);

        string storedPass = usuario.Contrasenia;
        string userRol = usuario.Rol;
        byte[] salt = new byte[10];
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: Contrasenia,  
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8
        ));
        if (storedPass == hashed)
        {
            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, Email),
                                new Claim(ClaimTypes.Role, userRol)
                            };
            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }
        else
        {
            ViewBag.Error = "Datos Ingresados Incorrectos";
            return View();
        }

    }
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Usuarios");
    }
}

