using System.Reflection;
using Geotrend.Application.Services;
using Geotrend.Domain.Entities;
using Geotrend.Domain.Enums;
using Geotrend.Domain.Interfaces;

namespace Geotrend.UnitTests.Application;

public class BarragemServiceTests
{
    [Fact]
    public async Task ObterStatusResumo_SemInstrumentos_DeveRetornarSemDados()
    {
        var barragem = CriarBarragem();
        var service = CriarService(barragem);

        var resumo = await service.ObterStatusResumoAsync(barragem.Id);

        Assert.NotNull(resumo);
        Assert.Equal("SemDados", resumo.StatusGlobal);
        Assert.Equal(0, resumo.TotalInstrumentos);
        Assert.Equal(0, resumo.QuantidadeNormal);
        Assert.Equal(0, resumo.QuantidadeSemLeitura);
        Assert.Empty(resumo.Instrumentos);
    }

    [Fact]
    public async Task ObterStatusResumo_SemLeituras_NaoDeveContarInstrumentosComoNormais()
    {
        var barragem = CriarBarragem(null, null);
        var service = CriarService(barragem);

        var resumo = await service.ObterStatusResumoAsync(barragem.Id);

        Assert.NotNull(resumo);
        Assert.Equal("SemDados", resumo.StatusGlobal);
        Assert.Equal(2, resumo.TotalInstrumentos);
        Assert.Equal(2, resumo.QuantidadeSemLeitura);
        Assert.Equal(0, resumo.QuantidadeNormal);
        Assert.All(resumo.Instrumentos, instrumento =>
        {
            Assert.Equal("SemLeitura", instrumento.UltimoStatus);
            Assert.Null(instrumento.UltimoValor);
            Assert.Null(instrumento.DataUltimaLeitura);
        });
    }

    [Theory]
    [InlineData(0, "Normal", 1, 0, 0, 0)]
    [InlineData(10, "Atencao", 0, 1, 0, 0)]
    [InlineData(15, "Alerta", 0, 0, 1, 0)]
    [InlineData(20, "Emergencia", 0, 0, 0, 1)]
    public async Task ObterStatusResumo_ComDadosParciais_DevePreservarNivelDisponivel(
        double valor, string status, int normal, int atencao, int alerta, int emergencia)
    {
        var barragem = CriarBarragem(null, valor);
        var service = CriarService(barragem);

        var resumo = await service.ObterStatusResumoAsync(barragem.Id);

        Assert.NotNull(resumo);
        Assert.Equal(status, resumo.StatusGlobal);
        Assert.Equal(2, resumo.TotalInstrumentos);
        Assert.Equal(1, resumo.QuantidadeSemLeitura);
        Assert.Equal(normal, resumo.QuantidadeNormal);
        Assert.Equal(atencao, resumo.QuantidadeAtencao);
        Assert.Equal(alerta, resumo.QuantidadeAlerta);
        Assert.Equal(emergencia, resumo.QuantidadeEmergencia);
        var comLeitura = Assert.Single(resumo.Instrumentos.Where(i => i.UltimoValor.HasValue));
        Assert.Equal(valor, comLeitura.UltimoValor);
        Assert.Equal(status, comLeitura.UltimoStatus);
        Assert.NotNull(comLeitura.DataUltimaLeitura);
    }

    [Fact]
    public async Task ObterStatusResumo_ComVariosNiveis_DeveManterPiorNivelEContagens()
    {
        var barragem = CriarBarragem(20, 0, null, 15, 10);
        var service = CriarService(barragem);

        var resumo = await service.ObterStatusResumoAsync(barragem.Id);

        Assert.NotNull(resumo);
        Assert.Equal("Emergencia", resumo.StatusGlobal);
        Assert.Equal(5, resumo.TotalInstrumentos);
        Assert.Equal(1, resumo.QuantidadeNormal);
        Assert.Equal(1, resumo.QuantidadeAtencao);
        Assert.Equal(1, resumo.QuantidadeAlerta);
        Assert.Equal(1, resumo.QuantidadeEmergencia);
        Assert.Equal(1, resumo.QuantidadeSemLeitura);
    }

    [Fact]
    public async Task ObterStatusResumo_ComTodosOsInstrumentosLidos_DeveTerZeroSemLeitura()
    {
        var barragem = CriarBarragem(0, 10);
        var service = CriarService(barragem);

        var resumo = await service.ObterStatusResumoAsync(barragem.Id);

        Assert.NotNull(resumo);
        Assert.Equal("Atencao", resumo.StatusGlobal);
        Assert.Equal(0, resumo.QuantidadeSemLeitura);
        Assert.Equal(1, resumo.QuantidadeNormal);
        Assert.Equal(1, resumo.QuantidadeAtencao);
    }

    [Fact]
    public async Task ObterStatusResumo_BarragemInexistente_DeveRetornarNull()
    {
        var service = CriarService(CriarBarragem());

        var resumo = await service.ObterStatusResumoAsync(Guid.NewGuid());

        Assert.Null(resumo);
    }

    private static BarragemService CriarService(Barragem barragem)
    {
        var repository = new ResumoRepositoryStub(barragem);
        return new BarragemService(repository, repository);
    }

    private static Barragem CriarBarragem(params double?[] valores)
    {
        var barragem = new Barragem("Teste", "Local de teste");
        for (int i = 0; i < valores.Length; i++)
        {
            var instrumento = new Instrumento($"PZ-{i}", TipoInstrumento.Piezometro,
                10, 15, 20, barragem.Id);
            if (valores[i] is double valor)
                ObterColecao<Leitura>(instrumento, "_leituras").Add(instrumento.AdicionarLeitura(valor));
            ObterColecao<Instrumento>(barragem, "_instrumentos").Add(instrumento);
        }
        return barragem;
    }

    // Monta o grafo que o repositório entregaria após o carregamento pelo EF Core.
    // A reflexão fica restrita à preparação dos testes, preservando a API do domínio.
    private static List<T> ObterColecao<T>(object entidade, string campo)
        => (List<T>)entidade.GetType()
            .GetField(campo, BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(entidade)!;

    private sealed class ResumoRepositoryStub(Barragem barragem)
        : IBarragemRepository, IInstrumentoRepository
    {
        public Task<Barragem?> ObterPorIdAsync(Guid id)
            => Task.FromResult<Barragem?>(id == barragem.Id ? barragem : null);

        public Task<IEnumerable<Barragem>> ObterTodosAsync() => throw new NotSupportedException();
        public Task AdicionarAsync(Barragem entidade) => throw new NotSupportedException();
        public Task AtualizarAsync(Barragem entidade) => throw new NotSupportedException();
        Task<Instrumento?> IInstrumentoRepository.ObterPorIdAsync(Guid id)
            => throw new NotSupportedException();
        public Task<IEnumerable<Instrumento>> ObterPorBarragemIdAsync(Guid id)
            => throw new NotSupportedException();
        public Task AdicionarAsync(Instrumento instrumento) => throw new NotSupportedException();
    }
}