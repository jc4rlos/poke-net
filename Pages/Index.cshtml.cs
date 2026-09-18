using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PokeNet.Pages;

public sealed class IndexModel(ILogger<IndexModel> logger) : PageModel
{
    private readonly ILogger<IndexModel> _logger = logger;

    public IReadOnlyList<PokemonCard> FeaturedPokemon { get; } =
    [
        new(1, "Bulbasaur", "Planta", "bg-emerald-500/15 text-emerald-300 ring-emerald-500/30"),
        new(4, "Charmander", "Fuego", "bg-orange-500/15 text-orange-300 ring-orange-500/30"),
        new(7, "Squirtle", "Agua", "bg-sky-500/15 text-sky-300 ring-sky-500/30"),
        new(25, "Pikachu", "Eléctrico", "bg-yellow-500/15 text-yellow-300 ring-yellow-500/30"),
        new(133, "Eevee", "Normal", "bg-amber-500/15 text-amber-300 ring-amber-500/30"),
        new(150, "Mewtwo", "Psíquico", "bg-purple-500/15 text-purple-300 ring-purple-500/30"),
        new(149, "Dragonite", "Dragón", "bg-indigo-500/15 text-indigo-300 ring-indigo-500/30"),
        new(130, "Gyarados", "Agua", "bg-blue-600/15 text-blue-300 ring-blue-600/30"),
        new(131, "Lapras", "Hielo", "bg-cyan-500/15 text-cyan-300 ring-cyan-500/30"),
        new(68, "Machamp", "Lucha", "bg-red-600/15 text-red-300 ring-red-600/30"),
        new(59, "Arcanine", "Fuego", "bg-orange-600/15 text-orange-300 ring-orange-600/30"),
    ];

    public void OnGet()
    {
        _logger.LogInformation("Landing page rendered with {Count} featured Pokemon", FeaturedPokemon.Count);
    }
}

public sealed record PokemonCard(int Id, string Name, string Type, string TypeBadgeClasses)
{
    public string SpriteUrl =>
        $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/{Id}.png";
}
