# PokeNet

POC de una landing page con temática Pokémon construida con ASP.NET Core 8 (Razor Pages) y Tailwind CSS (vía CDN).

## Stack

- .NET 8 / ASP.NET Core Razor Pages
- Tailwind CSS (CDN, sin build step, ideal para este POC)
- `Microsoft.Extensions.Diagnostics.HealthChecks` para el monitoreo de salud

## Ejecutar

```bash
dotnet run
```

La app queda disponible en `http://localhost:5054` (ver `Properties/launchSettings.json`).

## Endpoints

- `GET /` — landing page.
- `GET /health` — estado de salud de la aplicación en JSON (`status`, `checks`, `timestampUtc`).
