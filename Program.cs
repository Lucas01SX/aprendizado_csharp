using FinanceiroApi.Domain;
using FinanceiroApi.Infrastructure;
using FinanceiroApi.Dtos;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=Financeiro.db"));
builder.Services.AddScoped<IRepositorioUsuario, RepositorioUsuarioEfCore>();
builder.Services.AddValidation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

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
}).WithName("GetUsers");

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
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
