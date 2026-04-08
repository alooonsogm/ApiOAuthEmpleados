using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MvcOAuthApiEmpleados.Models;
using MvcOAuthApiEmpleados.Services;
using System.Security.Claims;

namespace MvcOAuthApiEmpleados.Controllers
{
    public class ManagedController : Controller
    {
        private ServiceEmpleados service;

        public ManagedController(ServiceEmpleados service)
        {
            this.service = service;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LogInModel model)
        {
            string token = await this.service.LogInAsync(model.UserName, model.Password);
            if (token == null)
            {
                ViewData["MENSAJE"] = "Credenciales incorrectas.";
                return View();
            }
            else
            {
                ViewData["MENSAJE"] = "Ya tienes tu token!!";
                HttpContext.Session.SetString("TOKEN", token);
                ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                //Almacenamos el nombre del usuario (Bonito)
                identity.AddClaim(new Claim(ClaimTypes.Name, model.UserName));
                //Almacenamos el password del usuario como identifier.
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, model.Password));
                identity.AddClaim(new Claim("TOKEN", token));
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                //Damos de alta al usuario durante 20 minutos
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
                });
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
