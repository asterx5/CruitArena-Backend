namespace CruitArena.Models;

public class GameEngine
{
    public void ApplyAction(GameState state, GameAction action)
    {
        switch (action)
        {
            case ChangeTier a:
                HandleChangeTier(state, a);
                break;
            case ChangeArchetype a:
                HandleChangeArchetype(state, a);
                break;
            case ChangeSpecialty a:
                HandleChangeSpecialty(state, a);
                break;
            case SendToGraveyard a:
                HandleSendToGraveyard(state, a);
                break;
            case ReturnToHand a:
                HandleReturnToHand(state, a);
                break;
            case BanishFaceUp a:
                HandleBanishFaceUp(state, a);
                break;
            case BanishFaceDown a:
                HandleBanishFaceDown(state, a);
                break;
            case SetCard a:
                HandleSetCard(state, a);
                break;
            case PlayCard a:
                HandlePlayCard(state, a);
                break;
            case FlipFaceUp a:
                HandleFlipFaceUp(state, a);
                break;
            case FlipFaceDown a:
                HandleFlipFaceDown(state, a);
                break;
            case GiveToOpponent a:
                HandleGiveToOpponent(state, a);
                break;
            case Freeze a:
                HandleFreeze(state, a);
                break;
            case Unfreeze a:
                HandleUnfreeze(state, a);
                break;
            case Negate a:
                HandleNegate(state, a);
                break;
            case UnNegate a:
                HandleUnNegate(state, a);
                break;
            case Nullify a:
                HandleNullify(state, a);
                break;
            case UnNullify a:
                HandleUnNullify(state, a);
                break;
            case TurnObsolete a:
                HandleTurnObsolete(state, a);
                break;
            case Restore a:
                HandleRestore(state, a);
                break;
            case Reveal a:
                HandleReveal(state, a);
                break;
            case UnReveal a:
                HandleUnReveal(state, a);
                break;
            case AddProtectionLayer a:
                HandleAddProtectionLayer(state, a);
                break;
            case RemoveProtectionLayer a:
                HandleRemoveProtectionLayer(state, a);
                break;
            case AddDestructionLayer a:
                HandleAddDestructionLayer(state, a);
                break;
            case RemoveDestructionLayer a:
                HandleRemoveDestructionLayer(state, a);
                break;
            case AddNegationLayer a:
                HandleAddNegationLayer(state, a);
                break;
            case RemoveNegationLayer a:
                HandleRemoveNegationLayer(state, a);
                break;
            case AddZeroLayer a:
                HandleAddZeroLayer(state, a);
                break;
            case RemoveZeroLayer a:
                HandleRemoveZeroLayer(state, a);
                break;
            case NextPhase a:
                HandleNextPhase(state, a);
                break;
            case EndTurn a:
                HandleEndTurn(state, a);
                break;
        }
    }

    // --- Attribute Changes ---

    private void HandleChangeTier(GameState state, ChangeTier action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.Tier = action.NewTier;
        state.LastAction = $"{player.Name} changed {card.Name}'s tier to {action.NewTier}";
    }

    private void HandleChangeArchetype(GameState state, ChangeArchetype action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.Archetype = action.NewArchetype;
        state.LastAction = $"{player.Name} changed {card.Name}'s archetype to {action.NewArchetype}";
    }

    private void HandleChangeSpecialty(GameState state, ChangeSpecialty action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.Specialty = action.NewSpecialty;
        state.LastAction = $"{player.Name} changed {card.Name}'s specialty to {action.NewSpecialty}";
    }

    // --- Movement ---

    private void HandleSendToGraveyard(GameState state, SendToGraveyard action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.Location = GameCard.LocationGraveyard;
        card.IsFaceUp = true;
        // Check if card id is 1 and if true set the opponent as winner
        if (card.Id == "1")
        {
            var opponent = state.GetOpponent(player.Id);
            state.Winner = opponent;
            state.IsGameOver = true;
        }
        state.LastAction = $"{player.Name} sent {card.Name} to the graveyard";
    }

