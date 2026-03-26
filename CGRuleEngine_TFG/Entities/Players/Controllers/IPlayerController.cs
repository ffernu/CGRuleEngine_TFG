using CGRuleEngine_TFG.GameSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Entities.Players.Controllers
{
    public interface IPlayerController
    {
        public void RequestAction(Game gameState)
        {
            // TODO: Implement the logic for the player controller to request an action from the player, this could involve displaying a user interface for the player to choose an action or using some kind of AI to make a decision based on the game state
        }
    }
}
