using Geotrend.Application.DTOs;
using Geotrend.Application.Interfaces;
using Geotrend.Domain.Entities;
using Geotrend.Domain.Interfaces;

namespace Geotrend.Application.Services;

public class BarragemService : IBarragemService
{
    private readonly IBarragemRepository _barragemRepository;

    public BarragemService(IBarragemRepository barragemRepository)
    {
        _barragemRepository = barragemRepository;
    }

    public async Task<BarragemOutputDto> CriarAsync(CriarBarragemInputDto dto)
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
}