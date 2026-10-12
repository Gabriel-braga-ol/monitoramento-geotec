using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Geotrend.Application.DTOs;
using Geotrend.Application.Services;
using Geotrend.Domain.Entities;
using Geotrend.Domain.Enums;
using Geotrend.Domain.Interfaces;

namespace Geotrend.UnitTests.Application;

public class LeituraServiceTests
{
    [Theory]
    [InlineData("{}")]
    [InlineData("{\"valor\":null}")]
    public void ValidarDto_ValorAusenteOuNulo_DeveRejeitar(string json)
    {
        var dto = JsonSerializer.Deserialize<RegistrarLeituraInputDto>(json,
            new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
        var erros = new List<ValidationResult>();

        var valido = Validator.TryValidateObject(dto, new ValidationContext(dto), erros, true);

        Assert.False(valido);
        Assert.Contains(erros, erro => erro.MemberNames.Contains(nameof(dto.Valor)));
    }

    [Fact]
    public void ValidarDto_ZeroInformado_DeveAceitar()
    {
        var dto = JsonSerializer.Deserialize<RegistrarLeituraInputDto>("{\"valor\":0}",
            new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
        var erros = new List<ValidationResult>();

        var valido = Validator.TryValidateObject(dto, new ValidationContext(dto), erros, true);

        Assert.True(valido);
        Assert.Empty(erros);
        Assert.Equal(0d, dto.Valor);
    }

    [Fact]
    public async Task RegistrarLeitura_SemValor_NaoDeveConsultarNemSalvar()
    {
        var instrumentoRepository = new InstrumentoRepositoryStub();
        var leituraRepository = new LeituraRepositorySpy();
        var service = new LeituraService(instrumentoRepository, leituraRepository);
        var dto = new RegistrarLeituraInputDto { InstrumentoId = instrumentoRepository.Instrumento.Id };

        await Assert.ThrowsAsync<ArgumentException>(() => service.RegistrarLeituraAsync(dto));

        Assert.Equal(0, instrumentoRepository.Consultas);
        Assert.Empty(leituraRepository.Salvas);
    }

    [Theory]
    [InlineData(0, "Normal")]
    [InlineData(12, "Atencao")]
    public async Task RegistrarLeitura_ComValor_DevePersistirValorInformado(double valor, string status)
    {
        var instrumentoRepository = new InstrumentoRepositoryStub();
        var leituraRepository = new LeituraRepositorySpy();
        var service = new LeituraService(instrumentoRepository, leituraRepository);
        var dto = new RegistrarLeituraInputDto
        {
            InstrumentoId = instrumentoRepository.Instrumento.Id,
            Valor = valor
        };

        var resultado = await service.RegistrarLeituraAsync(dto);

        var salva = Assert.Single(leituraRepository.Salvas);
        Assert.Equal(valor, salva.Valor);
        Assert.Equal(valor, resultado.Valor);
        Assert.Equal(status, resultado.Status);
        Assert.Equal(dto.InstrumentoId, salva.InstrumentoId);
    }

    private sealed class InstrumentoRepositoryStub : IInstrumentoRepository
    {
        public Instrumento Instrumento { get; } = new("PZ-01", TipoInstrumento.Piezometro,
            10, 15, 20, Guid.NewGuid());
        public int Consultas { get; private set; }

        public Task<Instrumento?> ObterPorIdAsync(Guid id)
        {
            Consultas++;
            return Task.FromResult<Instrumento?>(id == Instrumento.Id ? Instrumento : null);
        }

        public Task<IEnumerable<Instrumento>> ObterPorBarragemIdAsync(Guid barragemId)
            => throw new NotSupportedException();
        public Task AdicionarAsync(Instrumento instrumento) => throw new NotSupportedException();
    }

    private sealed class LeituraRepositorySpy : ILeituraRepository
    {
        public List<Leitura> Salvas { get; } = new();

        public Task AdicionarAsync(Leitura leitura)
        {
            Salvas.Add(leitura);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Leitura>> ObterUltimasLeiturasPorInstrumentoAsync(
            Guid instrumentoId, DateTime? dataInicio, DateTime? dataFim, int pagina, int tamanhoPagina)
            => throw new NotSupportedException();
    }
}
