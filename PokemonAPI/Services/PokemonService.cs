using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using PokemonAPI.Models;

namespace PokemonAPI
{
    /// <summary>
    /// Serviço para buscar informações de Pokémon a partir da PokeAPI.
    /// </summary>
    public class PokemonService
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Construtor da classe PokemonService.
        /// </summary>
        /// <param name="httpClient">Instância de HttpClient para realizar requisições HTTP.</param>
        public PokemonService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Obtém informações de um Pokémon a partir da API.
        /// </summary>
        /// <param name="name">Nome do Pokémon a ser buscado.</param>
        /// <returns>Objeto <see cref="PokemonInfo"/> contendo detalhes do Pokémon.</returns>
        public async Task<PokemonInfo> GetPokemonInfoAsync(string name)
        {
            // Faz uma requisição HTTP para obter os dados do Pokémon.
            var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{name}");

            // Se a requisição falhar, retorna null.
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            // Lê a resposta da API e converte para JSON.
            var data = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(data).RootElement;

            // Retorna um objeto contendo todas as informações extraídas do JSON.
            return new PokemonInfo
            {
                Name = json.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : "Unknown",
                BaseExperience = json.TryGetProperty("base_experience", out var expProp) ? expProp.GetInt32() : 0,
                Height = json.TryGetProperty("height", out var heightProp) ? heightProp.GetInt32() : 0,
                Weight = json.TryGetProperty("weight", out var weightProp) ? weightProp.GetInt32() : 0,

                /// <summary>
                /// Lista das habilidades do Pokémon.
                /// </summary>
                Abilities = json.TryGetProperty("abilities", out var abilitiesProp)
                    ? abilitiesProp.EnumerateArray()
                        .Where(a => a.TryGetProperty("ability", out var ability) && ability.TryGetProperty("name", out var abilityName))
                        .Select(a => a.GetProperty("ability").GetProperty("name").GetString())
                        .ToList()
                    : new List<string>(),

                /// <summary>
                /// Lista dos primeiros 10 golpes disponíveis do Pokémon.
                /// </summary>
                Moves = json.TryGetProperty("moves", out var movesProp)
                    ? movesProp.EnumerateArray()
                        .Take(10) // Pegamos apenas os 10 primeiros para não sobrecarregar
                        .Where(m => m.TryGetProperty("move", out var move) && move.TryGetProperty("name", out var moveName))
                        .Select(m => m.GetProperty("move").GetProperty("name").GetString())
                        .ToList()
                    : new List<string>(),

                /// <summary>
                /// Itens que o Pokémon pode segurar.
                /// </summary>
                HeldItems = json.TryGetProperty("held_items", out var itemsProp)
                    ? itemsProp.EnumerateArray()
                        .Where(i => i.TryGetProperty("item", out var item) && item.TryGetProperty("name", out var itemName))
                        .Select(i => i.GetProperty("item").GetProperty("name").GetString())
                        .ToList()
                    : new List<string>(),

                /// <summary>
                /// Espécie do Pokémon.
                /// </summary>
                Species = json.TryGetProperty("species", out var speciesProp) && speciesProp.TryGetProperty("name", out var speciesName)
                    ? speciesProp.GetProperty("name").GetString()
                    : "Unknown",

                /// <summary>
                /// Tipos do Pokémon (exemplo: Fogo, Água, Planta).
                /// </summary>
                Types = json.TryGetProperty("types", out var typesProp)
                    ? typesProp.EnumerateArray()
                        .Where(t => t.TryGetProperty("type", out var type) && type.TryGetProperty("name", out var typeName))
                        .Select(t => t.GetProperty("type").GetProperty("name").GetString())
                        .ToList()
                    : new List<string>()
            };
        }


        /// <summary>
        /// Obtém uma lista paginada de Pokémon da PokeAPI.
        /// </summary>
        /// <param name="limit">Número de Pokémon a serem retornados (padrão: 10).</param>
        /// <param name="offset">Número de Pokémon a serem ignorados antes de começar a listar (padrão: 0).</param>
        /// <returns>Objeto JSON contendo a lista de Pokémon.</returns>
        public async Task<JsonElement?> GetPokemonListAsync(int limit = 10, int offset = 0)
        {
            var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon?limit={limit}&offset={offset}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var data = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonElement>(data);
        }


    }
}
