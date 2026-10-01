using Geotrend.Application.DTOs;
using Geotrend.Application.Interfaces;
using Geotrend.Domain.Entities;
using Geotrend.Domain.Enums;
using Geotrend.Domain.Interfaces;

namespace Geotrend.Application.Services;

public class BarragemService : IBarragemService
{
    private readonly IBarragemRepository _barragemRepository;
    private readonly IInstrumentoRepository _instrumentoRepository;

    public BarragemService(
        IBarragemRepository barragemRepository, 
        IInstrumentoRepository instrumentoRepository)
    {
        _barragemRepository = barragemRepository;
        _instrumentoRepository = instrumentoRepository;
    }

    public async Task<BarragemOutputDto> CriarBarragemAsync(CriarBarragemInputDto dto)
    {
        var barragem = new Barragem(dto.Nome, dto.Localizacao);
        await _barragemRepository.AdicionarAsync(barragem);

        return new BarragemOutputDto
        {
            Id = barragem.Id,
            Nome = barragem.Nome,
            Localizacao = barragem.Localizacao
        };
    }

    public async Task<IEnumerable<BarragemOutputDto>> ObterTodasAsync()
    {
        var barragens = await _barragemRepository.ObterTodosAsync();
        return barragens.Select(b => new BarragemOutputDto
        {
            Id = b.Id,
            Nome = b.Nome,
            Localizacao = b.Localizacao
        });
    }
    
    public async Task<BarragemOutputDto?> ObterPorIdAsync(Guid id)
    {
        var barragem = await _barragemRepository.ObterPorIdAsync(id);
        if (barragem == null) return null;

        return new BarragemOutputDto
        {
            Id = barragem.Id,
            Nome = barragem.Nome,
            Localizacao = barragem.Localizacao
        };
    }
    
    public async Task<BarragemStatusOutputDto?> ObterStatusResumoAsync(Guid barragemId)
    {
        var barragem = await _barragemRepository.ObterPorIdAsync(barragemId);
        if (barragem == null) return null;

        var instrumentos = await _instrumentoRepository.ObterPorBarragemIdAsync(barragemId);

        var resumo = new BarragemStatusOutputDto
        {
            BarragemId = barragem.Id,
            NomeBarragem = barragem.Nome,
            TotalInstrumentos = instrumentos.Count()
        };

        var piorStatus = NivelAlerta.Normal;

        foreach (var inst in instrumentos)
        {
            var ultimaLeitura = inst.Leituras.OrderByDescending(l => l.DataHora).FirstOrDefault();
            var statusInst = ultimaLeitura?.Status ?? NivelAlerta.Normal;

            switch (statusInst)
            {
                case NivelAlerta.Atencao: resumo.QuantidadeAtencao++; break;
                case NivelAlerta.Alerta: resumo.QuantidadeAlerta++; break;
                case NivelAlerta.Emergencia: resumo.QuantidadeEmergencia++; break;
                default: resumo.QuantidadeNormal++; break;
            }

            if (statusInst > piorStatus)
            {
                piorStatus = statusInst;
            }

            resumo.Instrumentos.Add(new InstrumentoResumoDto
            {
                Id = inst.Id,
                Codigo = inst.Codigo,
                Tipo = inst.Tipo.ToString(),
                UltimoStatus = statusInst.ToString(),
                UltimoValor = ultimaLeitura?.Valor,
                DataUltimaLeitura = ultimaLeitura?.DataHora
            });
        }

        resumo.StatusGlobal = piorStatus.ToString();
        return resumo;
    }
}