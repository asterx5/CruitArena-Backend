using System.Text.Json.Serialization;

namespace CruitArena.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GamePhase
{
    Play,
    FaceUp,
    Spell,
    Flip,
    Intermediate,
    Critical,
    Prebattle,
    Battle,
    PostBattle,
    CoolDown,
    END
}
