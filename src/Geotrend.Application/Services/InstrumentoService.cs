using Geotrend.Application.DTOs;
using Geotrend.Application.Interfaces;
using Geotrend.Domain.Entities;
using Geotrend.Domain.Interfaces;

namespace Geotrend.Application.Services;

public class InstrumentoService : IInstrumentoService
{
    private readonly IInstrumentoRepository _instrumentoRepository;
    private readonly IBarragemRepository _barragemRepository;

    public InstrumentoService(IInstrumentoRepository instrumentoRepository, IBarragemRepository barragemRepository)
    {
        _instrumentoRepository = instrumentoRepository;
        _barragemRepository = barragemRepository;
    }

    public async Task<InstrumentoOutputDto> CriarAsync(CriarInstrumentoInputDto dto)
    {
        var barragem = await _barragemRepository.ObterPorIdAsync(dto.BarragemId);
        if (barragem == null)
            throw new InvalidOperationException($"Barragem com ID '{dto.BarragemId}' não foi encontrada.");

        var instrumento = new Instrumento(
            dto.Codigo,
            dto.Tipo,
            dto.LimiteAtencao,
            dto.LimiteAlerta,
            dto.LimiteEmergencia,
            dto.BarragemId
        );

        await _instrumentoRepository.AdicionarAsync(instrumento);

        return new InstrumentoOutputDto
        {
            Id = instrumento.Id,
            Codigo = instrumento.Codigo,
            Tipo = instrumento.Tipo.ToString(),
            LimiteAtencao = instrumento.LimiteAtencao,
            LimiteAlerta = instrumento.LimiteAlerta,
            LimiteEmergencia = instrumento.LimiteEmergencia,
            BarragemId = instrumento.BarragemId
        };
    }
}