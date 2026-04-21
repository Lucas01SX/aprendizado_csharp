using FinanceiroApi.Domain;
using FinanceiroApi.Infrastructure;
using FinanceiroApi.Dtos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=Financeiro.db"));
builder.Services.AddScoped<IRepositorioUsuario, RepositorioUsuarioEfCore>();


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


app.MapGet("/users/find", async (IRepositorioUsuario repo) =>
{
    Guid id = Guid.NewGuid();
    Usuario user = new Usuario{Id=id, Nome = "Lucas", Email="teste@gmail.com"};
    Usuario user2 = new Usuario{Id=Guid.NewGuid(), Nome="Pedro", Email="pedro@gmail.com"};
    Func<Usuario, bool> filtro = u => u.Email.Contains("@gmail");
    await repo.AdicionarAsync(user);
    await repo.AdicionarAsync(user2);
    IReadOnlyCollection<Usuario> usuarios = await repo.FiltrarAsync(filtro);

    List<UsuarioResponseDto> resultado = usuarios.Select(u => new UsuarioResponseDto(u.Nome, u.Email))
        .ToList();

    return resultado;
}).WithName("/users/find");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
