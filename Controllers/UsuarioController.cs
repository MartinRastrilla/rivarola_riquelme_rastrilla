using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
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

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Crear(Usuarios usuario)
    {
        repo.Crear(usuario);
        return View();
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

            return RedirectToAction("Index", "Inquilino");
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

