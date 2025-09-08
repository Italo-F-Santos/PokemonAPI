using PokemonAPI.Domain.Entities;
using PokemonAPI.Domain.Interfaces;

namespace PokemonAPI.Infrastructure.Repositories
{
    public class MockPokemonRepository : IPokemonRepository
    {
        private readonly Dictionary<string, Pokemon> _mockPokemons;

        public MockPokemonRepository()
        {
            _mockPokemons = new Dictionary<string, Pokemon>
            {
                ["pikachu"] = new Pokemon
                {
                    Name = "pikachu",
                    BaseExperience = 112,
                    Height = 4,
                    Weight = 60,
                    Abilities = new List<string> { "static", "lightning-rod" },
                    Moves = new List<string> { "thunderbolt", "quick-attack", "thunder", "agility", "tackle" },
                    HeldItems = new List<string> { "light-ball" },
                    Species = "pikachu",
                    Types = new List<string> { "electric" }
                },
                ["charizard"] = new Pokemon
                {
                    Name = "charizard",
                    BaseExperience = 267,
                    Height = 17,
                    Weight = 905,
                    Abilities = new List<string> { "blaze", "solar-power" },
                    Moves = new List<string> { "flamethrower", "fly", "dragon-claw", "fire-blast", "slash" },
                    HeldItems = new List<string>(),
                    Species = "charizard",
                    Types = new List<string> { "fire", "flying" }
                }
            };
        }

        public Task<Pokemon> GetByNameAsync(string name)
        {
            var normalizedName = name.ToLower();
            if (_mockPokemons.TryGetValue(normalizedName, out var pokemon))
            {
                return Task.FromResult(pokemon);
            }
            
            throw new HttpRequestException($"Pokemon '{name}' not found", null, System.Net.HttpStatusCode.NotFound);
        }

        public Task<List<string>> GetPokemonListAsync(int limit = 10, int offset = 0)
        {
            var pokemonNames = _mockPokemons.Keys.Skip(offset).Take(limit).ToList();
            return Task.FromResult(pokemonNames);
        }
    }
}