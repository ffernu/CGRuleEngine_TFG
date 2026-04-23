using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.GameSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Execution.Actions
{
    public interface IAction
    {
        // The source player of the action, this property is used to identify the player who is performing the action
        Player SourcePlayer { get; }

        // Checks if the action is valid in the current game state, this method is used to determine if the action can be performed or not
        bool IsValid(IGameState gameState);

        // The method that will execute the action, this method is used to perform the action and move its effects to the stack
        void Commit(IActionHandler handler);
    }
}
