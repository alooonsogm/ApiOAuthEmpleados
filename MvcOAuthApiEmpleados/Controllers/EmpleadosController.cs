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
            Empleado emp = await this.service.GetPerfilAsync();
            return View(emp);
        }

        [AuthorizeEmpleados]
        public async Task<IActionResult> Compis()
        {
            List<Empleado> empleados = await this.service.GetCompisAsync();
            return View(empleados);
        }
    }
}
