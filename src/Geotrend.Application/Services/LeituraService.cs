using Geotrend.Application.DTOs;
using Geotrend.Application.Interfaces;
using Geotrend.Domain.Interfaces;

namespace Geotrend.Application.Services;

public class LeituraService : ILeituraService
{
    private readonly IInstrumentoRepository _instrumentoRepository;
    private readonly ILeituraRepository _leituraRepository;

    // Injeção de Dependência: o serviço recebe os repositórios prontos através do construtor
    public LeituraService(IInstrumentoRepository instrumentoRepository, ILeituraRepository leituraRepository)
    {
        _instrumentoRepository = instrumentoRepository;
        _leituraRepository = leituraRepository;
    }

    public async Task<LeituraOutputDto> RegistrarLeituraAsync(RegistrarLeituraInputDto dto)
    {
        // Busca o instrumento associado no banco de dados através da interface
        var instrumento = await _instrumentoRepository.ObterPorIdAsync(dto.InstrumentoId);

        if (instrumento == null)
        {
            throw new InvalidOperationException($"Instrumento com ID '{dto.InstrumentoId}' não foi encontrado.");
        }

        // Cria a entidade Leitura (que calcula seu próprio Status automaticamente)
        var leitura = instrumento.AdicionarLeitura(dto.Valor);

        // Persiste a leitura no repositório
        await _leituraRepository.AdicionarAsync(leitura);

        // Mapeia a entidade do domínio para o DTO de saída
        return new LeituraOutputDto
        {
            Id = leitura.Id,
            InstrumentoId = leitura.InstrumentoId,
            DataHora = leitura.DataHora,
            Valor = leitura.Valor,
            Status = leitura.Status.ToString()
        };
    }
    public async Task<IEnumerable<LeituraOutputDto>> ObterHistoricoPorInstrumentoAsync(
        Guid instrumentoId,
        ObterLeiturasFiltroInputDto filtro)
    {
        var leituras = await _leituraRepository.ObterUltimasLeiturasPorInstrumentoAsync(
            instrumentoId,
            filtro.DataInicio,
            filtro.DataFim,
            filtro.Pagina,
            filtro.TamanhoPagina);

        return leituras.Select(l => new LeituraOutputDto
        {
            Id = l.Id,
            InstrumentoId = l.InstrumentoId,
            DataHora = l.DataHora,
            Valor = l.Valor,
            Status = l.Status.ToString()
        });
    }

}