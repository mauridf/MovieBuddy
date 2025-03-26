# MovieBuddy - Backend API (.NET 9)

O backend do MovieBuddy � uma API RESTful constru�da com .NET 9 (C#), respons�vel pelo gerenciamento de dados de filmes, usu�rios e recomenda��es.
Esse backend foi implementado para estudar o uso de APIs p�blicas e gratuitas, no caso foi usada a do TMDB (The Movie DataBase).

## Tecnologias Principais
- .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- Swagger/OpenAPI
- PostGreSQL

## Configura��o do Ambiente

### 1. **Pr�-requisitos**
   - .NET SDK 9.0
   - Visual Studio 2022 ou VS Code (com extens�o C#)
   - PostgreSQL

### 2. **Instala��o**
   ```bash
   dotnet restore
   ```

### 3. **Configura��o do Banco de Dados**

- Crie um arquivo `appsettings.Development.json` baseado no `appsettings.json`.
- Configure a connection string no arquivo:

```json
"ConnectionStrings": {
    "PostgreSQL": "Server=localhost;Port=5432;Database=MovieBuddy;User Id=<User>;Password=<Password>;"
  }
```

### 4. **Aplicar Migra��es**
```bash
   dotnet ef database update
```

### 5. **Executar a API**
```bash
   dotnet run
```
A API estar� dispon�vel em `https://localhost:7279` (porta pode variar).

---
## Estrutura do Projeto

```
src/
  MovieBuddy.API/          # Projeto principal
    Controllers/           # Controladores API
    Service/              # L�gica de neg�cio
    Models/                # Modelos de dados
    Data/                  # Contexto do EF e configura��es
    DTOs/                  # Objetos de transfer�ncia de dados
    Mappings/              # Objetos de mapeamento das Models e DTOs           
    Program.cs             # Configura��o principal
  MovieBuddy.Tests/        # Projeto de testes
```

---
## Configura��o do Ambiente

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
## Documenta��o da API

A API utiliza Swagger (OpenAPI). Acesse:

```
https://localhost:7279/swagger
```

---
## Padr�es de Desenvolvimento

- **Clean Architecture** - Separa��o clara entre camadas.
- **Repository Pattern** - Para acesso a dados.
- **DTOs** - Para transfer�ncia de dados entre camadas.
- **Dependency Injection** - Inje��o de depend�ncia nativa.

---
## Migra��es

Para criar uma nova migra��o:
```bash
   dotnet ef migrations add NomeDaMigracao --project src/MovieBuddy.API
```

---
## Deploy

### Publica��o
```bash
   dotnet publish -c Release -o ./publish
```
---
## Contribui��o

1. Fa�a um fork do projeto.
2. Crie uma branch para sua feature:
   ```bash
   git checkout -b feature/NovaFuncionalidade
   ```
3. Commit suas mudan�as:
   ```bash
   git commit -m 'Adiciona nova funcionalidade'
   ```
4. Push para a branch:
   ```bash
   git push origin feature/NovaFuncionalidade
   ```
5. Abra um Pull Request.