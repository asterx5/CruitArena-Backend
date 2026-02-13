using CruitArena.Models;
using CruitArena.Services;
using Microsoft.AspNetCore.SignalR;

namespace CruitArena.Hubs;

/// <summary>
/// SignalR Hub for CruitArena.
///
/// Flow:
///   1. CreateRoom → host gets roomId
///   2. JoinRoom  → guest joins, both notified
///   3. SetDeck   → each player submits their deck
///   4. SetReady  → once both ready, server creates game and broadcasts initial state
///   5. SendAction → server applies action, broadcasts updated state to both players
///
/// SignalR Groups:
///   - "room_{roomId}"  → used during lobby phase
///   - "game_{gameId}"  → used during gameplay
/// </summary>
public class GameHub : Hub
{
    private readonly RoomManager _rooms;
    private readonly GameManager _games;

    public GameHub(RoomManager rooms, GameManager games)
    {
        _rooms = rooms;
        _games = games;
    }

    // ==================== LOBBY ====================

    /// <summary>
    /// Host creates a new room and joins its SignalR group
    /// </summary>
    public async Task CreateRoom(string playerName, string masterRuleId)
    {
        Console.WriteLine($"{playerName} creating room with masterRuleId {masterRuleId}");
        var room = _rooms.CreateRoom(Context.ConnectionId, playerName, masterRuleId);
        await Groups.AddToGroupAsync(Context.ConnectionId, $"room_{room.Id}");
        await Clients.Caller.SendAsync("RoomCreated", room.Id);
        await BroadcastRoomList();
    }

    /// <summary>
    /// Guest joins an existing room
    /// </summary>
    public async Task JoinRoom(string roomId, string playerName)
    {
        var room = _rooms.GetRoom(roomId);
        if (room == null)
        {
            await Clients.Caller.SendAsync("Error", "Room not found");
            return;
        }

        if (room.HostConnectionId == Context.ConnectionId)
        {
            await Clients.Caller.SendAsync("RoomInfo", new
            {
                room.Id,
                HostName = room.HostName,
                GuestName = room.GuestName ?? string.Empty,
                HostHasDeck = room.HostDeck != null,
                GuestHasDeck = room.GuestDeck != null
            });
            return;
        }

        if (room.HostConnectionId == string.Empty)
        {
            room.HostConnectionId = Context.ConnectionId;
            room.HostName = playerName;
            await Groups.AddToGroupAsync(Context.ConnectionId, $"room_{roomId}");
            await Clients.Caller.SendAsync("RoomInfo", new
            {
                room.Id,
                HostName = room.HostName,
                GuestName = room.GuestName,
                HostHasDeck = room.HostDeck != null,
                GuestHasDeck = room.GuestDeck != null
            });
            await BroadcastRoomList();
            return;
        }
        var success = _rooms.TryJoinRoom(roomId, Context.ConnectionId, playerName);
        if (!success)
        {
            await Clients.Caller.SendAsync("Error", "Room is full or doesn't exist");
            return;
        }

        //var room = _rooms.GetRoom(roomId);
        //if (room == null) return;

        await Groups.AddToGroupAsync(Context.ConnectionId, $"room_{roomId}");

        // ✅ Send full room info to the guest who just joined
        await Clients.Caller.SendAsync("RoomInfo", new
        {
            room.Id,
            HostName = room.HostName,
            GuestName = playerName,
            HostHasDeck = room.HostDeck != null,
            GuestHasDeck = room.GuestDeck != null
        });

        // Notify others in room that guest joined
        await Clients.OthersInGroup($"room_{roomId}").SendAsync("PlayerJoined", playerName);

        await BroadcastRoomList();
    }

    /// <summary>
    /// Host or guest leaves the room. If host leaves, the room is closed and everyone in it is notified.
    /// </summary>
    public async Task RemoveRoom(string roomId)
    {
        var room = _rooms.GetRoom(roomId);
        if (room == null) return;
        if (room.HostConnectionId == Context.ConnectionId)
        {
            await Clients.Group($"room_{roomId}").SendAsync("PlayerLeft");
            _rooms.RemoveRoom(roomId);
            await BroadcastRoomList();
        }
    }

    /// <summary>
    /// Player submits their deck for the game
    /// </summary>
    public async Task SetDeck(string roomId, List<CardData> deck)
    {
        _rooms.SetDeck(roomId, Context.ConnectionId, deck);
        var room = _rooms.GetRoom(roomId);
        if (room == null) return;

        // Notify the room about deck status
        await Clients.Group($"room_{roomId}").SendAsync("DeckSubmitted", new
        {
            HostHasDeck = room.HostDeck != null,
            GuestHasDeck = room.GuestDeck != null
        });
    }

