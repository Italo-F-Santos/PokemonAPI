namespace PokemonAPI.Models
{
    public class PokemonInfo
    {
        public string Name { get; set; }
        public int BaseExperience { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public List<string> Abilities { get; set; }
        public List<string> Moves { get; set; }
        public List<string> HeldItems { get; set; }
        public string Species { get; set; }
        public List<string> Types { get; set; }
    }
}
