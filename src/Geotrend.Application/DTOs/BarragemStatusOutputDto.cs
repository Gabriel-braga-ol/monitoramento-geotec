namespace Geotrend.Application.DTOs;

public class InstrumentoResumoDto
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string UltimoStatus { get; set; } = string.Empty;
    public double? UltimoValor { get; set; }
    public DateTime? DataUltimaLeitura { get; set; }
}

public class BarragemStatusOutputDto
{
    public Guid BarragemId { get; set; }
    public string NomeBarragem { get; set; } = string.Empty;
    public string StatusGlobal { get; set; } = "Normal";
    public int TotalInstrumentos { get; set; }
    public int QuantidadeNormal { get; set; }
    public int QuantidadeAtencao { get; set; }
    public int QuantidadeAlerta { get; set; }
    public int QuantidadeEmergencia { get; set; }
    public List<InstrumentoResumoDto> Instrumentos { get; set; } = new();
}