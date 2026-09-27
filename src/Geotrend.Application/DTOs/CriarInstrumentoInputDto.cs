using Geotrend.Domain.Enums;

namespace Geotrend.Application.DTOs;

public class CriarInstrumentoInputDto
{
    public string Codigo { get; set; } = string.Empty;
    public TipoInstrumento Tipo { get; set; }
    public double LimiteAtencao { get; set; }
    public double LimiteAlerta { get; set; }
    public double LimiteEmergencia { get; set; }
    public Guid BarragemId { get; set; }
}