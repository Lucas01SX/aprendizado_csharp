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

> Sprints futuras serão adicionadas conforme o progresso do estudo.