    private void HandleReturnToHand(GameState state, ReturnToHand action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.Location = GameCard.LocationHand;
        card.IsFaceUp = true;
        state.LastAction = $"{player.Name} returned {card.Name} to hand";
    }

    private void HandleBanishFaceUp(GameState state, BanishFaceUp action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.Location = GameCard.LocationPhantomZone;
        card.IsFaceUp = true;
        state.LastAction = $"{player.Name} banished {card.Name} face-up";
    }

    private void HandleBanishFaceDown(GameState state, BanishFaceDown action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.Location = GameCard.LocationPhantomZone;
        card.IsFaceUp = false;
        state.LastAction = $"{player.Name} banished a card face-down";
    }

    private void HandleSetCard(GameState state, SetCard action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.Location = GameCard.LocationField;
        card.IsFaceUp = false;
        state.LastAction = $"{player.Name} set a card face-down";
    }

    private void HandlePlayCard(GameState state, PlayCard action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.Location = GameCard.LocationField;
        card.IsFaceUp = true;
        state.LastAction = $"{player.Name} played {card.Name}";
    }

    // --- Flip ---

    private void HandleFlipFaceUp(GameState state, FlipFaceUp action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.IsFaceUp = true;
        state.LastAction = $"{player.Name} flipped {card.Name} face-up";
    }

    private void HandleFlipFaceDown(GameState state, FlipFaceDown action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.IsFaceUp = false;
        state.LastAction = $"{player.Name} flipped a card face-down";
    }

    // --- Transfer ---

    private void HandleGiveToOpponent(GameState state, GiveToOpponent action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (currentPlayer, card) = result.Value;
        var opponent = state.GetOpponent(action.PlayerId);

        currentPlayer.RemoveCard(action.CardId!);
        opponent.AddCard(card);
        state.LastAction = $"{currentPlayer.Name} gave {card.Name} to {opponent.Name}";
    }

    // --- Status Effects ---

    private void HandleFreeze(GameState state, Freeze action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.IsFrozen = true;
        state.LastAction = $"{player.Name} froze {card.Name}";
    }

    private void HandleUnfreeze(GameState state, Unfreeze action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.IsFrozen = false;
        state.LastAction = $"{player.Name} unfroze {card.Name}";
    }

    private void HandleNegate(GameState state, Negate action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.IsNegated = true;
        state.LastAction = $"{player.Name} negated {card.Name}";
    }

    private void HandleUnNegate(GameState state, UnNegate action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.IsNegated = false;
        state.LastAction = $"{player.Name} un-negated {card.Name}";
    }

    private void HandleNullify(GameState state, Nullify action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.IsNullified = true;
        state.LastAction = $"{player.Name} nullified {card.Name}";
    }

    private void HandleUnNullify(GameState state, UnNullify action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.IsNullified = false;
        state.LastAction = $"{player.Name} un-nullified {card.Name}";
    }

    private void HandleTurnObsolete(GameState state, TurnObsolete action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        if (card.Type == "monster") card.Tier = 0;
        card.IsNullified = true;
        card.IsNegated = true;
        state.LastAction = $"{player.Name} turned {card.Name} obsolete";
    }

    private void HandleReveal(GameState state, Reveal action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.IsRevealed = true;
        state.LastAction = $"{player.Name} revealed {card.Name}";
    }

    private void HandleUnReveal(GameState state, UnReveal action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.IsRevealed = false;
        state.LastAction = $"{player.Name} un-revealed {card.Name}";
    }

    // --- Layer Actions ---

    private void HandleAddProtectionLayer(GameState state, AddProtectionLayer action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.ProtectionLayer += action.Amount;
        state.LastAction = $"{player.Name} added {action.Amount} protection layer(s) to {card.Name}";
    }

    private void HandleRemoveProtectionLayer(GameState state, RemoveProtectionLayer action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.ProtectionLayer = Math.Max(0, card.ProtectionLayer - action.Amount);
        state.LastAction = $"{player.Name} removed {action.Amount} protection layer(s) from {card.Name}";
    }

