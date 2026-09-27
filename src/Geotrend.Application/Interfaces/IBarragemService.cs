using Geotrend.Application.DTOs;

namespace Geotrend.Application.Interfaces;

public interface IBarragemService
{
    Task<BarragemOutputDto> CriarAsync(CriarBarragemInputDto dto);
    Task<IEnumerable<BarragemOutputDto>> ObterTodasAsync();
}