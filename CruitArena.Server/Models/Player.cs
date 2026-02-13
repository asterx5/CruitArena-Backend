namespace CruitArena.Models;

public class Player
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<GameCard> Cards { get; set; } = new();

    // Zone accessors
    public List<GameCard> GetHand() => Cards.Where(c => c.Location == GameCard.LocationHand).ToList();
    public List<GameCard> GetField() => Cards.Where(c => c.Location == GameCard.LocationField).ToList();
    public List<GameCard> GetGraveyard() => Cards.Where(c => c.Location == GameCard.LocationGraveyard).ToList();
    public List<GameCard> GetPhantomZone() => Cards.Where(c => c.Location == GameCard.LocationPhantomZone).ToList();

    public GameCard? FindCard(string cardId) => Cards.FirstOrDefault(c => c.Id == cardId);

    public void AddCard(GameCard card) => Cards.Add(card);

    public GameCard? RemoveCard(string cardId)
    {
        var card = FindCard(cardId);
        if (card != null)
            Cards.Remove(card);
        return card;
    }
}
