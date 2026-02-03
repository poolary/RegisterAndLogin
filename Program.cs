using LoggAutorz.DataBase;
using LoggAutorz.Middleware;
using Microsoft.EntityFrameworkCore;
using LoggAutorz.ServicesDb;
//using Microsoft.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using Swashbuckle.AspNetCore.Swagger;


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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = false, // Ajuste conforme sua necessidade
         ValidateAudience = false,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:PrivateKey"]))
     };
 });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminMax", policy => 
        policy.RequireRole("AdminMax"));
    options.AddPolicy("Admin", policy =>
        policy.RequireRole("Admin"));
    options.AddPolicy("Employee", policy =>
        policy.RequireRole("Employee"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(/*c =>
{

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api Test", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JwtSettings:PrivateKey"
    });
}*/
);

builder.Services.AddScoped<GenerateService>();
var app = builder.Build();


if (app.Environment.IsDevelopment()) 
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "Admin");
    });
}

app.UseMiddleware<MiddlewareException>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapGet("/", (GenerateService service) => service.Generate(null)); 

app.MapControllers();

app.Run();
