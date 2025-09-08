using PokemonAPI.Application.Services;
using PokemonAPI.Domain.Interfaces;
using PokemonAPI.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Adicionando servi�os ao cont�iner
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Configuração DDD - Repository Pattern
builder.Services.AddHttpClient<PokemonRepository>();
builder.Services.AddScoped<IPokemonRepository, PokemonRepository>();
// Alternative mock implementation for testing (uncomment if external API is not accessible):
// builder.Services.AddScoped<IPokemonRepository, MockPokemonRepository>();
builder.Services.AddScoped<PokemonApplicationService>();
var app = builder.Build();

// Configurando o pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
