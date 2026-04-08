using Microsoft.AspNetCore.Mvc;
using MvcOAuthApiEmpleados.Models;
using MvcOAuthApiEmpleados.Services;

namespace MvcOAuthApiEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        private ServiceEmpleados service;

        public EmpleadosController(ServiceEmpleados service)
        {
            this.service = service;
        }

        public async Task<IActionResult> Index()
        {
            List<Empleado> empleados = await this.service.GetEmpleadosAsync();
            return View(empleados);
        }

        public async Task<IActionResult> Details(int idEmpleado)
        {
            //Tendremos el token en session
            string token = HttpContext.Session.GetString("TOKEN");
            if (token == null)
            {
                ViewData["MENSAJE"] = "Debe hacer Log In";
                return View();
            }
            else
            {
                Empleado empleado = await this.service.FindEmpleadoAsync(idEmpleado, token);
                return View(empleado);
            }
        }
    }
}
