namespace CruitArena.Models;

/// <summary>
/// Immutable card template data. Players send these when joining a game.
/// This is the "blueprint" — GameCard is the mutable in-game instance.
/// </summary>
public class CardData
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "monster" or "spell"
    public string Description { get; set; } = string.Empty;
    public int? Tier { get; set; }
    public string? Archetype { get; set; }
    public string? Specialty { get; set; }
    public int? Priority { get; set; }
    public string Rarity { get; set; } = string.Empty;
}
