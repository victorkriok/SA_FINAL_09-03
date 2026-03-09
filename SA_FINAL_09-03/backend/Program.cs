using api.Data;
using api.Repository;
using api.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração de CORS (Liberando geral para o Dashboard e ESP32)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Registra os serviços
builder.Services.AddScoped<AppDbContext>();
builder.Services.AddScoped<ProducaoRepository>();
builder.Services.AddHttpClient<IotApiService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("sua_chave_super_secreta_aqui_12345678")
            )
        };
    });

var app = builder.Build();

// --- CONFIGURAÇÃO DO PIPELINE (A ORDEM IMPORTA!) ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ATENÇÃO: Deixamos o app.UseHttpsRedirection() de fora para facilitar a vida do ESP32/Wokwi

app.UseCors("AllowAll"); // CORS sempre antes de Static Files e Auth

app.UseDefaultFiles();   
app.UseStaticFiles();    

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();