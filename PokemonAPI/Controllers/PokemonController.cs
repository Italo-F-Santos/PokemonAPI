using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;

namespace PokemonAPI.Controllers
{
    /// <summary>
    /// Controlador responsável por fornecer endpoints da API de Pokémon.
    /// </summary>
    [ApiController]
    [Route("api/pokemon")]
    public class PokemonController : ControllerBase
    {
        private readonly PokemonService _pokemonService;

        /// <summary>
        /// Construtor do controlador de Pokémon.
        /// </summary>
        /// <param name="pokemonService">Serviço responsável pela comunicação com a PokeAPI.</param>
        public PokemonController(PokemonService pokemonService)
        {
            _pokemonService = pokemonService;
        }

        /// <summary>
        /// Obtém os detalhes de um Pokémon específico.
        /// </summary>
        /// <param name="name">Nome do Pokémon.</param>
        /// <returns>Objeto JSON com os detalhes do Pokémon.</returns>
        [HttpGet("{name}")]
        public async Task<IActionResult> GetPokemon(string name)
        {
            var pokemon = await _pokemonService.GetPokemonInfoAsync(name);
            if (!pokemon.Success)
            {
                return StatusCode(pokemon.StatusCode, pokemon.Result);
            }
            return Ok(pokemon);
        }

        /// <summary>
        /// Obtém uma lista paginada de Pokémon.
        /// </summary>
        /// <param name="limit">Número de Pokémon a serem retornados (padrão: 10).</param>
        /// <param name="offset">Número de Pokémon a serem ignorados antes de começar a listar (padrão: 0).</param>
        /// <returns>Lista paginada de Pokémon.</returns>
        [HttpGet("list")]
        public async Task<IActionResult> GetPokemonList(int limit = 10, int offset = 0)
        {
            var pokemonList = await _pokemonService.GetPokemonListAsync(limit, offset);
            if (!pokemonList.Success)
            {
                return StatusCode(pokemonList.StatusCode, pokemonList.Result);
            }
            return Ok(pokemonList);
        }
    }
}
