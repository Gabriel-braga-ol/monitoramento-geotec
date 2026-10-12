# Geotrend — Sistema de Monitoramento Geotécnico de Barragens

O **Geotrend** é uma aplicação web para acompanhamento e monitoramento do nível de risco e leitura de instrumentos geotécnicos em barragens de rejeito e reservatórios (como piezômetros, inclinômetros, marcos topográficos e medidores de vazão).

O objetivo do projeto é consolidar dados do banco de dados em um painel simples e direto, permitindo identificar rapidamente o status de segurança de cada estrutura (Normal, Atenção, Alerta ou Emergência).

---

## Tecnologias Utilizadas

### Frontend
- **React 19** com **TypeScript**
- **Vite** (Build tool e servidor de desenvolvimento)
- **Tailwind CSS** (Estilização da interface)
- **Lucide React** (Ícones da interface)
- **Axios** (Consumo da API REST)

### Backend
- **.NET 10 (ASP.NET Core Web API)**
- **C#**
- **Entity Framework Core 10** (ORM)
- **Npgsql / PostgreSQL** (Banco de dados relacional)

---

## Funcionalidades Implementadas

- **Visualização de Status Global:** Apresenta o nível de risco consolidado da barragem selecionada com base nas leituras dos seus instrumentos.
- **Contadores por Nível de Risco:** Exibe a contagem de instrumentos em status *Normal*, *Atenção* e *Alerta/Emergência*.
- **Tabela de Instrumentos Geotécnicos:** Lista os instrumentos cadastrados na estrutura com código, tipo, valor da última leitura e badge visual indicando o status.
- **Seleção de Barragens:** Permite alternar a visualização do painel entre diferentes barragens cadastradas no banco de dados.
- **Atualização Manual:** Botão para recarregar os dados do painel consumindo a API.

---

## Como Executar

### Pré-requisitos

- SDK .NET 10.
- PostgreSQL em execução, com usuário e senha configurados.
- Node.js 24 e npm para o frontend.
- Git.

### 1. Configurar e Executar o Backend

1. Clone o repositório:

   ```bash
   git clone https://github.com/Gabriel-braga-ol/monitoramento-geotec.git
   cd monitoramento-geotec
   ```

2. Configure a conexão local com o PostgreSQL usando os User Secrets do projeto da API. Substitua `sua_senha` pela senha do seu usuário e ajuste os demais campos conforme seu ambiente:

   ```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=geotrend_db;Username=postgres;Password=sua_senha" --project src/Geotrend.API
   ```

   O projeto já possui um `UserSecretsId`. Essa configuração é carregada no ambiente `Development` e mantém a senha fora dos arquivos versionados. O `appsettings.json` contém somente a configuração base, sem senha.

3. Restaure as dependências e instale a ferramenta do Entity Framework Core, caso ainda não tenha uma versão 10 instalada:

   ```bash
   dotnet restore
   dotnet tool install --global dotnet-ef --version 10.0.12
   ```

   Se precisar ajustar uma instalação global existente, use `dotnet tool update --global dotnet-ef --version 10.0.12`.

4. Na raiz do repositório, aplique as migrations existentes:

   ```bash
   dotnet ef database update --project src/Geotrend.Infrastructure --startup-project src/Geotrend.API -- --environment Development
   ```

   Esse comando cria as tabelas e, se necessário, o banco de dados, desde que o usuário configurado tenha permissão para criá-lo. Caso contrário, crie previamente o banco `geotrend_db` com um usuário autorizado.

   As migrations não inserem dados de exemplo. O repositório não possui script SQL de carga; o cadastro inicial é descrito abaixo.

5. Execute a API com o perfil HTTP:

   ```bash
   dotnet run --project src/Geotrend.API --launch-profile http
   ```

   A API ficará disponível em `http://localhost:5196`, e o Swagger em `http://localhost:5196/swagger`.

### 2. Cadastrar Dados para Visualizar o Painel

