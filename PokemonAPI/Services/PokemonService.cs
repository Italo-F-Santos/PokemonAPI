using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using PokemonAPI.Models;


namespace PokemonAPI
{
    public class PokemonService
    {
        private readonly HttpClient _httpClient;

        public PokemonService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PokemonInfo> GetPokemonAsync(string name)
        {
            var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{name}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var data = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(data).RootElement;

            return new PokemonInfo
            {
                Name = json.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : "Unknown",
                BaseExperience = json.TryGetProperty("base_experience", out var expProp) ? expProp.GetInt32() : 0,
                Height = json.TryGetProperty("height", out var heightProp) ? heightProp.GetInt32() : 0,
                Weight = json.TryGetProperty("weight", out var weightProp) ? weightProp.GetInt32() : 0,
                Abilities = json.TryGetProperty("abilities", out var abilitiesProp)
                    ? abilitiesProp.EnumerateArray()
                        .Where(a => a.TryGetProperty("ability", out var ability) && ability.TryGetProperty("name", out var abilityName))
                        .Select(a => a.GetProperty("ability").GetProperty("name").GetString())
                        .ToList()
                    : new List<string>(),
                Moves = json.TryGetProperty("moves", out var movesProp)
                    ? movesProp.EnumerateArray()
                        .Take(10) // Pegamos apenas os 10 primeiros para não sobrecarregar
                        .Where(m => m.TryGetProperty("move", out var move) && move.TryGetProperty("name", out var moveName))
                        .Select(m => m.GetProperty("move").GetProperty("name").GetString())
                        .ToList()
                    : new List<string>(),
                HeldItems = json.TryGetProperty("held_items", out var itemsProp)
                    ? itemsProp.EnumerateArray()
                        .Where(i => i.TryGetProperty("item", out var item) && item.TryGetProperty("name", out var itemName))
                        .Select(i => i.GetProperty("item").GetProperty("name").GetString())
                        .ToList()
                    : new List<string>(),
                Species = json.TryGetProperty("species", out var speciesProp) && speciesProp.TryGetProperty("name", out var speciesName)
                    ? speciesProp.GetProperty("name").GetString()
                    : "Unknown",
                Types = json.TryGetProperty("types", out var typesProp)
                    ? typesProp.EnumerateArray()
                        .Where(t => t.TryGetProperty("type", out var type) && type.TryGetProperty("name", out var typeName))
                        .Select(t => t.GetProperty("type").GetProperty("name").GetString())
                        .ToList()
                    : new List<string>()
            };
        }

        
    }
}
