using Microsoft.EntityFrameworkCore;
using APIeloT_grupo_4.Data;

var builder = WebApplication.CreateBuilder(args);

var stringDeConexao = "Server=localhost;Database=EstoqueSA;User=root;Password=;";

builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(stringDeConexao, ServerVersion.AutoDetect(stringDeConexao)));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5050);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();