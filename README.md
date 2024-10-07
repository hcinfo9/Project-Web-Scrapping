# Projeto WebScrapping

Projeto consiste em um **sistema de scraping de dados alimentares** que coleta informações de uma fonte online e armazena esses dados em um banco de dados PostgreSQL. A aplicação é composta por vários componentes, incluindo serviços de scraping, repositórios de dados, configuração de banco de dados e testes unitários.



## Tecnologias Utilizadas


- **[.NET](https://dotnet.microsoft.com/pt-br/)**: utilizado para integrar o servidor com o PostgreSQL e migrações automáticas e suporte a consultas complexas, além de ser otimizado para desempenho.
- **[C#](https://learn.microsoft.com/pt-br/dotnet/csharp/)**: Utilizado para criar o Servidor, comunicação com banco de dados e as rotas da API.
- **[HtmlAgilityPack](https://html-agility-pack.net/)**: Utilizado para criar o Servidor, comunicação com banco de dados e as rotas da API.
- **[PostGreSQL](https://www.postgresql.org/)**: Banco de dados utilizado para organizar e manter nossas metas guardadas.
- **[Docker](https://www.docker.com/)**: Utilizado para criar uma imagem Postgres, assim permitindo executar o banco de dados mesmo sem instala-lo na maquina.
- **[XUnit](https://xunit.net/)**: Utilizado para criar uma imagem Postgres, assim permitindo executar o banco de dados mesmo sem instala-lo na maquina.



## Funcionalidades 

- Extrair Dados de uma pagina HTML com HTMLAgilityPack.
- Salvar Dados extraidos no PostGreSQL.

  

## Pré-requisitos

- **.NET SDK**: [Instalar .NET](https://dotnet.microsoft.com/pt-br/download/visual-studio-sdks)
- **Visual Studio Code**: [Instalar VsCODE](https://code.visualstudio.com/)
- **Docker**:[Instalar Docker](https://www.docker.com/products/docker-desktop/)


## Instalação 

Siga as etapas para instalar e executar o projeto localmente:

1. Clone o repositório:

   ```bash
   https://github.com/hcinfo9/Project-Web-Scrapping.git
   ```

2. Acesse o diretório do projeto:
   
   ```bash
   cd Project-Web-Scrapping
   ```

3. Instale as dependências:
    
   ```bash
   dotnet restore
   ```

4. Crie e inicie o container docker rodando a imagem do PostgreSQL:

    ```bash
   docker-compose up
   ```

5. Execute o Program.cs para realizar o Scrapping de dados:

   ```bash
   dotnet run
   ```



6. Para testar a aplicação:

   ```bash
   dotnet test
   ```

## Scripts Disponíveis

## BackEnd
- **`dotnet run`**: Executa o arquivo principal para fazer o Scrapping dos dados.
- **`dotnet test`**: Executa o arquivo de testes Unitarios da Aplicação.




## Estrutura de Pastas

A estrutura principal do projeto é a seguinte:

```bash
# BackEnd
WEBSCRAPER/
├── src/
│   ├── config/
│   │   └── DatabaseConfig.cs
│   ├── models/
│   │   ├── FoodComponent.cs
│   │   └── FoodData.cs
│   ├── repositories/
│   │   └── FoodRepository.cs
│   ├── services/
│   │   ├── FoodService.cs
│   │   └── ScrapingService.cs
│   └── tests/
|        └── FoodServiceTests.cs
|
├── .gitignore
├── docker-compose.yml
├── Program.cs
├── WebScraper.csproj
├── WebScraper.sln   
└── README.md

```


## Autor

Este projeto foi desenvolvido por Henrique Donato como parte de um desafio pra aprimorar minhas habilidades.

