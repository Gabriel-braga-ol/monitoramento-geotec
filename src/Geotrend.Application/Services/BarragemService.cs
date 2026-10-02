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

        var instrumentosResumo = new List<InstrumentoResumoDto>();

        foreach (var inst in barragem.Instrumentos)
        {
            var ultimaLeitura = inst.Leituras?
                .OrderByDescending(l => l.DataHora)
                .FirstOrDefault();
            
            string statusNome = ultimaLeitura != null 
                ? ConvertEnumStatus(ultimaLeitura.Status) 
                : "Normal";

            instrumentosResumo.Add(new InstrumentoResumoDto
            {
                Id = inst.Id,
                Codigo = inst.Codigo,
                Tipo = inst.Tipo.ToString(),
                UltimoStatus = statusNome,
                UltimoValor = ultimaLeitura?.Valor,
                DataUltimaLeitura = ultimaLeitura?.DataHora
            });
        }
        
        int normal = instrumentosResumo.Count(i => i.UltimoStatus == "Normal");
        int atencao = instrumentosResumo.Count(i => i.UltimoStatus == "Atencao");
        int alerta = instrumentosResumo.Count(i => i.UltimoStatus == "Alerta");
        int emergencia = instrumentosResumo.Count(i => i.UltimoStatus == "Emergencia");

        // Define o Status Global da Barragem com base no pior status encontrado
        string statusGlobal = "Normal";
        if (emergencia > 0) statusGlobal = "Emergencia";
        else if (alerta > 0) statusGlobal = "Alerta";
        else if (atencao > 0) statusGlobal = "Atencao";

        return new BarragemStatusOutputDto
        {
            BarragemId = barragem.Id,
            NomeBarragem = barragem.Nome,
            StatusGlobal = statusGlobal,
            TotalInstrumentos = instrumentosResumo.Count,
            QuantidadeNormal = normal,
            QuantidadeAtencao = atencao,
            QuantidadeAlerta = alerta,
            QuantidadeEmergencia = emergencia,
            Instrumentos = instrumentosResumo
        };
    }
    
    private string ConvertEnumStatus(NivelAlerta status)
    {
        return status switch
        {
            NivelAlerta.Normal => "Normal",
            NivelAlerta.Atencao => "Atencao",
            NivelAlerta.Alerta => "Alerta",
            NivelAlerta.Emergencia => "Emergencia",
            _ => status.ToString()
        };
    }
}