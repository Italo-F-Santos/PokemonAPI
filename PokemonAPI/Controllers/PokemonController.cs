using Microsoft.AspNetCore.Mvc;
using PokemonAPI;
using System.Text.Json;

namespace PokemonAPI.Controllers
{

    [ApiController]
    [Route("api/pokemon")]
    public class PokemonController : ControllerBase
    {
        private readonly PokemonService _pokemonService;

        public PokemonController(PokemonService pokemonService)
        {
            _pokemonService = pokemonService;
        }


        [HttpGet("{name}")]
        public async Task<IActionResult> GetPokemon(string name)
        {
            var pokemon = await _pokemonService.GetPokemonAsync(name);
            if (pokemon == null)
            {
                return NotFound(new { message = "Pokémon not found." });
            }
            return Ok(pokemon);
        }

        //    [HttpGet("list")]
        //    public async Task<IActionResult> GetPokemonList(int limit = 10, int offset = 0)
        //    {
        //        var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon?limit={limit}&offset={offset}");
        //        if (!response.IsSuccessStatusCode)
        //        {
        //            return BadRequest(new { message = "Error fetching Pokémon list." });
        //        }

        //        var data = await response.Content.ReadAsStringAsync();
        //        var jsonData = JsonSerializer.Deserialize<JsonElement>(data);
        //        return Ok(jsonData);
        //    }
        //}

    }

}

