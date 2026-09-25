using Geotrend.Domain.Entities;

namespace Geotrend.Domain.Interfaces;

public interface IInstrumentoRepository
{
    Task<Instrumento?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<Instrumento>> ObterPorBarragemIdAsync(Guid barragemId);
    // recebe um instrumento e o adiciona na base de dados
    Task AdicionarAsync(Instrumento instrumento);
}