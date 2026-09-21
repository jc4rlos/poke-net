using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PokeNet.Pages;

public sealed class IndexModel(ILogger<IndexModel> logger) : PageModel
{
    private readonly ILogger<IndexModel> _logger = logger;

    private static readonly Dictionary<int, string> PokemonDescriptions = new()
    {
        { 1, "Bulbasaur es un Pokémon tipo Planta/Veneno. Tiene un bulbo en su espalda que crece mientras se desarrolla." },
        { 4, "Charmander es un Pokémon tipo Fuego. Vive en montañas rocosas y expulsa fuego por la boca." },
        { 7, "Squirtle es un Pokémon tipo Agua. Se retrae dentro de su caparazón para defenderse." },
        { 25, "Pikachu es un Pokémon tipo Eléctrico. Tiene bolsas en las mejillas que acumulan electricidad." },
        { 59, "Arcanine es un Pokémon tipo Fuego. Es conocido por su velocidad y poder en batalla." },
        { 68, "Machamp es un Pokémon tipo Lucha. Tiene cuatro brazos que usa para golpear con precisión." },
        { 130, "Gyarados es un Pokémon tipo Agua/Volador. Es una bestia devastadora cuando entra en batalla." },
        { 131, "Lapras es un Pokémon tipo Agua/Hielo. Puede llevar personas sobre su caparazón." },
        { 133, "Eevee es un Pokémon tipo Normal. Tiene una capacidad única: puede evolucionar de varias formas." },
        { 149, "Dragonite es un Pokémon tipo Dragón/Volador. Es sumamente raro y muy poderoso." },
        { 150, "Mewtwo es un Pokémon tipo Psíquico legendario. Fue creado genéticamente en un laboratorio." },
    };

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

    public IActionResult OnGetDetails(int id)
    {
        if (!PokemonDescriptions.TryGetValue(id, out var description))
            return NotFound();

        var pokemon = FeaturedPokemon.FirstOrDefault(p => p.Id == id);
        if (pokemon is null)
            return NotFound();

        return new JsonResult(new
        {
            pokemon.Id,
            pokemon.Name,
            pokemon.Type,
            Description = description,
            pokemon.SpriteUrl
        });
    }
}

public sealed record PokemonCard(int Id, string Name, string Type, string TypeBadgeClasses)
{
    public string SpriteUrl =>
        $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/{Id}.png";
}
