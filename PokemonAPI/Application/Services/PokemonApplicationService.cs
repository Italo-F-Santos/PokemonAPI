using PokemonAPI.Application.DTOs;
using PokemonAPI.Domain.Entities;
using PokemonAPI.Domain.Interfaces;

namespace PokemonAPI.Application.Services
{
    public class PokemonApplicationService
    {
        private readonly IPokemonRepository _pokemonRepository;

        public PokemonApplicationService(IPokemonRepository pokemonRepository)
        {
            _pokemonRepository = pokemonRepository;
        }

        public async Task<(bool Success, object Result, int StatusCode)> GetPokemonAsync(string name)
        {
            try
            {
                var pokemon = await _pokemonRepository.GetByNameAsync(name);
                var pokemonDto = MapToDto(pokemon);
                return (true, pokemonDto, 200);
            }
            catch (HttpRequestException)
            {
                return (false, new { error = "Pokémon not found." }, 404);
            }
            catch (Exception ex)
            {
                return (false, new { error = ex.Message }, 500);
            }
        }

        public async Task<(bool Success, object Result, int StatusCode)> GetPokemonListAsync(int limit = 10, int offset = 0)
        {
            try
            {
                var pokemonList = await _pokemonRepository.GetPokemonListAsync(limit, offset);
                var listDto = new PokemonListDto { Pokemon = pokemonList };
                return (true, listDto, 200);
            }
            catch (HttpRequestException)
            {
                return (false, new { error = "Failed to fetch Pokemon list." }, 500);
            }
            catch (Exception ex)
            {
                return (false, new { error = ex.Message }, 500);
            }
        }

        private static PokemonDto MapToDto(Pokemon pokemon)
        {
            return new PokemonDto
            {
                Name = pokemon.Name,
                BaseExperience = pokemon.BaseExperience,
                Height = pokemon.Height,
                Weight = pokemon.Weight,
                Abilities = pokemon.Abilities,
                Moves = pokemon.Moves,
                HeldItems = pokemon.HeldItems,
                Species = pokemon.Species,
                Types = pokemon.Types
            };
        }
    }
}