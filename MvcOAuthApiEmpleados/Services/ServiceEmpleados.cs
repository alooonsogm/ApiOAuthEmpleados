using Microsoft.AspNetCore.Mvc;
using MvcOAuthApiEmpleados.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace MvcOAuthApiEmpleados.Services
{
    public class ServiceEmpleados
    {
        private string UrlApi;
        private MediaTypeWithQualityHeaderValue header;
        private IHttpContextAccessor contextAccessor;
        
        public ServiceEmpleados(IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            this.UrlApi = configuration.GetValue<string>("ApiUrls:ApiEmpleados");
            this.header = new MediaTypeWithQualityHeaderValue("application/json");
            this.contextAccessor = contextAccessor;
        }

        public async Task<string> LogInAsync(string user, string pass)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/Auth/Login";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                LogInModel model = new LogInModel
                {
                    UserName = user,
                    Password = pass
                };

                string json = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode == true)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    JObject keys = JObject.Parse(data);
                    string token = keys.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode == true)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        //Realizamos una sobrecarga del metodo
        private async Task<T> CallApiAsync<T>(string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode == true)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            string request = "api/Empleados";
            List<Empleado> empleados = await this.CallApiAsync<List<Empleado>>(request);
            return empleados;
        }

        public async Task<Empleado> FindEmpleadoAsync(int idEmpleado)
        {
            string request = "api/Empleados/" + idEmpleado;
            string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN").Value;
            Empleado empleado = await this.CallApiAsync<Empleado>(request, token);
            return empleado;
        }

        public async Task<Empleado> GetPerfilAsync()
        {
            string request = "api/Empleados/Perfil";
            string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN").Value;
            Empleado empleado = await this.CallApiAsync<Empleado>(request, token);
            return empleado;
        }

        public async Task<List<Empleado>> GetCompisAsync()
        {
            string request = "api/Empleados/Compis";
            string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN").Value;
            List<Empleado> empleados = await this.CallApiAsync<List<Empleado>>(request, token);
            return empleados;
        }

        public async Task<List<string>> GetOficiosAsync()
        {
            string request = "api/Empleados/Oficios";
            List<string> oficios = await this.CallApiAsync<List<string>>(request);
            return oficios;
        }

        //Tanto en incrementar como el buscar empleados, necesitamos generar el siguiente string para el request
        //oficios=ANALISTA&oficios=DIRECTOR
        //A partir de una coleccion
        private string TrasnformarColeccionToQuery(List<string> collection)
        {
            string result = "";
            foreach (string oficio in collection)
            {
                result += "oficios=" + oficio + "&";
            }
            result = result.TrimEnd('&');
            return result;
        }

        public async Task<List<Empleado>> GetEmpleadosOficiosAsync(List<string> oficios)
        {
            string request = "api/Empleados/EmpleadosOficios";
            string data = this.TrasnformarColeccionToQuery(oficios);
            List<Empleado> empleados = await this.CallApiAsync<List<Empleado>>(request + "?" + data);
            return empleados;
        }

        public async Task UpdateEmpleadosAsync(int incremento, List<string> oficios)
        {
            string request = "api/Empleados/IncrementarSalarios/" + incremento;
            string data = this.TrasnformarColeccionToQuery(oficios);
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                HttpResponseMessage response = await client.PutAsync(request + "?" + data, null);
            }
        }
    }
}
