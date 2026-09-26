using Geotrend.Application.DTOs;
using Geotrend.Application.Interfaces;
using Geotrend.Domain.Entities;
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
        // 1. Busca o instrumento associado no banco de dados através da interface
        var instrumento = await _instrumentoRepository.ObterPorIdAsync(dto.InstrumentoId);

        if (instrumento == null)
        {
            throw new InvalidOperationException($"Instrumento com ID '{dto.InstrumentoId}' não foi encontrado.");
        }

        // 2. Cria a entidade Leitura (que calcula seu próprio Status automaticamente)
        var leitura = new Leitura(instrumento.Id, dto.Valor, instrumento);

        // 3. Persiste a leitura no repositório
        await _leituraRepository.AdicionarAsync(leitura);

        // 4. Mapeia a entidade do domínio para o DTO de saída
        return new LeituraOutputDto
        {
            Id = leitura.Id,
            InstrumentoId = leitura.InstrumentoId,
            DataHora = leitura.DataHora,
            Valor = leitura.Valor,
            Status = leitura.Status.ToString()
        };
    }
}