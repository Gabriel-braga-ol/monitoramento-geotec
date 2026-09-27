using Geotrend.Application.DTOs;

namespace Geotrend.Application.Interfaces;

public interface IInstrumentoService
{
    Task<InstrumentoOutputDto> CriarAsync(CriarInstrumentoInputDto dto);
}