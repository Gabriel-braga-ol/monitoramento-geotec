using Geotrend.Application.Interfaces;
using Geotrend.Application.Services;
using Geotrend.Domain.Interfaces;
using Geotrend.Infrastructure.Data;
using Geotrend.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar o DbContext com PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<GeotrendDbContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Injeção de Dependência - Repositórios (Infraestrutura)
builder.Services.AddScoped<IBarragemRepository, BarragemRepository>();
builder.Services.AddScoped<IInstrumentoRepository, InstrumentoRepository>();
builder.Services.AddScoped<ILeituraRepository, LeituraRepository>();

// 3. Injeção de Dependência - Serviços (Aplicação)
builder.Services.AddScoped<ILeituraService, LeituraService>();
builder.Services.AddScoped<IBarragemService, BarragemService>();
builder.Services.AddScoped<IInstrumentoService, InstrumentoService>();

// 4. Configurar Controllers e Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuração do Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
