using Microsoft.AspNetCore.Mvc;
using PokemonAPI.Application.Services;

namespace PokemonAPI.Controllers
{
    /// <summary>
    /// Controlador responsável por fornecer endpoints da API de Pokémon.
    /// </summary>
    [ApiController]
    [Route("api/pokemon")]
    public class PokemonController : ControllerBase
    {
        private readonly PokemonApplicationService _pokemonApplicationService;

        /// <summary>
        /// Construtor do controlador de Pokémon.
        /// </summary>
        /// <param name="pokemonApplicationService">Serviço de aplicação responsável pela lógica de negócio.</param>
        public PokemonController(PokemonApplicationService pokemonApplicationService)
        {
            _pokemonApplicationService = pokemonApplicationService;
        }

        /// <summary>
        /// Obtém os detalhes de um Pokémon específico.
        /// </summary>
        /// <param name="name">Nome do Pokémon.</param>
        /// <returns>Objeto JSON com os detalhes do Pokémon.</returns>
        [HttpGet("{name}")]
        public async Task<IActionResult> GetPokemon(string name)
        {
            var result = await _pokemonApplicationService.GetPokemonAsync(name);
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Result);
            }
            return Ok(result.Result);
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
            var result = await _pokemonApplicationService.GetPokemonListAsync(limit, offset);
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Result);
            }
            return Ok(result.Result);
        }
    }
}
