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
        private IConfiguration configuration;

        public EmpleadosController(RepositoryHospital repo, IConfiguration configuration)
        {
            this.repo = repo;
            this.configuration = configuration;
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
            Claim claim = HttpContext.User.FindFirst(z => z.Type == "UserData");
            string jsonEncriptado = claim.Value;
            string secretKey = this.configuration.GetValue<string>("ConfiguracionCifrado:LlaveSecreta");
            string jsonEmpleado = HelperCifrado.DecryptString(secretKey, jsonEncriptado);
            Empleado empleado = JsonConvert.DeserializeObject<Empleado>(jsonEmpleado);
            return await this.repo.FindEmpleadoAsync(empleado.idEmpleado);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Empleado>>> Compis()
        {
            Claim claim = HttpContext.User.FindFirst(z => z.Type == "UserData");
            string jsonEncriptado = claim.Value;
            string secretKey = this.configuration.GetValue<string>("ConfiguracionCifrado:LlaveSecreta");
            string jsonEmpleado = HelperCifrado.DecryptString(secretKey, jsonEncriptado);
            Empleado empleado = JsonConvert.DeserializeObject<Empleado>(jsonEmpleado);
            return await this.repo.GetCompisAsync(empleado.idDepartamento);
        }
    }
}
