namespace CruitArena.Models;

/// <summary>
/// A mutable in-game card instance. Created from a CardData template.
/// Each card has a unique instance Id separate from the CardData.Id it was created from.
/// </summary>
public class GameCard
{
    // Location constants
    public const string LocationHand = "hand";
    public const string LocationField = "field";
    public const string LocationGraveyard = "graveyard";
    public const string LocationPhantomZone = "phantom_zone";

    // Identity
    public string Id { get; set; } = string.Empty;           // Unique instance ID
    public string CardDataId { get; set; } = string.Empty;   // Reference to original CardData
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;

    // Mutable game properties (can be modified during game)
    public string Description { get; set; } = string.Empty;
    public int? Tier { get; set; }
    public string? Archetype { get; set; }
    public string? Specialty { get; set; }
    public int? Priority { get; set; }
    public string Rarity { get; set; } = string.Empty;

    // Game state
    public string Location { get; set; } = LocationHand;
    public bool IsFaceUp { get; set; } = false;
    public bool IsKnown { get; set; } = false;
    public bool IsFrozen { get; set; } = false;
    public bool IsNegated { get; set; } = false;
    public bool IsNullified { get; set; } = false;
    public bool IsRevealed { get; set; } = false;

    // Layer system
    public int ProtectionLayer { get; set; } = 0;
    public int DestructionLayer { get; set; } = 0;
    public int NegationLayer { get; set; } = 0;
    public int ZeroLayer { get; set; } = 0;

    // Convenience
    public bool HasAnyLayer() =>
        ProtectionLayer > 0 || DestructionLayer > 0 || NegationLayer > 0 || ZeroLayer > 0;

    public int GetTotalLayers() =>
        ProtectionLayer + DestructionLayer + NegationLayer + ZeroLayer;

    /// <summary>
    /// Create a GameCard instance from a CardData template
    /// </summary>
    public static GameCard FromCardData(CardData data, string? instanceId = null)
    {
        return new GameCard
        {
            Id = instanceId ?? Guid.NewGuid().ToString(),
            CardDataId = data.Id,
            Name = data.Name,
            Type = data.Type,
            Description = data.Description,
            Tier = data.Tier,
            Archetype = data.Archetype,
            Specialty = data.Specialty,
            Priority = data.Priority,
            Rarity = data.Rarity,
            Location = LocationHand
        };
    }
}
