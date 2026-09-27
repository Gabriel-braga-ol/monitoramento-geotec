using Geotrend.Domain.Entities;
using Geotrend.Domain.Interfaces;
using Geotrend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Geotrend.Infrastructure.Repositories;

public class BarragemRepository : IBarragemRepository
{
    private readonly GeotrendDbContext _context;
    
    public BarragemRepository(GeotrendDbContext context)
    {
        _context = context;
    }

    public async Task<Barragem?> ObterPorIdAsync(Guid id)
    {
        // Busca a barragem pelo ID e inclui os seus instrumentos na consulta (Eager Loading)
        return await _context.Barragens
            .Include(b => b.Instrumentos)
            .FirstOrDefaultAsync(b => b.Id == id);
    }
    
    public async Task<IEnumerable<Barragem>> ObterTodosAsync()
    {
        return await _context.Barragens
            .AsNoTracking() // AsNoTracking otimiza consultas de leitura
            .ToListAsync(); // Realiza uma consilta SELECT * FROM "Barragens"
    }
    
    public async Task AdicionarAsync(Barragem barragem)
    {
        await _context.Barragens.AddAsync(barragem);
        await _context.SaveChangesAsync(); // Efetiva o INSERT no banco de dados
    }
    
    public async Task AtualizarAsync(Barragem barragem)
    {
        _context.Barragens.Update(barragem);
        await _context.SaveChangesAsync(); // Efetiva o UPDATE no banco de dados
    }
}