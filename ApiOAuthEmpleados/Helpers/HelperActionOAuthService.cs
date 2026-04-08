using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ApiOAuthEmpleados.Helpers
{
    public class HelperActionOAuthService
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }

        public HelperActionOAuthService(IConfiguration configuration)
        {
            this.Issuer = configuration.GetValue<string>("ApiOAuthToken:Issuer");
            this.Audience = configuration.GetValue<string>("ApiOAuthToken:Audience");
            this.SecretKey = configuration.GetValue<string>("ApiOAuthToken:SecretKey");
        }

        //Necesitamos un metodo para generar el Token a partir de nuestro secret key.
        public SymmetricSecurityKey GetKeyToken()
        {
            //Convertimos a Byte nuestro secret key
            byte[] data = Encoding.UTF8.GetBytes(this.SecretKey);
            return new SymmetricSecurityKey(data);
        }

        //Utilizamos clases action para separar la capa de los services de autorizacion del program.
        public Action<JwtBearerOptions> GetJWtBearerOptions()
        {
            Action<JwtBearerOptions> options = new Action<JwtBearerOptions>(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = this.Issuer,
                    ValidAudience = this.Audience,
                    IssuerSigningKey = this.GetKeyToken()
                };
            });
            return options;
        }

        //El esquema de nuestra validacion JwtBearerDefaults
        public Action<AuthenticationOptions> GetAuthenticationSchema()
        {
            Action<AuthenticationOptions> options = new Action<AuthenticationOptions>(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            });
            return options;
        }
    }
}
