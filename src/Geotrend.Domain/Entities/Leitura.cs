using Geotrend.Domain.Enums;

namespace Geotrend.Domain.Entities;

public class Leitura
{
    public Guid Id { get; private set; }
    public Guid InstrumentoId { get; private set; }
    public DateTime DataHora { get; private set; }
    public double Valor { get; private set; }
    public NivelAlerta Status { get; private set; }

    private Leitura() { }

    public Leitura(Guid instrumentoId, double valor, Instrumento instrumento)
    {
        if (instrumento == null)
            throw new ArgumentNullException(nameof(instrumento), "O instrumento associado não pode ser nulo.");

        Id = Guid.NewGuid();
        InstrumentoId = instrumentoId;
        DataHora = DateTime.UtcNow;
        Valor = valor;
        
        // A própria leitura calcula seu status com base nos limites do instrumento
        Status = CalcularStatus(valor, instrumento);
    }
    
    // Compara o valor recebido com os limites configurados no Instrumentos.
    private static NivelAlerta CalcularStatus(double valor, Instrumento instrumento)
    {
        if (valor >= instrumento.LimiteEmergencia)
            return NivelAlerta.Emergencia;
            
        if (valor >= instrumento.LimiteAlerta)
            return NivelAlerta.Alerta;
            
        if (valor >= instrumento.LimiteAtencao)
            return NivelAlerta.Atencao;

        return NivelAlerta.Normal;
    }
}