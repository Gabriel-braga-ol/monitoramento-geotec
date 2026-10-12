using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Geotrend.Application.DTOs;

namespace Geotrend.UnitTests.Application;

public class CriarInstrumentoInputDtoTests
{
    [Theory]
    [InlineData("{}")]
    [InlineData("{\"tipo\":0}")]
    [InlineData("{\"tipo\":-1}")]
    [InlineData("{\"tipo\":5}")]
    [InlineData("{\"tipo\":999}")]
    public void Validar_TipoAusenteOuInvalido_DeveRejeitar(string json)
    {
        var dto = CriarDto(json);
        var erros = new List<ValidationResult>();

        var valido = Validator.TryValidateObject(dto, new ValidationContext(dto), erros, true);

        Assert.False(valido);
        Assert.Contains(erros, erro => erro.MemberNames.Contains(nameof(dto.Tipo)));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void Validar_TipoDefinido_DeveAceitar(int tipo)
    {
        var dto = CriarDto($"{{\"tipo\":{tipo}}}");
        var erros = new List<ValidationResult>();

        var valido = Validator.TryValidateObject(dto, new ValidationContext(dto), erros, true);

        Assert.True(valido);
        Assert.Empty(erros);
    }

    private static CriarInstrumentoInputDto CriarDto(string json)
    {
        var dto = JsonSerializer.Deserialize<CriarInstrumentoInputDto>(json,
            new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
        dto.Codigo = "INST-01";
        dto.BarragemId = Guid.NewGuid();
        dto.LimiteAtencao = 10;
        dto.LimiteAlerta = 15;
        dto.LimiteEmergencia = 20;
        return dto;
    }
}
