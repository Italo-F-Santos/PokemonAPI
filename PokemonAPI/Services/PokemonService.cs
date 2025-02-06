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
        public async Task<(bool Success, object Result, int StatusCode)> GetPokemonInfoAsync(string name)
        {
            var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{name}");

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = response.Content != null
                    ? await response.Content.ReadAsStringAsync()
                    : "No error details provided.";

                return (false, new { error = errorMessage }, (int)response.StatusCode);
            }

            var data = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(data).RootElement;

            var pokemonInfo = new PokemonInfo
            {
                Name = json.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : "Unknown",
                BaseExperience = json.TryGetProperty("base_experience", out var expProp) ? expProp.GetInt32() : 0,
                Height = json.TryGetProperty("height", out var heightProp) ? heightProp.GetInt32() : 0,
                Weight = json.TryGetProperty("weight", out var weightProp) ? weightProp.GetInt32() : 0,

                // Lista das habilidades do Pokémon
                Abilities = json.TryGetProperty("abilities", out var abilitiesProp)
                    ? abilitiesProp.EnumerateArray()
                        .Select(a => a.GetProperty("ability").GetProperty("name").GetString())
                        .ToList()
                    : new List<string>(),

                // Lista dos primeiros 10 golpes disponíveis do Pokémon
                Moves = json.TryGetProperty("moves", out var movesProp)
                    ? movesProp.EnumerateArray()
                        .Take(10) // Pegamos apenas os 10 primeiros para não sobrecarregar
                        .Select(m => m.GetProperty("move").GetProperty("name").GetString())
                        .ToList()
                    : new List<string>(),

                // Itens que o Pokémon pode segurar
                HeldItems = json.TryGetProperty("held_items", out var itemsProp)
                    ? itemsProp.EnumerateArray()
                        .Select(i => i.GetProperty("item").GetProperty("name").GetString())
                        .ToList()
                    : new List<string>(),

                // Espécie do Pokémon
                Species = json.TryGetProperty("species", out var speciesProp)
                    ? speciesProp.GetProperty("name").GetString()
                    : "Unknown",

                // Tipos do Pokémon (exemplo: Fogo, Água, Planta)
                Types = json.TryGetProperty("types", out var typesProp)
                    ? typesProp.EnumerateArray()
                        .Select(t => t.GetProperty("type").GetProperty("name").GetString())
                        .ToList()
                    : new List<string>()
            };

            return (true, pokemonInfo, (int)response.StatusCode);
        }



        /// <summary>
        /// Obtém uma lista paginada de Pokémon da PokeAPI.
        /// </summary>
        /// <param name="limit">Número de Pokémon a serem retornados (padrão: 10).</param>
        /// <param name="offset">Número de Pokémon a serem ignorados antes de começar a listar (padrão: 0).</param>
        /// <returns>Objeto JSON contendo a lista de Pokémon.</returns>
        public async Task<(bool Success, object Result, int StatusCode)> GetPokemonListAsync(int limit = 10, int offset = 0)
        {
            var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon?limit={limit}&offset={offset}");

            if (!response.IsSuccessStatusCode)
            {
                // Verifica se a resposta tem um corpo válido antes de ler
                var errorMessage = response.Content != null
                    ? await response.Content.ReadAsStringAsync()
                    : "No error details provided.";

                return response.StatusCode switch
                {
                    System.Net.HttpStatusCode.BadRequest => (false, new { error = "Invalid request parameters." }, 400),
                    System.Net.HttpStatusCode.NotFound => (false, new { error = "Pokémon list not found." }, 404),
                    System.Net.HttpStatusCode.InternalServerError => (false, new { error = "Internal Server Error at PokeAPI." }, 500),
                    _ => (false, new { error = $"Unexpected error: {errorMessage}" }, (int)response.StatusCode)
                };
            }

            var data = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(data).RootElement;

            var pokemonList = json.GetProperty("results")
                                  .EnumerateArray()
                                  .Select(p => p.GetProperty("name").GetString())
                                  .ToList();

            return (true, new { pokemon = pokemonList }, 200);
        }



    }
}
