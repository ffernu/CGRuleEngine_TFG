using CGRuleEngine_TFG.GameSystem;
using CGRuleEngine_TFG.Execution.Actions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Entities.Players.Controllers
{
    public class HumanController : IPlayerController
    {
        public bool IsWaitingInput { get; set; }
        public IAction CurrentAction { get; set; }

        // Constructor that initializes the IsWaitingInput property to false and the CurrentAction property to null
        public HumanController()
        {
            IsWaitingInput = false;
            CurrentAction = null;
        }

        // TODO:
        public void requestAction(Game gameState)
        {
            // TODO: Implement the logic for the human controller to request an action from the player, this could involve displaying a user interface for the player to choose an action or using some kind of AI to make a decision based on the game state
        }

        // TODO:
        public void UIInputReceived(IAction action)
        {
            // TODO: Implement the logic for handling the input received from the user interface, this could involve validating the action and updating the CurrentAction property accordingly
        }
    }
}
