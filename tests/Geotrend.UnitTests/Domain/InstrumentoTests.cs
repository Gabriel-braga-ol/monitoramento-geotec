using Geotrend.Domain.Entities;
using Geotrend.Domain.Enums;

namespace Geotrend.UnitTests.Domain;

public class InstrumentoTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(5)]
    [InlineData(999)]
    public void CriarInstrumento_TipoInvalido_DeveLancarExcecao(int tipo)
    {
        var erro = Assert.Throws<ArgumentException>(() => new Instrumento(
            "PZ-01", (TipoInstrumento)tipo, 10, 15, 20, Guid.NewGuid()));

        Assert.Equal("tipo", erro.ParamName);
    }

    [Theory]
    [InlineData(TipoInstrumento.Piezometro)]
    [InlineData(TipoInstrumento.Inclinometro)]
    [InlineData(TipoInstrumento.MarcoTopografico)]
    [InlineData(TipoInstrumento.MedidorVazao)]
    public void CriarInstrumento_TipoValido_DevePreservarTipo(TipoInstrumento tipo)
    {
        var instrumento = new Instrumento("INST-01", tipo, 10, 15, 20, Guid.NewGuid());

        Assert.Equal(tipo, instrumento.Tipo);
    }
}

