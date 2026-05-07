using CGRuleEngine_TFG.GameSystem;
using CGRuleEngine_TFG.Execution.Actions;
using CGRuleEngine_TFG.Execution;
using System;

namespace CGRuleEngine_TFG.Entities.Players.Controllers
{
    public class HumanController : IPlayerController
    {
        public bool IsWaitingInput { get; set; }
        public IAction CurrentAction { get; set; }

        // Reference to the current game state, which can be used to validate and execute actions received from the user interface
        private Game _currentGameRef;

        // Constructor that initializes the human controller, setting the waiting input flag to false and the current action to null
        public HumanController()
        {
            IsWaitingInput = false;
            CurrentAction = null;
        }

        // Method to request an action from the player, which sets the waiting input flag to true and stores a reference to the current game state for later validation and execution of actions received from the user interface
        public void RequestAction(Game gameState)
        {
            this._currentGameRef = gameState; 
            this.IsWaitingInput = true;
        }

        // Method to receive an action from the user interface, which checks if the controller is currently waiting for input and if the received action is valid in the context of the current game state. If the action is valid, it commits the action to the game state and resets the waiting input flag and current action.
        public void UIInputReceived(IAction action)
        {
            if (!IsWaitingInput) return;

            // Validate the received action against the current game state reference. If the action is valid, commit it to the game state and reset the waiting input flag and current action.
            if (action.IsValid(_currentGameRef))
            {
                this.IsWaitingInput = false;
                this.CurrentAction = action;

                this.CurrentAction.Commit(_currentGameRef);

                this.CurrentAction = null;
            }

            _currentGameRef.NotifyPriorityPlayer();
        }
    }
}