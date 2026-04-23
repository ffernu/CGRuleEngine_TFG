using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.GameSystem.TurnSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.GameSystem
{
    public interface IGameState
    {
        // IReadOnlyList is used here to allow the Players property to be accessed as a read-only list when accessed through an IGameState reference without preventing the Players property from being modified as a List<Player> when accessed through a Game reference, this allows for more flexibility in how the Players property can be used while still enforcing immutability when accessed through an IGameState reference
        IReadOnlyList<Player> Players { get; }
        Player? ActivePlayer { get; }
        Player? PriorityPlayer { get; }
        Turn? CurrentTurn { get; }
        int TurnNumber { get; }
    }
}
