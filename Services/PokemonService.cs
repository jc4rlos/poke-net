using System.Net.Http.Json;
using System.Text.Json.Serialization;
using PokeNet.Models;

namespace PokeNet.Services;

public sealed class PokemonService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PokemonService> _logger;
    private const string PokeApiBaseUrl = "https://pokeapi.co/api/v2";

    public PokemonService(HttpClient httpClient, ILogger<PokemonService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PokemonDetail?> GetPokemonByIdAsync(int id)
    {
        return await GetPokemonByNameAsync(id.ToString());
    }

    public async Task<(List<PokemonDetail> Pokemon, int Total)> GetPaginatedPokemonAsync(int limit = 20, int offset = 0)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{PokeApiBaseUrl}/pokemon?limit={limit}&offset={offset}");
            if (!response.IsSuccessStatusCode)
                return ([], 0);

            var result = await response.Content.ReadFromJsonAsync<PaginatedResult>();
            if (result?.Results is null)
                return ([], 0);

            var pokemonList = new List<PokemonDetail>();
            foreach (var entry in result.Results)
            {
                var pokemon = await GetPokemonByNameAsync(entry.Name);
                if (pokemon is not null)
                    pokemonList.Add(pokemon);
            }

            return (pokemonList, result.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching paginated pokemon");
            return ([], 0);
        }
    }

    public async Task<List<PokemonDetail>> GetPokemonByTypeAsync(string type)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{PokeApiBaseUrl}/type/{type.ToLowerInvariant()}");
            if (!response.IsSuccessStatusCode)
                return [];

            var typeData = await response.Content.ReadFromJsonAsync<PokeApiType>();
            if (typeData?.Pokemon is null)
                return [];

            var pokemonList = new List<PokemonDetail>();
            foreach (var pokemonEntry in typeData.Pokemon.Take(20))
            {
                var pokemon = await GetPokemonByNameAsync(pokemonEntry.Pokemon.Name);
                if (pokemon is not null)
                    pokemonList.Add(pokemon);
            }

            return pokemonList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching pokemon by type {Type}", type);
            return [];
        }
    }

    public async Task<PokemonDetail?> GetPokemonByNameAsync(string name)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{PokeApiBaseUrl}/pokemon/{name.ToLowerInvariant()}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Pokemon {Name} not found", name);
                return null;
            }

            var pokemon = await response.Content.ReadFromJsonAsync<PokeApiPokemon>();
            if (pokemon is null)
                return null;

            var species = await GetPokemonSpeciesAsync(pokemon.Id);

            return new PokemonDetail(
                Id: pokemon.Id,
                Name: pokemon.Name,
                Type: pokemon.Types.FirstOrDefault()?.Type.Name ?? "Unknown",
                Description: species?.Description ?? "Sin descripción disponible",
                SpriteUrl: pokemon.Sprites.Other?.OfficialArtwork?.FrontDefault ?? ""
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching pokemon {Name}", name);
            return null;
        }
    }

    private async Task<PokeApiSpecies?> GetPokemonSpeciesAsync(int pokemonId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{PokeApiBaseUrl}/pokemon-species/{pokemonId}");
            if (!response.IsSuccessStatusCode) return null;

            var species = await response.Content.ReadFromJsonAsync<PokeApiSpecies>();
            return species;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching pokemon species {Id}", pokemonId);
            return null;
        }
    }
}

// DTO classes for PokeAPI responses
internal sealed class PokeApiPokemon
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("types")]
    public List<TypeInfo> Types { get; set; } = [];

    [JsonPropertyName("sprites")]
    public Sprites Sprites { get; set; } = new();
}

internal sealed class TypeInfo
{
    [JsonPropertyName("type")]
    public NamedResource Type { get; set; } = new();
}

internal sealed class NamedResource
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}

internal sealed class Sprites
{
    [JsonPropertyName("other")]
    public OtherSprites? Other { get; set; }
}

internal sealed class OtherSprites
{
    [JsonPropertyName("official-artwork")]
    public ArtworkSprite? OfficialArtwork { get; set; }
}

internal sealed class ArtworkSprite
{
    [JsonPropertyName("front_default")]
    public string? FrontDefault { get; set; }
}

internal sealed class PokeApiSpecies
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("flavor_text_entries")]
    public List<FlavorText> FlavorTextEntries { get; set; } = [];

    public string Description =>
        FlavorTextEntries
            .FirstOrDefault(f => f.Language.Name == "es")?
            .Text?
            .Replace("\n", " ")
            .Replace("\r", " ")
            .Trim() ?? "";
}

internal sealed class FlavorText
{
    [JsonPropertyName("flavor_text")]
    public string? Text { get; set; }

    [JsonPropertyName("language")]
    public NamedResource Language { get; set; } = new();
}

internal sealed class PokeApiType
{
    [JsonPropertyName("pokemon")]
    public List<PokemonTypeEntry> Pokemon { get; set; } = [];
}

internal sealed class PokemonTypeEntry
{
    [JsonPropertyName("pokemon")]
    public NamedResource Pokemon { get; set; } = new();
}

internal sealed class PaginatedResult
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("results")]
    public List<PaginatedPokemon> Results { get; set; } = [];
}

internal sealed class PaginatedPokemon
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("url")]
    public string Url { get; set; } = "";
}
