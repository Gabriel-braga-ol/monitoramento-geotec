using Geotrend.Domain.Entities;
using Geotrend.Domain.Interfaces;
using Geotrend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Geotrend.Infrastructure.Repositories;

public class InstrumentoRepository : IInstrumentoRepository
{
    private readonly GeotrendDbContext _context;

    public InstrumentoRepository(GeotrendDbContext context)
    {
        _context = context;
    }

    public async Task<Instrumento?> ObterPorIdAsync(Guid id)
    {
        return await _context.Instrumentos
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<Instrumento>> ObterPorBarragemIdAsync(Guid barragemId)
    {
        return await _context.Instrumentos
            .AsNoTracking()
            .Where(i => i.BarragemId == barragemId)
            .ToListAsync();
    }

    public async Task AdicionarAsync(Instrumento instrumento)
    {
        await _context.Instrumentos.AddAsync(instrumento);
        await _context.SaveChangesAsync();
    }
}