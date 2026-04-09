using ApiOAuthEmpleados.Helpers;
using ApiOAuthEmpleados.Models;
using ApiOAuthEmpleados.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiOAuthEmpleados.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RepositoryHospital repo;
        private HelperActionOAuthService helper;
        private IConfiguration configuration;

        public AuthController(RepositoryHospital repo, HelperActionOAuthService helper, IConfiguration configuration)
        {
            this.repo = repo;
            this.helper = helper;
            this.configuration = configuration;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> Login(LogInModel model)
        {
            Empleado empleado = await this.repo.LogInEmpleadoAsync(model.UserName, int.Parse(model.Password));
            if (empleado == null)
            {
                return Unauthorized();
            }
            else
            {
                //Debemos crear unas credenciales con nuestro token
                SigningCredentials credentials = new SigningCredentials(this.helper.GetKeyToken(), SecurityAlgorithms.HmacSha256);

                //Creamos nuestro ModeloEmpleado para almacenar lo datos que se necesitan en el token.
                EmpleadoModel modelEmp = new EmpleadoModel
                {
                    idEmpleado = empleado.idEmpleado,
                    Apellido = empleado.Apellido,
                    Oficio = empleado.Oficio,
                    Salario = empleado.Salario,
                    idDepartamento = empleado.idDepartamento
                };

                string jsonEmpleado = JsonConvert.SerializeObject(modelEmp);
                string jsonCifrado = HelperCifrado.CifrarString(jsonEmpleado);
                //Creamos un array de Claims, que es lo que se guarda en el token (se puede toda la info que nos apetezca)
                //Aqui almacenamos el rol del usuario.
                Claim[] informacion = new[]
                {
                    new Claim("UserData", jsonCifrado),
                    new Claim(ClaimTypes.Role, empleado.Oficio)
                };
                //El token se genera con una clase y debemos almacenar los datos de issuer, credentials...
                JwtSecurityToken token = new JwtSecurityToken(
                    claims: informacion,
                    issuer: this.helper.Issuer,
                    audience: this.helper.Audience,
                    signingCredentials: credentials,
                    expires: DateTime.UtcNow.AddMinutes(20),
                    notBefore: DateTime.UtcNow
                    );
                //Por ultimo devolvemos la respuesta afirmatica con el token.
                return Ok(new
                {
                    response = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
        }
    }
}
