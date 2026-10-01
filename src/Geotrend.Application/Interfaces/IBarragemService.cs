using Geotrend.Application.DTOs;

namespace Geotrend.Application.Interfaces;

public interface IBarragemService
{
    Task<BarragemOutputDto> CriarBarragemAsync(CriarBarragemInputDto dto);
    Task<IEnumerable<BarragemOutputDto>> ObterTodasAsync();
    Task<BarragemOutputDto?> ObterPorIdAsync(Guid id);
    Task<BarragemStatusOutputDto?> ObterStatusResumoAsync(Guid barragemId);
}