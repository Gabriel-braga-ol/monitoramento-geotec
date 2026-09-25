using  Geotrend.Domain.Entities;

namespace Geotrend.Domain.Interfaces;

public interface IBarragemRepository
{
    // Busca uma barragem pelo seu identificador único. Retorna null se a barragem não for encontrada
    Task<Barragem?> ObterPorIdAsync(Guid id);
    // Retorna todas as barragens cadastradas, como uma coleção/lista
    Task<IEnumerable<Barragem>> ObterTodosAsync();
    // Insere no banco de dados uma nova barragem. Não retorna nada
    Task AdicionarAsync(Barragem barragem);
    // Atualiza no banco de dados os dados de uma barragem existente. Não retorna nada
    Task AtualizarAsync(Barragem barragem);
}