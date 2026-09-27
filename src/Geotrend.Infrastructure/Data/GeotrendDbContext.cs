using Geotrend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Geotrend.Infrastructure.Data;

public class GeotrendDbContext : DbContext
{
    public GeotrendDbContext(DbContextOptions<GeotrendDbContext> options) : base(options)
    {
        
    }
    
    // Mapeamento das entidades para tabelas no BD
    // Cada Dbset representa uma tabela física que será criada no PostgeeSQL
    public DbSet<Barragem> Barragens => Set<Barragem>();
    public DbSet<Instrumento> Instrumentos => Set<Instrumento>();
    public DbSet<Leitura> Leituras => Set<Leitura>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configurando a tabela barragem
        modelBuilder.Entity<Barragem>(builder =>
        {
            builder.ToTable("Barragem");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Nome).IsRequired().HasMaxLength(150);
            builder.Property(b => b.Localizacao).HasMaxLength(200);
            
            // Relacionamento 1:N (1 barragem possui N instrumentos)
            builder.HasMany(b => b.Instrumentos)
                .WithOne()
                .HasForeignKey(i => i.BarragemId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Configurando Tabela Instrumento
        modelBuilder.Entity<Instrumento>(builder =>
        {
            builder.ToTable("Instrumentos");
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Codigo).IsRequired().HasMaxLength(50);
            
            // Relacionamento 1:N (1 instrumento possui N leitoras)
            builder.HasMany(i => i.Leituras)
                .WithOne()
                .HasForeignKey(l => l.InstrumentoId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Configurandoo a tabela Leitura
        modelBuilder.Entity<Leitura>(builder =>
        {
            builder.ToTable("Leituras");
            builder.HasKey(l => l.Id);
            builder.Property(l => l.Valor).IsRequired();
            builder.Property(l => l.DataHora).IsRequired();
        });
    }
}