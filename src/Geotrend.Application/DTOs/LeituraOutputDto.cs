namespace Geotrend.Application.DTOs;

public class LeituraOutputDto
{
    public Guid Id { get; set; }
    public Guid InstrumentoId { get; set; }
    public DateTime DataHora { get; set; }
    public double Valor { get; set; }
    public string Status { get; set; } = string.Empty;
}