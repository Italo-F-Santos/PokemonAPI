using System.Text.Json;
using PokemonAPI.Domain.Entities;
using PokemonAPI.Domain.Interfaces;

namespace PokemonAPI.Infrastructure.Repositories
{
    public class PokemonRepository : IPokemonRepository
    {
        private readonly HttpClient _httpClient;

        public PokemonRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Pokemon> GetByNameAsync(string name)
        {
            var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{name}");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Pokemon '{name}' not found", null, response.StatusCode);
            }

            var data = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(data).RootElement;

            return new Pokemon
            {
                Name = json.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : "Unknown",
                BaseExperience = json.TryGetProperty("base_experience", out var expProp) ? expProp.GetInt32() : 0,
                Height = json.TryGetProperty("height", out var heightProp) ? heightProp.GetInt32() : 0,
                Weight = json.TryGetProperty("weight", out var weightProp) ? weightProp.GetInt32() : 0,

                Abilities = json.TryGetProperty("abilities", out var abilitiesProp)
                    ? abilitiesProp.EnumerateArray()
                        .Select(a => a.GetProperty("ability").GetProperty("name").GetString())
                        .Where(name => name != null)
                        .ToList()!
                    : new List<string>(),

                Moves = json.TryGetProperty("moves", out var movesProp)
                    ? movesProp.EnumerateArray()
                        .Take(10)
                        .Select(m => m.GetProperty("move").GetProperty("name").GetString())
                        .Where(name => name != null)
                        .ToList()!
                    : new List<string>(),

                HeldItems = json.TryGetProperty("held_items", out var itemsProp)
                    ? itemsProp.EnumerateArray()
                        .Select(i => i.GetProperty("item").GetProperty("name").GetString())
                        .Where(name => name != null)
                        .ToList()!
                    : new List<string>(),

                Species = json.TryGetProperty("species", out var speciesProp)
                    ? speciesProp.GetProperty("name").GetString() ?? "Unknown"
                    : "Unknown",

                Types = json.TryGetProperty("types", out var typesProp)
                    ? typesProp.EnumerateArray()
                        .Select(t => t.GetProperty("type").GetProperty("name").GetString())
                        .Where(name => name != null)
                        .ToList()!
                    : new List<string>()
            };
        }

        public async Task<List<string>> GetPokemonListAsync(int limit = 10, int offset = 0)
        {
            var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon?limit={limit}&offset={offset}");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Failed to fetch Pokemon list", null, response.StatusCode);
            }

            var data = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(data).RootElement;

            return json.GetProperty("results")
                      .EnumerateArray()
                      .Select(p => p.GetProperty("name").GetString())
                      .Where(name => name != null)
                      .ToList()!;
        }
    }
}