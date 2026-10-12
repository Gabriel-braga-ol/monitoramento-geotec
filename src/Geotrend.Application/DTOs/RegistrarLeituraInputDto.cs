using System.ComponentModel.DataAnnotations;

namespace Geotrend.Application.DTOs;

public class RegistrarLeituraInputDto
{
    public Guid InstrumentoId { get; set; }
    [Required(ErrorMessage = "O valor da leitura é obrigatório.")]
    public double? Valor { get; set; }
}