namespace PokeNet.Models;

public sealed record PokemonDetail(
    int Id,
    string Name,
    string Type,
    string Description,
    string SpriteUrl
);
