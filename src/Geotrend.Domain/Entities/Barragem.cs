using Geotrend.Domain.Enums;

namespace Geotrend.Domain.Entities;

public class Barragem
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Localizacao { get; private set; } = string.Empty;
    public NivelAlerta NivelAlertaAtual { get; private set; } // garuda o estado atual de risco da barragem

    public IReadOnlyCollection<Instrumento> Instrumentos => _instrumentos.AsReadOnly();
    private readonly List<Instrumento> _instrumentos = new(); 
    
    private Barragem() { }

    public Barragem(string nome, string localizacao)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome da barragem não pode ser vazio.", nameof(nome));
        
        Id = Guid.NewGuid();
        Nome = nome;
        Localizacao = localizacao;
        // nível de alerta padrão
        NivelAlertaAtual = NivelAlerta.Normal;
    }
    
    // Altera o nível de alerta da barargem
    public void AtualizarNivelAlerta(NivelAlerta novoNivel)
    {
        NivelAlertaAtual = novoNivel;
    }
}