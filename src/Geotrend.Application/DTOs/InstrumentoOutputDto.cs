namespace Geotrend.Application.DTOs;

public class InstrumentoOutputDto
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public double LimiteAtencao { get; set; }
    public double LimiteAlerta { get; set; }
    public double LimiteEmergencia { get; set; }
    public Guid BarragemId { get; set; }
}