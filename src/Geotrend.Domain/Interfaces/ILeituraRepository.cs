using Geotrend.Domain.Entities;

namespace Geotrend.Domain.Interfaces;

public interface ILeituraRepository
{
    // recebe uma nova leitura e adiciona na base de dados
    Task AdicionarAsync(Leitura leitura);
    // recebe um instrumento e a qtd e devolve uma coleção/lista com a qtd leituras
    Task<IEnumerable<Leitura>> ObterUltimasLeiturasPorInstrumentoAsync(Guid instrumentoId, int quantidade);
}