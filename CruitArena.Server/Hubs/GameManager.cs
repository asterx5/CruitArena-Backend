using System.Collections.Concurrent;
using CruitArena.Models;

namespace CruitArena.Services;

/// <summary>
/// Manages active game sessions. Each game has its own GameState and GameEngine.
/// </summary>
public class GameManager
{
    private readonly ConcurrentDictionary<string, GameSession> _games = new();

    public GameSession CreateGame(GameRoom room)
    {
        var gameId = Guid.NewGuid().ToString();

        var player1 = new Player
        {
            Id = Guid.NewGuid().ToString(),
            Name = room.HostName,
            Cards = room.HostDeck!.Select(c => GameCard.FromCardData(c)).ToList()
        };

        var player2 = new Player
        {
            Id = Guid.NewGuid().ToString(),
            Name = room.GuestName!,
            Cards = room.GuestDeck!.Select(c => GameCard.FromCardData(c)).ToList()
        };

        var state = new GameState
        {
            GameId = gameId,
            Player1 = player1,
            Player2 = player2
        };

        var engine = new GameEngine();

        // Register originals so Restore works
        var allOriginals = room.HostDeck!.Concat(room.GuestDeck!).ToList();
        engine.RegisterOriginalCards(allOriginals);

        var session = new GameSession
        {
            GameId = gameId,
            RoomId = room.Id,
            State = state,
            Engine = engine,
            HostConnectionId = room.HostConnectionId,
            GuestConnectionId = room.GuestConnectionId!,
            Player1Id = player1.Id,
            Player2Id = player2.Id
        };

        _games[gameId] = session;
        return session;
    }

    public GameSession? GetGame(string gameId) =>
        _games.GetValueOrDefault(gameId);

    /// <summary>
    /// Find a game session by SignalR connection ID
    /// </summary>
    public GameSession? FindGameByConnection(string connectionId) =>
        _games.Values.FirstOrDefault(g =>
            g.HostConnectionId == connectionId || g.GuestConnectionId == connectionId);

    /// <summary>
    /// Get the player ID for a given connection in a game
    /// </summary>
    public string? GetPlayerId(string gameId, string connectionId)
    {
        var game = GetGame(gameId);
        if (game == null) return null;

        if (game.HostConnectionId == connectionId) return game.Player1Id;
        if (game.GuestConnectionId == connectionId) return game.Player2Id;
        return null;
    }

    public void RemoveGame(string gameId) =>
        _games.TryRemove(gameId, out _);
}

public class GameSession
{
    public string GameId { get; set; } = string.Empty;
    public string RoomId { get; set; } = string.Empty;
    public GameState State { get; set; } = new();
    public GameEngine Engine { get; set; } = new();
    public string HostConnectionId { get; set; } = string.Empty;
    public string GuestConnectionId { get; set; } = string.Empty;
    public string Player1Id { get; set; } = string.Empty;
    public string Player2Id { get; set; } = string.Empty;

    /// <summary>
    /// Thread safety lock for applying actions
    /// </summary>
    public readonly object Lock = new();
}
