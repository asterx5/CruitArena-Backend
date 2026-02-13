namespace CruitArena.Models;

public class GameState
{
    public string GameId { get; set; } = string.Empty;
    public int CurrentRound { get; set; } = 1;
    public GamePhase CurrentPhase { get; set; } = GamePhase.Play;
    public Player Player1 { get; set; } = new();
    public Player Player2 { get; set; } = new();
    public string? LastAction { get; set; }

    public Player? Winner { get; set; }
    public bool IsGameOver { get; set; } = false;

    public Player GetOpponent(string playerId) =>
        playerId == Player1.Id ? Player2 : Player1;

    public Player? GetPlayer(string playerId) =>
        playerId == Player1.Id ? Player1 :
        playerId == Player2.Id ? Player2 : null;

    public (Player Player, GameCard Card)? FindCard(string cardId)
    {
        var card1 = Player1.FindCard(cardId);
        if (card1 != null) return (Player1, card1);

        var card2 = Player2.FindCard(cardId);
        if (card2 != null) return (Player2, card2);

        return null;
    }
}
