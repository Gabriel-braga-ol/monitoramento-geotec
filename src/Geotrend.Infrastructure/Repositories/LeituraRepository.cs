using Geotrend.Domain.Entities;
using Geotrend.Domain.Interfaces;
using Geotrend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Geotrend.Infrastructure.Repositories;

public class LeituraRepository : ILeituraRepository
{
    private readonly GeotrendDbContext _context;

    public LeituraRepository(GeotrendDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(Leitura leitura)
    {
        await _context.Leituras.AddAsync(leitura);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Leitura>> ObterUltimasLeiturasPorInstrumentoAsync(Guid instrumentoId, int quantidade)
    {
        return await _context.Leituras
            .AsNoTracking()
            .Where(l => l.InstrumentoId == instrumentoId)
            .OrderByDescending(l => l.DataHora) // Ordena da mais recente para a mais antiga
            .Take(quantidade) // Limita a quantidade para plotar gráficos leves no React
            .ToListAsync();
    }
}