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

    public async Task<IEnumerable<Leitura>> ObterUltimasLeiturasPorInstrumentoAsync(
        Guid instrumentoId, 
        DateTime? dataInicio,
        DateTime? dataFim,
        int pagina,
        int tamanhoPagina)
    {
        var query = _context.Leituras.AsNoTracking()
            .Where(l => l.InstrumentoId == instrumentoId);

        if (dataInicio.HasValue)
            query = query.Where(l => l.DataHora >= DateTime.SpecifyKind(dataInicio.Value, DateTimeKind.Utc));

        if (dataFim.HasValue)
            query = query.Where(l => l.DataHora <= DateTime.SpecifyKind(dataFim.Value, DateTimeKind.Utc));

        return await query
            .OrderByDescending(l => l.DataHora)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();
    }
}