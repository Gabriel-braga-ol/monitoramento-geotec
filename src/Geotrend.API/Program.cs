using Geotrend.Application.Interfaces;
using Geotrend.Application.Services;
using Geotrend.Domain.Interfaces;
using Geotrend.Infrastructure.Data;
using Geotrend.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// DbContext da aplicação
builder.Services.AddDbContext<GeotrendDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Injeção de Dependências dos Repositórios
builder.Services.AddScoped<IBarragemRepository, BarragemRepository>();
builder.Services.AddScoped<IInstrumentoRepository, InstrumentoRepository>();
builder.Services.AddScoped<ILeituraRepository, LeituraRepository>();

// Injeção de Dependências dos Serviços
builder.Services.AddScoped<IBarragemService, BarragemService>();
builder.Services.AddScoped<IInstrumentoService, InstrumentoService>();
builder.Services.AddScoped<ILeituraService, LeituraService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthorization();
app.MapControllers();

app.Run();