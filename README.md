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

### Sprint 13 — Autorização por Roles (RBAC)
Evolução do pipeline de segurança para distinguir o que cada usuário pode fazer, não apenas quem ele é. `TokenService.GerarToken` passou a aceitar um parâmetro `role` (com valor padrão `"UsuarioComum"`) e inclui a claim de role no payload JWT. No `Program.cs`, `AddAuthorization` ganhou uma política nomeada `"SomenteAdmin"` que exige `Role = "Admin"`; `GET /users` e `POST /users` foram atualizados para usar essa política. O endpoint `POST /login` foi expandido para dois usuários mock com roles distintas. Novo endpoint `GET /me` injeta `ClaimsPrincipal` diretamente no handler — padrão suportado nativamente em Minimal APIs — e retorna email e role lidos do token. Ponto de atenção: `ClaimTypes.Role` é um URI longo que o `JwtSecurityTokenHandler` converte para o nome curto `"role"` ao serializar o token; ao ler o token com `ReadJwtToken` é necessário comparar contra `"role"`, não contra `ClaimTypes.Role`.

### Sprint 14 — Relacionamentos e Persistência Avançada com EF Core
Criação da entidade `Transacao` com relacionamento Um-para-Muitos com `Usuario`, configurado explicitamente via Fluent API no `OnModelCreating`. `HasMany().WithOne().HasForeignKey().OnDelete(Cascade)` define o relacionamento de forma declarativa e independente de convenções de nomes. `HasPrecision(18, 2)` aplicado à coluna `Valor` para evitar problemas de arredondamento em campos decimais. Novo interface `IRepositorioTransacao` e implementação `RepositorioTransacaoEfCore` seguindo o padrão estabelecido nas sprints anteriores. Endpoint `POST /transactions` injeta `ClaimsPrincipal` para identificar o usuário autenticado pelo email presente no token, busca o `Usuario` correspondente no banco e vincula a transação ao seu `UsuarioId`. Migration `AdicionaTransacao` aplicada para refletir o novo esquema no banco.

### Sprint 15 — Observabilidade e Resiliência
Preparação da API para produção com três pilares de observabilidade nativos do ASP.NET Core. **Tratamento global de exceções:** `AddProblemDetails()` + `UseExceptionHandler()` fazem qualquer erro não tratado retornar um objeto `ProblemDetails` no formato RFC 7807 — contrato público que qualquer cliente sabe interpretar. `UseExceptionHandler()` posicionado como primeiro middleware no pipeline para capturar exceções de qualquer camada. **Logging estruturado:** `ILogger<Program>` injetado nos endpoints de `/login` e `/transactions` usando named placeholders (`{Email}`, `{Role}`, `{TransacaoId}`) em vez de interpolação de string — a diferença é que named placeholders preservam os campos como dados estruturados, pesquisáveis em ferramentas como Seq ou Application Insights. **Health Check:** `AddHealthChecks().AddDbContextCheck<AppDbContext>()` registra uma verificação que executa uma query real no SQLite; `MapHealthChecks("/health")` expõe o resultado em `GET /health`, retornando `200 Healthy` ou `503 Unhealthy` — padrão esperado por orquestradores como Kubernetes e Azure App Service.

> Sprints futuras serão adicionadas conforme o progresso do estudo.
