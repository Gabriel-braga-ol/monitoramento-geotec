using Geotrend.Application.DTOs;
using Geotrend.Domain.Entities;

namespace Geotrend.Application.Interfaces;

public interface ILeituraService
{
    Task<LeituraOutputDto> RegistrarLeituraAsync(RegistrarLeituraInputDto dto);
}