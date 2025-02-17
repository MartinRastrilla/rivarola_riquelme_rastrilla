using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
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
    private readonly IWebHostEnvironment environment;

    public UsuariosController(IWebHostEnvironment environment)
    {
        this.environment = environment;
    }

    [HttpGet]
    [Authorize(Policy = "Administrador")]
    public IActionResult Index(int pagina = 1)
    {
        const int pageSize = 10;
        int totalUsuarios = repo.ObtenerTotalUsuarios();
        int totalPages = (int)Math.Ceiling((double)totalUsuarios / pageSize);

        // Asegurarse de que la página no sea mayor que el número total de páginas
        pagina = Math.Max(1, Math.Min(pagina, totalPages));

        var usuarios = repo.ObtenerPaginado(pagina, pageSize);
        var model = new UsuariosViewModel
        {
            Usuarios = usuarios,
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
    [Authorize(Policy = "Administrador")]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Policy = "Administrador")]
    public IActionResult Crear(Usuarios usuario)
    {
        TempData["ToastMessage"] = "Usuario creado con exito";
        TempData["ToastType"] = "success";
        repo.Crear(usuario);
        return RedirectToAction("Index", "Usuarios");
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
        TempData["ToastMessage"] = "Usuario editado con éxito";
        TempData["ToastType"] = "success";
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
        TempData["ToastMessage"] = "Usuario eliminado con éxito";
        TempData["ToastType"] = "danger";
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
    [Authorize(Policy = "Empleado")]
    public IActionResult Perfil(int Id)
    {
        var usuario = repo.ObtenerById(Id);
        return View(usuario);
    }


    [HttpPost]
    [Authorize(Policy = "Empleado")]
    public async Task<IActionResult> ActualizarPerfil(IFormFile avatar, string Nombre, string Apellido)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

        if (userId == null)
        {
            return Unauthorized();
        }

        var user = repo.ObtenerById(int.Parse(userId));

        if (user == null)
        {
            return NotFound();
        }

        //Actualizar datos básicos
        user.Nombre = Nombre;
        user.Apellido = Apellido;

        if (avatar != null && avatar.Length > 0)
        {
            string wwwPath = environment.WebRootPath;
            string path = Path.Combine(wwwPath, "Uploads");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (!string.IsNullOrEmpty(user.Avatar) && user.Avatar != "/Uploads/user_pic.jpg")
            {
                string ruteAnterior = Path.Combine(wwwPath, user.Avatar);
                if (System.IO.File.Exists(ruteAnterior))
                {
                    System.IO.File.Delete(ruteAnterior);
                }
            }

            string fileName = "avatar_" + user.Id + Path.GetExtension(avatar.FileName);
            string pathCompleto = Path.Combine(path, fileName);

            user.Avatar = Path.Combine("/Uploads", fileName);

            using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
            {
                await avatar.CopyToAsync(stream);
            }
            TempData["ToastMessage"] = "Perfil actualizado con éxito";
            TempData["ToastType"] = "success";
            repo.EditarAvatar(user);
        }
        else
        {
            TempData["ToastMessage"] = "Perfil actualizados con éxito";
            TempData["ToastType"] = "success";
            //Si no hay archivo, cambiar datos básicos
            repo.Editar(user);
        }

        //Actualizar cookie
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Nombre),
        new Claim("Id", user.Id.ToString()),
        new Claim("Avatar", user.Avatar),
        new Claim("Nombre", user.Nombre),
        new Claim("Apellido", user.Apellido),
        new Claim(ClaimTypes.Role, user.Rol)
    };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        // Crear un nuevo principal de usuario
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true
        };

        // Actualizar la cookie de autenticación
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Authorize(Policy = "Empleado")]
    public async Task<IActionResult> DeleteAvatar()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var user = repo.ObtenerById(int.Parse(userId));
        user.Avatar = "/Uploads/user_pic.jpg";

        TempData["ToastMessage"] = "Avatar eliminado con éxito";
        TempData["ToastType"] = "success";
        repo.EditarAvatar(user);

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Nombre),
        new Claim("Id", user.Id.ToString()),
        new Claim("Avatar", user.Avatar ?? "/Uploads/user_pic.jpg"),  // Ruta al avatar
        new Claim("Nombre", user.Nombre),
        new Claim("Apellido", user.Apellido),
        new Claim(ClaimTypes.Role, user.Rol)
    };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        // Crear un nuevo principal de usuario
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true
        };

        // Actualizar la cookie de autenticación
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

        return RedirectToAction("Index", "Home");
    }


    [HttpGet]
    [Authorize(Policy = "Empleado")]
    public IActionResult CambiarPass()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Policy = "Empleado")]
    public async Task<IActionResult> CambiarPass(string ContraseniaActual, string ContraseniaNueva)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var user = repo.ObtenerById(int.Parse(userId));
        string oldHashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: ContraseniaActual,
            salt: new byte[10],
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8
        ));

        if (oldHashed != user.Contrasenia)
        {
            ModelState.AddModelError("ContraseniaActual", "La contraseña actual es incorrecta.");
            return View();
        }

        user.Contrasenia = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: ContraseniaNueva,
            salt: new byte[10],
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8
        ));

        TempData["ToastMessage"] = "Contraseña modificada con éxito";
        TempData["ToastType"] = "success";
        repo.EditarContrasenia(user);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Usuarios");
    }
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpGet]
    public IActionResult MailSent()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string Email, string Contrasenia)
    {
        Usuarios? usuario = repo.ObtenerByEmail(Email);

        if (usuario == null)
        {
            ViewBag.Error = "Usuario no encontrado";
            return View("Login");
        }

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
                                new Claim(ClaimTypes.Role, userRol),
                                new Claim("Id", usuario.Id+""),
                                new Claim("Avatar", usuario.Avatar),
                                new Claim("Nombre", usuario.Nombre),
                                new Claim("Apellido", usuario.Apellido)
                            };
            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            TempData["ToastMessage"] = "Bienvenido/a " + usuario.Nombre + " " + usuario.Apellido;
            TempData["ToastType"] = "success";
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
        TempData["ToastMessage"] = "Sesión cerrada con éxito";
        TempData["ToastType"] = "success";
        return RedirectToAction("Login", "Usuarios");
    }

    [HttpPost]
    public async Task<IActionResult> EnviarCorreoRestaurarContrasenia(string Email)
    {
        var usuario = repo.ObtenerByEmail(Email);
        if (usuario == null)
        {
            ViewBag.Error = "Correo no registrado";
            return RedirectToAction("Login", "Usuarios");
        }
        int userId = usuario.Id;
        string token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        DateTime expirationDate = DateTime.UtcNow.AddHours(1);

        repo.CrearToken(usuario, token, expirationDate);

        var link = Url.Action("RestaurarContrasenia", "Usuarios", new { token = token }, Request.Scheme);

        await EnviarCorreo(Email, "Restaurar Contraseña", $"Para restaurar tu contrasenia, haz click en el siguiente link: <a href='{link}'>Restablecer Contraseña</a>");

        TempData["ToastMessage"] = "Correo enviado con éxito";
        TempData["ToastType"] = "success";
        return RedirectToAction("MailSent", "Usuarios");
    }

    private async Task EnviarCorreo(string destinatario, string asunto, string cuerpo)
    {
        string smtpHost = "fennazmarketing@gmail.com"; // Asegurar que el email esté completo
        string? smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD");

        if (string.IsNullOrEmpty(smtpPassword))
        {
            throw new Exception("SMTP_PASSWORD no está configurado correctamente.");
        }

        using (var smtp = new SmtpClient("smtp.gmail.com", 587))
        {
            smtp.Credentials = new NetworkCredential(smtpHost, smtpPassword);
            smtp.EnableSsl = true;

            var mail = new MailMessage
            {
                From = new MailAddress(smtpHost),
                Subject = asunto,
                Body = cuerpo,
                IsBodyHtml = true
            };
            mail.To.Add(destinatario);

            await smtp.SendMailAsync(mail);
        }
    }

    [HttpGet]
    public IActionResult RestaurarContrasenia(string token)
    {
        var usuario = repo.ObtenerByToken(token);
        if (usuario == null)
        {
            ViewBag.Error = "Token no valido";
            return View("Login");
        }
        ViewBag.Token = token;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ActualizarContrasenia(string token, string ContraseniaNueva)
    {
        var usuario = repo.ObtenerByToken(token);
        if (usuario == null)
        {
            ViewBag.Error = "Token no valido";
            return View("Login");
        }
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: ContraseniaNueva,
            salt: new byte[10],
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8
        ));
        usuario.Contrasenia = hashed;
        repo.EditarContrasenia(usuario);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["ToastMessage"] = "Contraseña modificada con éxito";
        TempData["ToastType"] = "success";
        return RedirectToAction("Login", "Usuarios");
    }
}