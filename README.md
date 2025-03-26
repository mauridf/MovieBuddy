# MovieBuddy - Backend API (.NET 9)

O backend do MovieBuddy é uma API RESTful construída com .NET 9 (C#), responsável pelo gerenciamento de dados de filmes, usuários e recomendações.
Esse backend foi implementado para estudar o uso de APIs públicas e gratuitas, no caso foi usada a do TMDB (The Movie DataBase).

## Tecnologias Principais
- .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- Swagger/OpenAPI
- PostGreSQL

## Configuração do Ambiente

### 1. **Pré-requisitos**
   - .NET SDK 9.0
   - Visual Studio 2022 ou VS Code (com extensão C#)
   - PostgreSQL

### 2. **Instalação**
   ```bash
   dotnet restore
   ```

### 3. **Configuração do Banco de Dados**

- Crie um arquivo `appsettings.Development.json` baseado no `appsettings.json`.
- Configure a connection string no arquivo:

```json
"ConnectionStrings": {
    "PostgreSQL": "Server=localhost;Port=5432;Database=MovieBuddy;User Id=<User>;Password=<Password>;"
  }
```

### 4. **Aplicar Migrações**
```bash
   dotnet ef database update
```

### 5. **Executar a API**
```bash
   dotnet run
```
A API estará disponível em `https://localhost:7279` (porta pode variar).

---
## Estrutura do Projeto

```
src/
  MovieBuddy.API/          # Projeto principal
    Controllers/           # Controladores API
    Service/              # Lógica de negócio
    Models/                # Modelos de dados
    Data/                  # Contexto do EF e configurações
    DTOs/                  # Objetos de transferência de dados
    Mappings/              # Objetos de mapeamento das Models e DTOs           
    Program.cs             # Configuração principal
  MovieBuddy.Tests/        # Projeto de testes
```

---
## Configuração do Ambiente

Exemplo de `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "PostgreSQL": "Server=localhost;Port=5432;Database=MovieBuddy;User Id=<User>;Password=<Password>;"
  },
  "TheMovieDb": {
    "ApiKey": "<ApiKey>",
    "BaseUrl": "https://api.themoviedb.org/3/"
  },
  "AllowedHosts": "*"
}
```

---
## Documentação da API

A API utiliza Swagger (OpenAPI). Acesse:

```
https://localhost:7279/swagger
```

---
## Padrões de Desenvolvimento

- **Clean Architecture** - Separação clara entre camadas.
- **Repository Pattern** - Para acesso a dados.
- **DTOs** - Para transferência de dados entre camadas.
- **Dependency Injection** - Injeção de dependência nativa.

---
## Migrações

Para criar uma nova migração:
```bash
   dotnet ef migrations add NomeDaMigracao --project src/MovieBuddy.API
```

---
## Deploy

### Publicação
```bash
   dotnet publish -c Release -o ./publish
```
---
## Contribuição

1. Faça um fork do projeto.
2. Crie uma branch para sua feature:
   ```bash
   git checkout -b feature/NovaFuncionalidade
   ```
3. Commit suas mudanças:
   ```bash
   git commit -m 'Adiciona nova funcionalidade'
   ```
4. Push para a branch:
   ```bash
   git push origin feature/NovaFuncionalidade
   ```
5. Abra um Pull Request.