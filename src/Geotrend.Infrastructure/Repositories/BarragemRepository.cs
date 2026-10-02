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

    // 1. Busca instrumentos e leituras por ID 
    public async Task<Barragem?> ObterPorIdAsync(Guid id)
    {
        return await _context.Barragens
            .Include(b => b.Instrumentos)
            .ThenInclude(i => i.Leituras)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    // Busca todas as barragens
    public async Task<IEnumerable<Barragem>> ObterTodosAsync()
    {
        return await _context.Barragens
            .Include(b => b.Instrumentos)
            .ThenInclude(i => i.Leituras)
            .ToListAsync();
    }
    
    // 3. Adicionar nova barragem
    public async Task AdicionarAsync(Barragem barragem)
    {
        await _context.Barragens.AddAsync(barragem);
        await _context.SaveChangesAsync();
    }
    
    // Atualiza barragem existente
    public async Task AtualizarAsync(Barragem barragem)
    {
        _context.Barragens.Update(barragem);
        await _context.SaveChangesAsync();
    }
    
    public async Task RemoverAsync(Guid id)
    {
        var barragem = await ObterPorIdAsync(id);
        if (barragem != null)
        {
            _context.Barragens.Remove(barragem);
            await _context.SaveChangesAsync();
        }
    }
}