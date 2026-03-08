using APIeloT_grupo_4.Models;
using Microsoft.EntityFrameworkCore;

namespace APIeloT_grupo_4.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Leitura> Leituras { get; set; }

    public DbSet<alerta> alerta { get; set; }
}