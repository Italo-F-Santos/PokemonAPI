namespace PokemonAPI.Domain.Entities
{
    public class Pokemon
    {
        public string Name { get; set; }
        public int BaseExperience { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public List<string> Abilities { get; set; } = new();
        public List<string> Moves { get; set; } = new();
        public List<string> HeldItems { get; set; } = new();
        public string Species { get; set; }
        public List<string> Types { get; set; } = new();
    }
}