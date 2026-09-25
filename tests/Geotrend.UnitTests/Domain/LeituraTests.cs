using Geotrend.Domain.Entities;
using Geotrend.Domain.Enums;
using Xunit;

namespace Geotrend.UnitTests.Domain;

public class LeituraTests
{
    [Fact]
    public void CriarLeitura_DeveCalcularStatusComoEmergencia_QuandoValorAtingirLimiteEmergencia()
    {
        // Arrange (Preparação)
        var barragemId = Guid.NewGuid();
        var instrumento = new Instrumento(
            codigo: "PZ-01",
            tipo: TipoInstrumento.Piezometro,
            limiteAtencao: 10.0,
            limiteAlerta: 15.0,
            limiteEmergencia: 20.0,
            barragemId: barragemId
        );

        // Act (Ação)
        var leitura = new Leitura(instrumento.Id, valor: 22.0, instrumento);

        // Assert (Verificação)
        Assert.Equal(NivelAlerta.Emergencia, leitura.Status);
    }

    [Fact]
    public void CriarLeitura_DeveCalcularStatusComoNormal_QuandoValorAbaixoDoLimiteAtencao()
    {
        // Arrange (Preparação)
        var barragemId = Guid.NewGuid();
        var instrumento = new Instrumento(
            codigo: "PZ-01",
            tipo: TipoInstrumento.Piezometro,
            limiteAtencao: 10.0,
            limiteAlerta: 15.0,
            limiteEmergencia: 20.0,
            barragemId: barragemId
        );

        // Act (Ação)
        var leitura = new Leitura(instrumento.Id, valor: 5.0, instrumento);

        // Assert (Verificação)
        Assert.Equal(NivelAlerta.Normal, leitura.Status);
    }

    [Fact]
    public void CriarLeitura_DeveCalcularStatusComoAtencao_QuandoValorAtingirLimiteAtencao()
    {
        var barragemId = Guid.NewGuid();
        var instrumento = new Instrumento(
            codigo: "PZ-01",
            tipo: TipoInstrumento.Piezometro,
            limiteAtencao: 10.0,
            limiteAlerta: 15.0,
            limiteEmergencia: 20.0,
            barragemId: barragemId
        );
        
        var leitura = new Leitura(instrumento.Id, valor: 12.0, instrumento);
        
        Assert.Equal(NivelAlerta.Atencao, leitura.Status);
    }

    [Fact]
    public void CriarLeitura_DeveCalcularStatusComoAlerta_QuandoValorAtingirLimiteAlerta()
    {
        var barragemId = Guid.NewGuid();
        var instrumento = new Instrumento(
            codigo: "PZ-01",
            tipo: TipoInstrumento.Piezometro,
            limiteAtencao: 10.0,
            limiteAlerta: 15.0,
            limiteEmergencia: 20.0,
            barragemId: barragemId
        );
        
        var leitura = new Leitura(instrumento.Id, valor: 16.0, instrumento);
        
        Assert.Equal(NivelAlerta.Alerta, leitura.Status);
    }
}