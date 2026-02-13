using System.Text.Json.Serialization;

namespace CruitArena.Models;

/// <summary>
/// All game actions. Uses System.Text.Json polymorphic serialization
/// so the Kotlin client can send { "type": "PlayCard", "playerId": "...", "cardId": "..." }
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ChangeTier), "ChangeTier")]
[JsonDerivedType(typeof(ChangeArchetype), "ChangeArchetype")]
[JsonDerivedType(typeof(ChangeSpecialty), "ChangeSpecialty")]
[JsonDerivedType(typeof(SendToGraveyard), "SendToGraveyard")]
[JsonDerivedType(typeof(ReturnToHand), "ReturnToHand")]
[JsonDerivedType(typeof(BanishFaceUp), "BanishFaceUp")]
[JsonDerivedType(typeof(BanishFaceDown), "BanishFaceDown")]
[JsonDerivedType(typeof(SetCard), "SetCard")]
[JsonDerivedType(typeof(PlayCard), "PlayCard")]
[JsonDerivedType(typeof(FlipFaceUp), "FlipFaceUp")]
[JsonDerivedType(typeof(FlipFaceDown), "FlipFaceDown")]
[JsonDerivedType(typeof(GiveToOpponent), "GiveToOpponent")]
[JsonDerivedType(typeof(Freeze), "Freeze")]
[JsonDerivedType(typeof(Unfreeze), "Unfreeze")]
[JsonDerivedType(typeof(Negate), "Negate")]
[JsonDerivedType(typeof(UnNegate), "UnNegate")]
[JsonDerivedType(typeof(Nullify), "Nullify")]
[JsonDerivedType(typeof(UnNullify), "UnNullify")]
[JsonDerivedType(typeof(TurnObsolete), "TurnObsolete")]
[JsonDerivedType(typeof(Restore), "Restore")]
[JsonDerivedType(typeof(Reveal), "Reveal")]
[JsonDerivedType(typeof(UnReveal), "UnReveal")]
[JsonDerivedType(typeof(AddProtectionLayer), "AddProtectionLayer")]
[JsonDerivedType(typeof(RemoveProtectionLayer), "RemoveProtectionLayer")]
[JsonDerivedType(typeof(AddDestructionLayer), "AddDestructionLayer")]
[JsonDerivedType(typeof(RemoveDestructionLayer), "RemoveDestructionLayer")]
[JsonDerivedType(typeof(AddNegationLayer), "AddNegationLayer")]
[JsonDerivedType(typeof(RemoveNegationLayer), "RemoveNegationLayer")]
[JsonDerivedType(typeof(AddZeroLayer), "AddZeroLayer")]
[JsonDerivedType(typeof(RemoveZeroLayer), "RemoveZeroLayer")]
[JsonDerivedType(typeof(NextPhase), "NextPhase")]
[JsonDerivedType(typeof(EndTurn), "EndTurn")]
public abstract class GameAction
{
    public string PlayerId { get; set; } = string.Empty;
    public string? CardId { get; set; }
}

// --- Attribute Changes ---

public class ChangeTier : GameAction
{
    public int NewTier { get; set; }
}

public class ChangeArchetype : GameAction
{
    public string NewArchetype { get; set; } = string.Empty;
}

public class ChangeSpecialty : GameAction
{
    public string NewSpecialty { get; set; } = string.Empty;
}

// --- Movement Actions ---

public class SendToGraveyard : GameAction { }
public class ReturnToHand : GameAction { }
public class BanishFaceUp : GameAction { }
public class BanishFaceDown : GameAction { }
public class SetCard : GameAction { }
public class PlayCard : GameAction { }

// --- Flip Actions ---

public class FlipFaceUp : GameAction { }
public class FlipFaceDown : GameAction { }

// --- Transfer ---

public class GiveToOpponent : GameAction { }

// --- Status Effects ---

public class Freeze : GameAction { }
public class Unfreeze : GameAction { }
public class Negate : GameAction { }
public class UnNegate : GameAction { }
public class Nullify : GameAction { }
public class UnNullify : GameAction { }
public class TurnObsolete : GameAction { }
public class Restore : GameAction { }
public class Reveal : GameAction { }
public class UnReveal : GameAction { }

// --- Layer Actions ---

public class AddProtectionLayer : GameAction
{
    public int Amount { get; set; } = 1;
}

public class RemoveProtectionLayer : GameAction
{
    public int Amount { get; set; } = 1;
}

public class AddDestructionLayer : GameAction
{
    public int Amount { get; set; } = 1;
}

public class RemoveDestructionLayer : GameAction
{
    public int Amount { get; set; } = 1;
}

public class AddNegationLayer : GameAction
{
    public int Amount { get; set; } = 1;
}

public class RemoveNegationLayer : GameAction
{
    public int Amount { get; set; } = 1;
}

public class AddZeroLayer : GameAction
{
    public int Amount { get; set; } = 1;
}

public class RemoveZeroLayer : GameAction
{
    public int Amount { get; set; } = 1;
}

// --- Phase/Turn Actions ---

public class NextPhase : GameAction { }
public class EndTurn : GameAction { }
