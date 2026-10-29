import { TYPE_COLORS } from '../config/pokemon-colors.js';
import { POKEMON_TYPES } from '../config/pokemon-types.js';
import { API_HANDLERS } from '../config/api-handlers.js';
import { fetchHandler } from '../utils/fetch-handler.js';

export function pokemonApp() {
  return {
    selectedPokemon: null,
    searchQuery: '',
    isLoading: false,
    searchResults: [],
    featuredPokemon: [],
    selectedType: null,
    types: POKEMON_TYPES,
    currentPage: 1,
    totalPages: 1,
    totalPokemon: 0,
    pageSize: 20,

    capitalize(str) {
      if (!str) return '';
      return str.charAt(0).toUpperCase() + str.slice(1).toLowerCase();
    },

    getTypeColor(type) {
      return TYPE_COLORS[type?.toLowerCase()] || 'bg-blue-500/15 text-blue-300 ring-blue-500/30';
    },

    get filteredCards() {
      const cards = Array.from(document.querySelectorAll('.pokemon-card'));
      return cards.filter(card => !card.classList.contains('hidden'));
    },

    async init() {
      await this.loadPage(1);
    },

    async loadPage(page) {
      this.isLoading = true;
      try {
        const data = await fetchHandler(API_HANDLERS.ALL_PAGINATED, { page });
        this.featuredPokemon = data.pokemon;
        this.currentPage = data.pagination.currentPage;
        this.totalPages = data.pagination.totalPages;
        this.totalPokemon = data.pagination.totalPokemon;
        this.clearFilters();
        window.scrollTo({ top: 0, behavior: 'smooth' });
      } catch (error) {
        console.error('Error loading page:', error);
      } finally {
        this.isLoading = false;
      }
    },

    clearFilters() {
      this.selectedType = null;
      this.searchQuery = '';
      this.searchResults = [];
    },

    async filterCards() {
      const query = this.searchQuery.toLowerCase().trim();
      const cards = Array.from(document.querySelectorAll('.pokemon-card'));

      if (!query) {
        cards.forEach(card => card.classList.remove('hidden'));
        this.searchResults = [];
        return;
      }

      cards.forEach(card => {
        const matches = card.dataset.pokemonName?.toLowerCase().includes(query);
        card.classList.toggle('hidden', !matches);
      });

      const hasLocal = cards.some(card => !card.classList.contains('hidden'));
      if (!hasLocal) {
        await this.searchPokeAPI(query);
      }
    },

    async searchPokeAPI(query) {
      try {
        const pokemon = await fetchHandler(API_HANDLERS.SEARCH, { name: query });
        this.searchResults = [pokemon];
      } catch (error) {
        console.error('Error searching PokeAPI:', error);
        this.searchResults = [];
      }
    },

    async loadPokemon(pokemonId) {
      this.isLoading = true;
      try {
        this.selectedPokemon = await fetchHandler(API_HANDLERS.DETAILS, { id: pokemonId });
      } catch (error) {
        console.error('Error loading pokemon:', error);
        alert('No se pudo cargar el Pokémon');
        this.selectedPokemon = null;
      } finally {
        this.isLoading = false;
      }
    },

    loadPokemonFromSearch(pokemon) {
      this.selectedPokemon = pokemon;
    },

    closeModal() {
      this.selectedPokemon = null;
    },

    async filterByType(type) {
      this.selectedType = this.selectedType === type ? null : type;
      this.searchQuery = '';
      this.searchResults = [];

      if (!this.selectedType) {
        await this.loadPage(1);
        return;
      }

      this.isLoading = true;
      try {
        const pokemon = await fetchHandler(API_HANDLERS.BY_TYPE, { type: this.selectedType });
        this.featuredPokemon = pokemon;
        this.currentPage = 1;
        this.totalPages = 1;
      } catch (error) {
        console.error('Error filtering by type:', error);
        alert(`No se encontraron pokemones de tipo "${this.selectedType}"`);
        this.selectedType = null;
        await this.loadPage(1);
      } finally {
        this.isLoading = false;
      }
    },

    prevPage() {
      if (this.currentPage > 1) {
        this.loadPage(this.currentPage - 1);
      }
    },

    nextPage() {
      if (this.currentPage < this.totalPages) {
        this.loadPage(this.currentPage + 1);
      }
    }
  };
}

window.pokemonApp = pokemonApp;
