using FinanceiroApi.Domain;
using FinanceiroApi.Infrastructure;
using FinanceiroApi.Dtos;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

//// VARIÁVEIS DE AMBIENTE
DotNetEnv.Env.Load();
string jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")!;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=Financeiro.db"));
builder.Services.AddScoped<IRepositorioUsuario, RepositorioUsuarioEfCore>();
builder.Services.AddScoped<IRepositorioTransacao, RepositorioTransacaoEfCore>();
builder.Services.AddValidation();
builder.Services.AddSingleton(new TokenService(jwtSecret));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization(opt =>
    opt.AddPolicy("SomenteAdmin", p => p.RequireRole("Admin"))
);



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");


app.MapGet("/", () => "Hello World!");

app.MapGet("/users", async (IRepositorioUsuario repo) =>
{
    IReadOnlyCollection<Usuario> usuarios = await repo.FiltrarAsync(u => true);
    List<UsuarioResponseDto> resultado = usuarios.Select(u => new UsuarioResponseDto(u.Nome, u.Email)).ToList();
    return resultado;
}).WithName("GetUsers").RequireAuthorization("SomenteAdmin");

app.MapPost("/users", async (CriarUsuarioDto createDto, IRepositorioUsuario repo) => {

    ValidationContext validationContext = new ValidationContext(createDto);
    List<ValidationResult> validationResults = new List<ValidationResult>();

    bool isValid = Validator.TryValidateObject(createDto, validationContext, validationResults, true);

    if(!isValid)
    {
        return Results.ValidationProblem(
            validationResults.ToDictionary(
                r => r.MemberNames.FirstOrDefault() ?? string.Empty,
                r => new[] { r.ErrorMessage ?? string.Empty }
            )
        );
    }

    Usuario usuario = new Usuario
    {
        Id = Guid.NewGuid(),
        Nome = createDto.Nome,
        Email = createDto.Email
    };

    await repo.AdicionarAsync(usuario);

    return Results.Created($"/users/{usuario.Id}", new UsuarioResponseDto(usuario.Nome, usuario.Email));
}).RequireAuthorization("SomenteAdmin");

app.MapPost("/login", (LoginDto dto, TokenService tokenService) =>
{
    if (dto.Senha != "senha123") return Results.Unauthorized();

    string? role = dto.Email switch
    {
        "admin@financeiro.com" => "Admin",
        "user@financeiro.com"  => "UsuarioComum",
        _                      => null
    };

    if (role is null) return Results.Unauthorized();

    string token = tokenService.GerarToken(dto.Email, role);
    return Results.Ok(new TokenResponseDto(token));
});

app.MapGet("/me", (ClaimsPrincipal user) =>
{
    string email = user.FindFirst(ClaimTypes.Email)!.Value;
    string role  = user.FindFirst(ClaimTypes.Role)!.Value;
    return Results.Ok(new UsuarioLogadoDto(email, role));
}).RequireAuthorization();



app.MapPost("/transactions", async (
    CriarTransacaoDto dto,
    ClaimsPrincipal user,
    IRepositorioUsuario repoUsuario,
    IRepositorioTransacao repoTransacao) =>
{
    string email = user.FindFirst(ClaimTypes.Email)!.Value;
    IReadOnlyCollection<Usuario> encontrados = await repoUsuario.FiltrarAsync(u => u.Email == email);
    Usuario? usuario = encontrados.FirstOrDefault();

    if (usuario is null) return Results.NotFound();

    Transacao transacao = new Transacao
    {
        Id        = Guid.NewGuid(),
        Descricao = dto.Descricao,
        Valor     = dto.Valor,
        Data      = dto.Data,
        UsuarioId = usuario.Id
    };

    await repoTransacao.AdicionarAsync(transacao);
    return Results.Created(
        $"/transactions/{transacao.Id}",
        new TransacaoResponseDto(transacao.Id, transacao.Descricao, transacao.Valor, transacao.Data)
    );
}).RequireAuthorization();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
