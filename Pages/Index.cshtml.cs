using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PokeNet.Models;
using PokeNet.Services;

namespace PokeNet.Pages;

public sealed class IndexModel(ILogger<IndexModel> logger, PokemonService pokemonService) : PageModel
{
    private readonly ILogger<IndexModel> _logger = logger;
    private readonly PokemonService _pokemonService = pokemonService;

    private static readonly int[] FeaturedPokemonIds = [1, 4, 7, 25, 133, 150, 149, 130, 131, 68, 59];

    public void OnGet()
    {
        _logger.LogInformation("Landing page loaded");
    }

    public async Task<IActionResult> OnGetDetailsAsync(int id)
    {
        var pokemon = await _pokemonService.GetPokemonByIdAsync(id);
        if (pokemon is null)
            return NotFound(new { error = $"Pokémon con ID {id} no encontrado" });
            
        return new JsonResult(pokemon);
    }

    public async Task<IActionResult> OnGetSearchAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest(new { error = "El nombre del Pokémon es requerido" });

        var pokemon = await _pokemonService.GetPokemonByNameAsync(name);
        if (pokemon is null)
            return NotFound(new { error = $"Pokémon '{name}' no encontrado" });

        return new JsonResult(pokemon);
    }

    public async Task<IActionResult> OnGetFeaturedAsync()
    {
        var pokemonList = new List<PokemonDetail>();
        foreach (var id in FeaturedPokemonIds)
        {
            var pokemon = await _pokemonService.GetPokemonByIdAsync(id);
            if (pokemon is not null)
                pokemonList.Add(pokemon);
        }
        return new JsonResult(pokemonList);
    }

    public async Task<IActionResult> OnGetByTypeAsync(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
            return BadRequest(new { error = "El tipo es requerido" });

        var pokemonList = await _pokemonService.GetPokemonByTypeAsync(type);
        if (pokemonList.Count == 0)
            return NotFound(new { error = $"No se encontraron pokemones de tipo '{type}'" });

        return new JsonResult(pokemonList);
    }

    public async Task<IActionResult> OnGetAllPaginatedAsync(int page = 1)
    {
        const int pageSize = 20;
        if (page < 1) page = 1;

        var offset = (page - 1) * pageSize;
        var (pokemonList, total) = await _pokemonService.GetPaginatedPokemonAsync(pageSize, offset);

        var totalPages = (total + pageSize - 1) / pageSize;

        return new JsonResult(new
        {
            pokemon = pokemonList,
            pagination = new
            {
                currentPage = page,
                pageSize,
                totalPokemon = total,
                totalPages
            }
        });
    }
}