    private void HandleAddDestructionLayer(GameState state, AddDestructionLayer action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.DestructionLayer += action.Amount;
        state.LastAction = $"{player.Name} added {action.Amount} destruction layer(s) to {card.Name}";
    }

    private void HandleRemoveDestructionLayer(GameState state, RemoveDestructionLayer action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.DestructionLayer = Math.Max(0, card.DestructionLayer - action.Amount);
        state.LastAction = $"{player.Name} removed {action.Amount} destruction layer(s) from {card.Name}";
    }

    private void HandleAddNegationLayer(GameState state, AddNegationLayer action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.NegationLayer += action.Amount;
        state.LastAction = $"{player.Name} added {action.Amount} negation layer(s) to {card.Name}";
    }

    private void HandleRemoveNegationLayer(GameState state, RemoveNegationLayer action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.NegationLayer = Math.Max(0, card.NegationLayer - action.Amount);
        state.LastAction = $"{player.Name} removed {action.Amount} negation layer(s) from {card.Name}";
    }

    private void HandleAddZeroLayer(GameState state, AddZeroLayer action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.ZeroLayer += action.Amount;
        state.LastAction = $"{player.Name} added {action.Amount} zero layer(s) to {card.Name}";
    }

    private void HandleRemoveZeroLayer(GameState state, RemoveZeroLayer action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;
        card.ZeroLayer = Math.Max(0, card.ZeroLayer - action.Amount);
        state.LastAction = $"{player.Name} removed {action.Amount} zero layer(s) from {card.Name}";
    }

    // --- Restore (resets card to original CardData values) ---

    private void HandleRestore(GameState state, Restore action)
    {
        var result = state.FindCard(action.CardId!);
        if (result == null) return;
        var (player, card) = result.Value;

        // Find the original CardData from the stored originals
        var original = _originalCards.GetValueOrDefault(card.CardDataId);
        if (original == null) return;

        card.Description = original.Description;
        card.Tier = original.Tier;
        card.Archetype = original.Archetype;
        card.Specialty = original.Specialty;
        card.Priority = original.Priority;

        card.IsFrozen = false;
        card.IsNegated = false;
        card.IsNullified = false;
        card.IsRevealed = false;

        card.ProtectionLayer = 0;
        card.DestructionLayer = 0;
        card.NegationLayer = 0;
        card.ZeroLayer = 0;

        state.LastAction = $"{player.Name} restored {card.Name} to its original state";
    }

    // --- Phase / Turn ---

    private void HandleNextPhase(GameState state, NextPhase action)
    {
        state.CurrentPhase = state.CurrentPhase switch
        {
            GamePhase.Play => GamePhase.FaceUp,
            GamePhase.FaceUp => GamePhase.Spell,
            GamePhase.Spell => GamePhase.Flip,
            GamePhase.Flip => GamePhase.Intermediate,
            GamePhase.Intermediate => GamePhase.Critical,
            GamePhase.Critical => GamePhase.Prebattle,
            GamePhase.Prebattle => GamePhase.Battle,
            GamePhase.Battle => GamePhase.PostBattle,
            GamePhase.PostBattle => GamePhase.CoolDown,
            GamePhase.CoolDown => GamePhase.END,
            GamePhase.END => GamePhase.Play,
            _ => GamePhase.Play
        };
        state.LastAction = $"Moved to {state.CurrentPhase} phase";
    }

    private void HandleEndTurn(GameState state, EndTurn action)
    {
        state.LastAction = $"Round {state.CurrentRound} ended";
        state.CurrentPhase = GamePhase.Play;
        state.CurrentRound += 1;
    }

    // --- Original card storage for Restore ---

    private readonly Dictionary<string, CardData> _originalCards = new();

    /// <summary>
    /// Call this when creating a game to store original card data for Restore actions.
    /// </summary>
    public void RegisterOriginalCards(List<CardData> cards)
    {
        foreach (var card in cards)
        {
            _originalCards.TryAdd(card.Id, card);
        }
    }
}
