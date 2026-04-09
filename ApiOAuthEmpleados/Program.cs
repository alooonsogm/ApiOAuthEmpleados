using ApiOAuthEmpleados.Data;
using ApiOAuthEmpleados.Helpers;
using ApiOAuthEmpleados.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<HelperEmpleadoToken>();
HelperCifrado.Initialize(builder.Configuration);
//Creamos una instancia de nuestro helper.
HelperActionOAuthService helper = new HelperActionOAuthService(builder.Configuration);
//Esta instancia solamente debemos crearla una vez
builder.Services.AddSingleton<HelperActionOAuthService>(helper);
//Habiltamos la seguridad dentro de program
builder.Services.AddAuthentication(helper.GetAuthenticationSchema()).AddJwtBearer(helper.GetJWtBearerOptions());

// Add services to the container.

string connectiionString = builder.Configuration.GetConnectionString("SqlHospital");
builder.Services.AddTransient<RepositoryHospital>();
builder.Services.AddDbContext<HospitalContext>(options => options.UseSqlServer(connectiionString));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
}

app.MapOpenApi();
app.MapScalarApiReference();
app.MapGet("/", context =>
{
    context.Response.Redirect("/scalar");
    return Task.CompletedTask;
});
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
