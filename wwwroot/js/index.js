const modal = document.getElementById('pokemonModal');
const closeBtn = document.getElementById('closeModal');
const pokemonCards = document.querySelectorAll('.pokemon-card');
const searchInput = document.getElementById('pokemonSearch');
const noResults = document.getElementById('noResults');

pokemonCards.forEach(card => {
    card.addEventListener('click', async () => {
        const pokemonId = card.dataset.pokemonId;
        try {
            const response = await fetch(`?handler=Details&id=${pokemonId}`);
            if (!response.ok) throw new Error('No encontrado');

            const data = await response.json();

            document.getElementById('modalImage').src = data.spriteUrl;
            document.getElementById('modalName').textContent = data.name;
            document.getElementById('modalType').textContent = `Tipo: ${data.type}`;
            document.getElementById('modalDescription').textContent = data.description;

            modal.classList.remove('hidden');
        } catch (error) {
            console.error('Error:', error);
            alert('No se pudo cargar el Pokémon');
        }
    });
});

closeBtn.addEventListener('click', () => modal.classList.add('hidden'));
modal.addEventListener('click', (e) => {
    if (e.target === modal) modal.classList.add('hidden');
});

searchInput.addEventListener('input', () => {
    const query = searchInput.value.trim().toLowerCase();
    let visibleCount = 0;

    pokemonCards.forEach(card => {
        const matches = card.dataset.pokemonName.includes(query);
        card.classList.toggle('hidden', !matches);
        if (matches) visibleCount++;
    });

    noResults.classList.toggle('hidden', visibleCount > 0);
});
