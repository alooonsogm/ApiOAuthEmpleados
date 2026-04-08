// See https://aka.ms/new-console-template for more information
using ClienteApiOAuthEmpleados;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

Console.WriteLine("Cliente Api OAuth");
Console.WriteLine("Introduzca apellido:");
string apellido = Console.ReadLine();
Console.WriteLine("Id empleado (Password):");
string password = Console.ReadLine();
string respuesta = await GetTokenAsync(apellido, password);
Console.WriteLine(respuesta);
Console.WriteLine("----------------------------------------------");
Console.WriteLine("Introduzca Id de empleado a buscar:");
string idEmpleado = Console.ReadLine();
string datos = await FindEmpleadoAsync(int.Parse(idEmpleado), respuesta);
Console.WriteLine(datos);

//Para crearme metodos en program, debemos hacerlos static
static async Task<string> GetTokenAsync(string user, string pass)
{
    string urlApi = "https://apioauthempleados.azurewebsites.net/";
    LogInModel model = new LogInModel
    {
        UserName = user,
        Password = pass
    };

    using(HttpClient client = new HttpClient())
    {
        string request = "api/Auth/Login";
        client.BaseAddress = new Uri(urlApi);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        string json = JsonConvert.SerializeObject(model);
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PostAsync(request, content);
        if (response.IsSuccessStatusCode == true)
        {
            string data = await response.Content.ReadAsStringAsync();
            JObject objeto = JObject.Parse(data);
            string token = objeto.GetValue("response").ToString();
            return token;
        }
        else
        {
            return "Petición incorrecta:" + response.StatusCode;
        }
    }
}

static async Task<string> FindEmpleadoAsync(int idEmpleado, string token)
{
    using (HttpClient client = new HttpClient())
    {
        string urlApi = "https://apioauthempleados.azurewebsites.net/";
        string request = "api/Empleados/" + idEmpleado;
        client.BaseAddress = new Uri(urlApi);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //Lo unico de novedad es incluir authorization con bearer token dentro del header.
        client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
        HttpResponseMessage response = await client.GetAsync(request);
        if (response.IsSuccessStatusCode == true)
        {
            string data = await response.Content.ReadAsStringAsync();
            return data;
        }
        else
        {
            return "Error de algo: " + response.StatusCode;
        }
    }
}