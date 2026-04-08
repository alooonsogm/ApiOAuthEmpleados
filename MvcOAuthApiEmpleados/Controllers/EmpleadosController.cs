using Microsoft.AspNetCore.Mvc;
using MvcOAuthApiEmpleados.Filters;
using MvcOAuthApiEmpleados.Models;
using MvcOAuthApiEmpleados.Services;
using System.Security.Claims;

namespace MvcOAuthApiEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        private ServiceEmpleados service;

        public EmpleadosController(ServiceEmpleados service)
        {
            this.service = service;
        }

        [AuthorizeEmpleados]
        public async Task<IActionResult> Index()
        {
            List<Empleado> empleados = await this.service.GetEmpleadosAsync();
            return View(empleados);
        }

        [AuthorizeEmpleados]
        public async Task<IActionResult> Details(int idEmpleado)
        {
            Empleado empleado = await this.service.FindEmpleadoAsync(idEmpleado);
            return View(empleado);
        }

        [AuthorizeEmpleados]
        public async Task<IActionResult> PerfilEmpleado()
        {
            //Necesitamos buscar el empleado con su claim y name identifier
            var data = HttpContext.User.FindFirst(z => z.Type == ClaimTypes.NameIdentifier).Value;
            int idEmpleado = int.Parse(data);
            Empleado emp = await this.service.FindEmpleadoAsync(idEmpleado);
            return View(emp);
        }
    }
}
