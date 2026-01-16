using LoggAutorz.DataBase;
using LoggAutorz.Middleware;
using Microsoft.EntityFrameworkCore;
using LoggAutorz.ServicesDb;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using LoggAutorz.Configuration;
using static LoggAutorz.ServicesDb.ServicesToApiHttp;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<JwtToken>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var secret = config["JwtSettings:PrivateKey"];
    return new JwtToken(secret);
});

builder.Services.AddScoped<ServicesToApiHttp>();
var connectionStrings = builder.Configuration.GetConnectionString("AppDbConnectionString");
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(connectionStrings, ServerVersion.AutoDetect(connectionStrings)));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<GenerateService>();
var app = builder.Build();


if (app.Environment.IsDevelopment()) 
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CRUD API V1");
    });
}

app.UseMiddleware<MiddlewareException>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGet("/", (GenerateService service) => Results.Ok("API online")); ;

app.MapControllers();

app.Run();
