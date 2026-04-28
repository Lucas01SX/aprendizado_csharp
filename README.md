# Estudo de C# — Projeto de Sprints

Projeto de estudo prático de C# com ASP.NET Core, organizado em sprints progressivas. O objetivo é aprender e aplicar os conceitos da linguagem de forma correta, eficiente e performática — desde fundamentos até padrões avançados.

## Tecnologias

- .NET 10 / ASP.NET Core 10 (Minimal API)
- C# com nullable reference types habilitado
- OpenAPI (Swagger) em desenvolvimento

## Como executar

```bash
# Restaurar dependências
dotnet restore

# Rodar em modo desenvolvimento
dotnet run --launch-profile http
# ou com HTTPS
dotnet run --launch-profile https

# Recarregar automaticamente ao salvar
dotnet watch run
```

A API sobe em `http://localhost:5125`. Em desenvolvimento, a documentação OpenAPI fica disponível em `/openapi/v1.json`.

## Sprints

### Sprint 1 — Setup inicial
Criação do projeto ASP.NET Core Minimal API com estrutura base.

### Sprint 2 — Modelagem de domínio
Definição da camada de domínio com interface genérica `IEntidade<TKey>` e classe base abstrata `EntidadeBase<TKey>`.

### Sprint 3 — Igualdade de objetos por Id
Implementação de igualdade de entidades baseada exclusivamente no `Id`, independentemente dos demais atributos. Implementa `IEquatable<T>` e sobrescreve `Equals` e `GetHashCode`.

### Sprint 4 — Coleções de alta performance e padrão Repositório
Introdução ao padrão Repository do DDD e uso de coleções otimizadas por hash. Interface `IRepositorioUsuario` definida no domínio; implementação `RepositorioUsuarioMemoria` na camada de infraestrutura usando `Dictionary<Guid, Usuario>` para busca O(1) por Id e `HashSet<Usuario>` para deduplicação automática baseada no `Equals`/`GetHashCode` da Sprint 3. Registro via injeção de dependência com `AddSingleton`.

### Sprint 5 — Programação funcional com Delegates, Func e Lambdas
Introdução a delegates genéricos do .NET (`Func<T, TResult>`) e expressões lambda. Adição do método `Filtrar(Func<Usuario, bool> filtro)` ao repositório, permitindo que qualquer critério de busca seja injetado pela camada superior sem alterar a infraestrutura. Implementação usando LINQ `.Where(filtro)` diretamente sobre os valores do dicionário.

### Sprint 6 — Transformação de dados com LINQ e DTOs
Introdução ao padrão DTO (Data Transfer Object) e projeções LINQ com `.Select()`. Criação de `UsuarioResponseDto` como `record` imutável na camada `dtos/`, expondo apenas `Nome` e `Email`. Endpoints passaram a retornar DTOs em vez de entidades de domínio, desacoplando o contrato da API da estrutura interna do domínio.

### Sprint 7 — Persistência com Entity Framework Core e SQLite
Substituição do repositório em memória por persistência real usando Entity Framework Core com SQLite. Criação de `AppDbContext` herdando de `DbContext` com primary constructor (C# 12) e `DbSet<Usuario>`. Implementação de `RepositorioUsuarioEfCore` substituindo `RepositorioUsuarioMemoria`, com registro via `AddScoped` (ciclo de vida por request, adequado para DbContext). Migrations geradas com `dotnet ef migrations add` criam o esquema do banco automaticamente a partir do modelo de domínio — sem SQL manual. Ponto de atenção: `Filtrar(Func<Usuario, bool>)` causa avaliação client-side (sem `WHERE` no SQL gerado); `Expression<Func<Usuario, bool>>` seria necessário para tradução para SQL — candidato à próxima sprint.

### Sprint 8 — Programação assíncrona com async/await
Migração do repositório para o modelo assíncrono do .NET. Todos os métodos de `IRepositorioUsuario` passaram a retornar `Task` ou `Task<T>`, e as implementações usam `async/await`. Os endpoints em `Program.cs` foram atualizados para `async` com `await` nas chamadas ao repositório. Ponto central da sprint: entender por que `Task<T>` não é o mesmo que `T` — retornar a `Task` sem `await` entrega uma promessa não resolvida ao chamador.

### Sprint 9 — Expression Trees e tradução para SQL
Substituição de `Func<Usuario, bool>` por `Expression<Func<Usuario, bool>>` no método `FiltrarAsync`. A diferença fundamental: `Func` é código compilado executado em memória (client-side evaluation); `Expression` é uma árvore de objetos que o EF Core consegue inspecionar e traduzir em cláusula `WHERE` no SQL gerado. A assinatura da interface no domínio não muda — apenas o tipo do parâmetro.

### Sprint 10 — Validação de entrada com DataAnnotations
Criação do DTO `CriarUsuarioDto` com validações declarativas via atributos (`[Required]`, `[EmailAddress]`, `[StringLength]`). O endpoint `POST /users` passou a usar `Validator.TryValidateObject` com `validateAllProperties: true` para acionar todas as regras antes de persistir. Respostas de erro seguem o formato padrão do ASP.NET Core (`Results.ValidationProblem`), que serializa os erros por campo.

### Sprint 11 — Testes unitários com MSTest 4 (TDD)
Introdução ao TDD com o framework MSTest 4. Claude mapeou os cenários e escreveu os arquivos de teste antes da implementação; o estudante implementou o código de produção para fazê-los passar. Cobertura: igualdade de entidades por `Id` (`UsuarioTests`), deduplicação do repositório em memória (`RepositorioUsuarioMemoriaTests`) e validação do DTO de criação (`CriarUsuarioDtoTests`). Conceitos aplicados: estrutura AAA (Arrange/Act/Assert), uma asserção por teste, `Assert.IsEmpty` / `IsNotEmpty` / `HasCount` do MSTest 4 em vez de `AreEqual` genérico.

### Sprint 12 — Autenticação JWT com TDD e variáveis de ambiente
Adição de autenticação JWT ao pipeline. Claude escreveu os testes do `TokenService` antes do código existir; o estudante implementou a classe usando `JwtSecurityTokenHandler`, `SecurityTokenDescriptor` e `HmacSha256Signature`. O segredo JWT é lido de um arquivo `.env` via pacote `DotNetEnv` — nunca do código-fonte. `TokenService` recebe o segredo via construtor para permitir testes com chave fixa, sem depender do ambiente. No `Program.cs`: `AddAuthentication(...).AddJwtBearer(...)` com `TokenValidationParameters`, `UseAuthentication()` antes de `UseAuthorization()`, e `.RequireAuthorization()` nos endpoints protegidos. Novo endpoint `POST /login` com credenciais mock retorna o token; acesso sem token retorna `401`.

> Sprints futuras serão adicionadas conforme o progresso do estudo.
