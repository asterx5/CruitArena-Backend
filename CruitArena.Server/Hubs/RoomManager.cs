using System.Collections.Concurrent;
using CruitArena.Models;

namespace CruitArena.Services;

/// <summary>
/// Thread-safe in-memory room management for lobbies.
/// </summary>
public class RoomManager
{
    private readonly ConcurrentDictionary<string, GameRoom> _rooms = new();

    public GameRoom CreateRoom(string connectionId, string hostName, string masterRuleId)
    {
        var existingRoom = _rooms.Values.FirstOrDefault(r => r.HostConnectionId == connectionId);
        if (existingRoom != null)
        {
            // remove any existing room for this host (e.g. from a previous disconnect)
            _rooms.TryRemove(existingRoom.Id, out _);
        }
        var room = new GameRoom
        {
            HostConnectionId = connectionId,
            HostName = hostName,
            MasterRuleId = masterRuleId
        };
        _rooms[room.Id] = room;
        return room;
    }

    public GameRoom? GetRoom(string roomId) =>
        _rooms.GetValueOrDefault(roomId);

    public bool TryJoinRoom(string roomId, string connectionId, string guestName)
    {
        if (!_rooms.TryGetValue(roomId, out var room)) return false;
        if (room.IsFull || room.IsStarted) return false;

        room.GuestConnectionId = connectionId;
        room.GuestName = guestName;
        return true;
    }

    public void SetDeck(string roomId, string connectionId, List<CardData> deck)
    {
        if (!_rooms.TryGetValue(roomId, out var room)) return;

        if (room.HostConnectionId == connectionId)
        {
            if (room.HostDeck != null)
            {
                return;
            }
            room.HostDeck = deck;
        }
        else if (room.GuestConnectionId == connectionId)
        {
            if (room.GuestDeck != null)
            {
                return;
            }
            room.GuestDeck = deck;
        }
    }

    public void SetReady(string roomId, string connectionId)
    {
        if (!_rooms.TryGetValue(roomId, out var room)) return;

        if (room.HostConnectionId == connectionId)
            room.HostReady = true;
        else if (room.GuestConnectionId == connectionId)
            room.GuestReady = true;
    }

    public void MarkStarted(string roomId)
    {
        if (_rooms.TryGetValue(roomId, out var room))
            room.IsStarted = true;
    }

    public void RemoveRoom(string roomId) =>
        _rooms.TryRemove(roomId, out _);

    /// <summary>
    /// Find which room a connection belongs to (for disconnect cleanup)
    /// </summary>
    public GameRoom? FindRoomByConnection(string connectionId) =>
        _rooms.Values.FirstOrDefault(r =>
            r.HostConnectionId == connectionId || r.GuestConnectionId == connectionId);

    /// <summary>
    /// Get all joinable rooms for the lobby list
    /// </summary>
    public List<RoomInfo> GetAvailableRooms() =>
        _rooms.Values
            .Where(r => !r.IsFull || r.HostConnectionId == string.Empty) // && !r.IsStarted
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new RoomInfo
            {
                Id = r.Id,
                HostName = r.HostName,
                IsFull = r.IsFull,
                IsStarted = r.IsStarted,
                CreatedAt = r.CreatedAt,
                MasterRuleId = r.MasterRuleId
            })
            .ToList();
}
