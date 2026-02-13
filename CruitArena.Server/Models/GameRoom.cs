namespace CruitArena.Models;

/// <summary>
/// Represents a lobby room before a game starts.
/// Host creates it, guest joins, both mark ready, game begins.
/// </summary>
public class GameRoom
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string HostConnectionId { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public List<CardData>? HostDeck { get; set; }
    public bool HostReady { get; set; } = false;

    public string? GuestConnectionId { get; set; }
    public string? GuestName { get; set; }
    public List<CardData>? GuestDeck { get; set; }
    public bool GuestReady { get; set; } = false;

    public bool IsStarted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsFull => GuestConnectionId != null;
    public bool BothReady => HostReady && GuestReady && IsFull;
    public string DisplayName => $"{HostName}'s Room";

    public string MasterRuleId { get; set; } = string.Empty;
}

/// <summary>
/// Lightweight room info sent to clients for the lobby list
/// </summary>
public class RoomInfo
{
    public string Id { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public bool IsFull { get; set; }
    public bool IsStarted { get; set; }
    public DateTime CreatedAt { get; set; }
    public string MasterRuleId { get; set; } = string.Empty;
}
