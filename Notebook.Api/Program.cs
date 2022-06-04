using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Notebook.Api.Data;
using Notebook.Api.Models;
using Notebook.Api.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// EF Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<NotebookContext>(options => options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());
builder.Services.AddTransient<INoteRepository, NoteRepository>();

// Identity
builder.Services.AddIdentity<NotebookUser, NotebookRole>().AddEntityFrameworkStores<NotebookContext>();

// JWT
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSigningKey"]));
builder.Services.AddTransient<ITokenService, JwtTokenService>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new()
    {
        IssuerSigningKey = signingKey,
        // TODO Need to change these in the future
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
