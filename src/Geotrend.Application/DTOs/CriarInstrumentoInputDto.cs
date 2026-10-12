using Geotrend.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Geotrend.Application.DTOs;

public class CriarInstrumentoInputDto
{
    public string Codigo { get; set; } = string.Empty;
    [EnumDataType(typeof(TipoInstrumento),
        ErrorMessage = "O tipo de instrumento insformado é inválido.")]
    public TipoInstrumento Tipo { get; set; }
    public double LimiteAtencao { get; set; }
    public double LimiteAlerta { get; set; }
    public double LimiteEmergencia { get; set; }
    public Guid BarragemId { get; set; }
}