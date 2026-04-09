using ApiOAuthEmpleados.Helpers;
using ApiOAuthEmpleados.Models;
using ApiOAuthEmpleados.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ApiOAuthEmpleados.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadosController : ControllerBase
    {
        private RepositoryHospital repo;
        private HelperEmpleadoToken helper;

        public EmpleadosController(RepositoryHospital repo, HelperEmpleadoToken helper)
        {
            this.repo = repo;
            this.helper = helper;
        }

        [HttpGet]
        public async Task<ActionResult<List<Empleado>>> GetEmpleados()
        {
            return await this.repo.GetEmpleadosAsync();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Empleado>> FindEmpleado(int id)
        {
            return await this.repo.FindEmpleadoAsync(id);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<Empleado>> Perfil()
        {
            EmpleadoModel empleado = this.helper.GetEmpleado();
            return await this.repo.FindEmpleadoAsync(empleado.idEmpleado);
        }

        [Authorize(Roles = "PRESIDENTE")]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Empleado>>> Compis()
        {
            EmpleadoModel empleado = this.helper.GetEmpleado();
            return await this.repo.GetCompisAsync(empleado.idDepartamento);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<string>>> Oficios()
        {
            return await this.repo.GetOficiosAsync();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Empleado>>> EmpleadosOficios([FromQuery] List<string> oficios)
        {
            return await this.repo.GetEmpleadosByOficioAsync(oficios);
        }

        [HttpPut]
        [Route("[action]/{incremento}")]
        public async Task<ActionResult> IncrementarSalarios(int incremento, [FromQuery] List<string> oficios)
        {
            await this.repo.IncrementarSalarioAsync(incremento, oficios);
            return Ok();
        }
    }
}
