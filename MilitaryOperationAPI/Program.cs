using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MilitaryOperationAPI.Data;
using MilitaryOperationAPI.Filters;
using MilitaryOperationAPI.Helpers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Military Operation API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid Token",
        Name = "Miltary Operation Authentication",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    options.OperationFilter<AuthResponsesOperationFilter>();
});

/*
 * 
 * #1 Provide UserAuthentication with JWTbearer first
 * #2 Provide UserAuthorization and give user Policy or permission
 * 
 */

var dbProvider = builder.Configuration.GetConnectionString("Provider");
builder.Services.AddDbContext<AppDBContext>(options =>
{
    if (dbProvider == "SqlServer")
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnectionString"));
    }
});

builder.Services.AddScoped<IConfiguration>(sp => sp.GetRequiredService<IConfiguration>());

// Add Logging
var serviceProvider = builder.Services.BuildServiceProvider();
var controllersLogger = serviceProvider.GetRequiredService<ILogger<ControllerBase>>();
builder.Services.AddSingleton(typeof(ILogger), controllersLogger);

// Add AutoMapper
var autoMapperLicense = builder.Configuration["AutoMapper:LicenseKey"] ?? Environment.GetEnvironmentVariable("AUTOMAPPER_LICENSE_KEY");
builder.Services.AddAutoMapper(config =>
{
    config.LicenseKey = autoMapperLicense;
},
typeof(Program).Assembly
);

// Add Authentication (AddAuthentication on services register this into the dependencies injections)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // This line suggest the schema for authentication is the Bearer Token
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt => //register JwtBearer to use as authentication (pass in a handler to handle jwtbearer auth)
{
    // Define how token will be parse and validated jwt.TokenValidationParametsr specify the parameters which the handler uses to validte token
    jwt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "")),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
