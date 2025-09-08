using PokemonAPI.Domain.Entities;

namespace PokemonAPI.Domain.Interfaces
{
    public interface IPokemonRepository
    {
        Task<Pokemon> GetByNameAsync(string name);
        Task<List<string>> GetPokemonListAsync(int limit = 10, int offset = 0);
    }
}