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

### 1. Configurar e Executar o Backend

1. Clone o repositório:
   git clone https://github.com/Gabriel-braga-ol/monitoramento-geotec.git
   cd Geotrend

2. Certifique-se de que o banco de dados PostgreSQL esteja em execução e configure a string de conexão no arquivo src/Geotrend.API/appsettings.json:
   {
      "ConnectionStrings": {
         "DefaultConnection": "Host=localhost;Database=geotrend_db;Username=postgres;Password=sua_senha"
      }
   }

3. Execute o script SQL de carga contido no projeto para criar e popular as tabelas no banco de dados.

4. Execute a API:
   dotnet run --project src/Geotrend.API

   A API iniciará na porta configurada, geralmente http://localhost:5000 ou https://localhost:7001.

### 2. Configurar e Executar o Frontend

1. Abra um novo terminal e navegue até a pasta do frontend:
   cd geotrend-web

2. Instale as dependências do projeto:
   npm install

3. Inicie o servidor de desenvolvimento:
   npm run dev

4. Acesse o endereço informado no terminal, normalmente http://localhost:5173.

## 📁 Estrutura do Projeto

O repositório é organizado no modelo de monorepo simplificado:

```text
Geotrend/
├── geotrend-web/             # Aplicação Frontend (React + Vite)
│   ├── src/
│   │   ├── assets/           # Imagens e recursos estáticos
│   │   ├── services/         # Configuração da API (axios)
│   │   ├── App.tsx           # Dashboard principal
│   │   ├── StatusBadge.tsx   # Componente visual para exibições de status
│   │   └── main.tsx          # Ponto de entrada do React
│   ├── package.json
│   └── vite.config.ts
│
└── src/                      # Aplicação Backend (.NET Clean Architecture)
    ├── Geotrend.API/         # Controllers e Program.cs (Ponto de entrada)
    ├── Geotrend.Application/ # DTOs, Interfaces e Serviços
    ├── Geotrend.Domain/      # Entidades do domínio (Barragem, Instrumento, Leitura) e Enums
    └── Geotrend.Infrastructure/ # DbContext e Repositórios com EF Core
```
---
## Aprendizados e objetivos do projetos

Este projeto foi desenvolvido para praticar e consolidar conceitos de desenvolvimento Full Stack:

- Organização de arquitetura em camadas utilizando C# e .NET.

- Mapeamento relacional e carregamento de entidades com Entity Framework Core e PostgreSQL.

- Comunicação entre frontend e backend via API REST com TypeScript e Axios.

- Construção de interface reativa e estilização estruturada com React e Tailwind CSS.