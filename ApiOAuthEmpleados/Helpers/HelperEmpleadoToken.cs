using ApiOAuthEmpleados.Models;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ApiOAuthEmpleados.Helpers
{
    public class HelperEmpleadoToken
    {
        private IHttpContextAccessor contextAccessor;

        public HelperEmpleadoToken(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        public EmpleadoModel GetEmpleado()
        {
            Claim claim = this.contextAccessor.HttpContext.User.FindFirst(z => z.Type == "UserData");
            string jsonCifrado = claim.Value;
            string jsonEmpleado = HelperCifrado.DescifrarString(jsonCifrado);
            EmpleadoModel empleado = JsonConvert.DeserializeObject<EmpleadoModel>(jsonEmpleado);
            return empleado;
        }
    }
}
