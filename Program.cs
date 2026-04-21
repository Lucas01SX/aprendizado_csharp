using FinanceiroApi.Domain;
using FinanceiroApi.Infrastructure;
using FinanceiroApi.Dtos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IRepositorioUsuario, RepositorioUsuarioMemoria>();


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

app.MapGet("/users", () => {
    Guid guid = Guid.NewGuid();
    Usuario user = new Usuario{Id = guid ,Nome = "Lucas", Email = "user@gmail.com"};
    Usuario user2 = new Usuario{Id = guid, Nome = "João", Email = "Joao@gmail.com"};

    return user.Equals(user2);

}).WithName("/users");


app.MapGet("/users/processing", (IRepositorioUsuario repo) =>
{
    Guid id = Guid.NewGuid();
    Usuario user = new Usuario{Id=id, Nome = "Lucas", Email="teste@gmail.com"};
    Usuario user2 = new Usuario{Id=id, Nome = "Lucas Santos", Email="lucas@gmail.com"};
    Usuario user3 = new Usuario{Id=Guid.NewGuid(), Nome="Pedro", Email="pedro@gmail.com"};
    List<Usuario> users = new List<Usuario>{user, user2, user3};
    IReadOnlyCollection<Usuario> resultado = repo.ProcessarLista(users);

    return resultado;
}).WithName("/users/processing");


app.MapGet("/users/find", (IRepositorioUsuario repo) =>
{
    Guid id = Guid.NewGuid();
    Usuario user = new Usuario{Id=id, Nome = "Lucas", Email="teste@gmail.com"};
    Usuario user2 = new Usuario{Id=Guid.NewGuid(), Nome="Pedro", Email="pedro@gmail.com"};
    Func<Usuario, bool> filtro = u => u.Email.Contains("@gmail");
    repo.Adicionar(user);
    repo.Adicionar(user2);
    IReadOnlyCollection<Usuario> usuarios = repo.Filtrar(filtro);

    List<UsuarioResponseDto> resultado = usuarios.Select(u => new UsuarioResponseDto(u.Nome, u.Email))
        .ToList();

    return resultado;
}).WithName("/users/find");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
