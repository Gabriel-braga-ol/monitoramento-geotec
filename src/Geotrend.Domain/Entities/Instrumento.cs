using Geotrend.Domain.Enums;

namespace Geotrend.Domain.Entities;

public class Instrumento
{
    public Guid Id { get; private set; }
    public string Codigo { get; private set; } = string.Empty;
    public TipoInstrumento Tipo { get; private set; }
    
    public double LimiteAtencao  { get; private set; }
    public double LimiteAlerta { get; private set; }
    public double LimiteEmergencia { get; private set; }
    
    public Guid BarragemId { get; private set; }
    
    public IReadOnlyCollection<Leitura> Leituras => _leitura.AsReadOnly();
    
    // guadra as medições feitas pelos instrumentos
    private readonly List<Leitura> _leitura = new(); 
    
    private Instrumento() { }

    public Instrumento(string codigo, TipoInstrumento tipo, 
        double limiteAtencao, double limiteAlerta, double limiteEmergencia, Guid barragemId)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("O código do instrumento é obrigatório.", nameof(codigo));

        if (limiteAtencao >= limiteAlerta || limiteAlerta >= limiteEmergencia)
            throw new ArgumentException(
                "Os limites do instrumento devem seguir a ordem crescente: Atenção < Alerta < Emergência.");
        
        Id = Guid.NewGuid();
        Codigo = codigo;
        Tipo = tipo;
        LimiteAtencao = limiteAtencao;
        LimiteAlerta = limiteAlerta;
        LimiteEmergencia = limiteEmergencia;
        BarragemId = barragemId;
    }
}