    /// <summary>
    /// Player marks themselves as ready. If both ready + both have decks → start game.
    /// </summary>
    public async Task SetReady(string roomId)
    {
        _rooms.SetReady(roomId, Context.ConnectionId);
        var room = _rooms.GetRoom(roomId);
        if (room == null) return;

        await Clients.Group($"room_{roomId}").SendAsync("ReadyStateChanged", new
        {
            HostReady = room.HostReady,
            GuestReady = room.GuestReady
        });

        // Both ready and both have decks → create the game
        if (room.BothReady && room.HostDeck != null && room.GuestDeck != null)
        {
            await StartGame(room);
        }
    }

    /// <summary>
    /// Get the list of available rooms
    /// </summary>
    public async Task GetRooms()
    {
        var rooms = _rooms.GetAvailableRooms();
        await Clients.Caller.SendAsync("RoomList", rooms);
    }

    // ==================== GAME ====================

    /// <summary>
    /// Player sends a game action. Server applies it and broadcasts the new state.
    /// </summary>
    public async Task SendAction(string gameId, GameAction action)
    {
        var session = _games.GetGame(gameId);
        if (session == null)
        {
            await Clients.Caller.SendAsync("Error", "Game not found");
            return;
        }

        // Apply action with lock for thread safety (both players can act simultaneously)
        lock (session.Lock)
        {
            session.Engine.ApplyAction(session.State, action);
        }

        // Broadcast updated state to both players
        await Clients.Group($"game_{gameId}").SendAsync("GameStateUpdated", session.State);

        // Check for game over
        if (session.State.IsGameOver && session.State.Winner != null)
        {
            await Clients.Group($"game_{gameId}").SendAsync("GameOver", session.State.Winner.Name);
            _games.RemoveGame(gameId);
            _rooms.RemoveRoom(session.RoomId);
        }
    }

    /// <summary>
    /// Request the current game state (useful for reconnection)
    /// </summary>
    public async Task GetGameState(string gameId)
    {
        var session = _games.GetGame(gameId);
        if (session == null)
        {
            await Clients.Caller.SendAsync("Error", "Game not found");
            return;
        }

        await Clients.Caller.SendAsync("GameStateUpdated", session.State);
    }

    // ==================== CONNECTION LIFECYCLE ====================

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Clean up rooms
        var room = _rooms.FindRoomByConnection(Context.ConnectionId);
        if (room != null)
        {
            if (!room.IsStarted)
            {
                await Clients.Group($"room_{room.Id}").SendAsync("PlayerDisconnected");
                _rooms.RemoveRoom(room.Id);
                await BroadcastRoomList();
            }

            if (room.HostConnectionId == Context.ConnectionId)
            {
                room.HostConnectionId = string.Empty;
            }

            if (room.GuestConnectionId == Context.ConnectionId)
            {
                room.GuestConnectionId = null;
            }
        }

        // Notify game opponent
        var game = _games.FindGameByConnection(Context.ConnectionId);
        if (game != null)
        {
            await Clients.Group($"game_{game.GameId}").SendAsync("OpponentDisconnected");
        }

        await base.OnDisconnectedAsync(exception);
    }

    // ==================== PRIVATE HELPERS ====================

    private async Task StartGame(GameRoom room)
    {
        _rooms.MarkStarted(room.Id);

        var session = _games.CreateGame(room);

        // Move both players from the room group to the game group
        await Groups.AddToGroupAsync(room.HostConnectionId, $"game_{session.GameId}");
        await Groups.AddToGroupAsync(room.GuestConnectionId!, $"game_{session.GameId}");

        // Send game started with each player's ID so they know which player they are
        await Clients.Client(room.HostConnectionId).SendAsync("GameStarted", new
        {
            session.GameId,
            MyPlayerId = session.Player1Id,
            State = session.State
        });

        await Clients.Client(room.GuestConnectionId!).SendAsync("GameStarted", new
        {
            session.GameId,
            MyPlayerId = session.Player2Id,
            State = session.State
        });
    }

    private async Task BroadcastRoomList()
    {
        var rooms = _rooms.GetAvailableRooms();
        await Clients.All.SendAsync("RoomList", rooms);
    }
}