Com a API em execução, abra o Swagger e use **Try it out** e **Execute** nos endpoints abaixo, nesta ordem. Os nomes e limites são fictícios, apenas para demonstrar o fluxo.

1. Em `POST /api/Barragem`, cadastre uma barragem:

   ```json
   {
     "nome": "Barragem de demonstração",
     "localizacao": "Local fictício"
   }
   ```

   Copie o `id` retornado na resposta.

2. Em `POST /api/Instrumento`, cadastre um instrumento, substituindo `ID_DA_BARRAGEM` pelo ID obtido:

   ```json
   {
     "codigo": "PZ-01",
     "tipo": 1,
     "limiteAtencao": 10,
     "limiteAlerta": 15,
     "limiteEmergencia": 20,
     "barragemId": "ID_DA_BARRAGEM"
   }
   ```

   O tipo `1` corresponde a um piezômetro. Copie o `id` do instrumento retornado na resposta.

3. Em `POST /api/Leitura`, registre uma leitura, substituindo `ID_DO_INSTRUMENTO`:

   ```json
   {
     "instrumentoId": "ID_DO_INSTRUMENTO",
     "valor": 12
   }
   ```

   Com esses limites, o status retornado será `Atencao`. O painel deverá mostrar um instrumento em atenção na barragem cadastrada.

### 3. Configurar e Executar o Frontend

1. Abra um novo terminal na raiz do repositório e navegue até a pasta do frontend:

   ```bash
   cd geotrend-web
   ```

2. Instale as dependências nas versões registradas no lockfile:

   ```bash
   npm ci
   ```

3. Inicie o servidor de desenvolvimento:

   ```bash
   npm run dev -- --port 5173 --strictPort
   ```

4. Acesse `http://localhost:5173`. Se o painel já estava aberto antes dos cadastros, recarregue a página para buscar a lista de barragens novamente.

O frontend está configurado em `src/services/api.ts` para acessar `http://localhost:5196/api`. A política CORS da API permite a origem `http://localhost:5173`; por isso, mantenha esses endereços ao executar localmente. Se a porta 5173 estiver ocupada, libere-a antes de iniciar o frontend.

### 4. Executar os Testes

Na raiz do repositório, execute os testes de unidade do backend:

```bash
dotnet test
```

Na pasta `geotrend-web`, execute o build e a análise estática do frontend:

```bash
npm run build
npm run lint
```

## 📁 Estrutura do Projeto

O repositório é organizado no modelo de monorepo simplificado:

```text
monitoramento-geotec/
├── geotrend-web/             # Aplicação Frontend (React + Vite)
│   ├── src/
│   │   ├── assets/           # Imagens e recursos estáticos
│   │   ├── components/
│   │   │   └── StatusBadge.tsx # Componente visual para exibição de status
│   │   ├── services/         # Configuração da API (axios)
│   │   ├── App.tsx           # Dashboard principal
│   │   └── main.tsx          # Ponto de entrada do React
│   ├── package.json
│   └── vite.config.ts
│
├── src/                      # Aplicação Backend (.NET Clean Architecture)
│   ├── Geotrend.API/         # Controllers e Program.cs (Ponto de entrada)
│   ├── Geotrend.Application/ # DTOs, Interfaces e Serviços
│   ├── Geotrend.Domain/      # Entidades do domínio (Barragem, Instrumento, Leitura) e Enums
│   └── Geotrend.Infrastructure/ # DbContext, migrations e repositórios com EF Core
├── tests/
│   └── Geotrend.UnitTests/   # Testes de unidade do backend
└── Geotrend.slnx            # Solução .NET
```
---
## Aprendizados e objetivos do projetos

Este projeto foi desenvolvido para praticar e consolidar conceitos de desenvolvimento Full Stack:

- Organização de arquitetura em camadas utilizando C# e .NET.

- Mapeamento relacional e carregamento de entidades com Entity Framework Core e PostgreSQL.

- Comunicação entre frontend e backend via API REST com TypeScript e Axios.

- Construção de interface reativa e estilização estruturada com React e Tailwind CSS.
