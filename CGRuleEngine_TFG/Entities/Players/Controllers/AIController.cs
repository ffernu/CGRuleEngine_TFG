using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Players.Controllers.Strategies;
using CGRuleEngine_TFG.Execution;
using CGRuleEngine_TFG.Execution.Actions;
using CGRuleEngine_TFG.GameSystem;
using System;
using System.Collections.Generic;

namespace CGRuleEngine_TFG.Entities.Players.Controllers
{
    public class AIController : IPlayerController
    {
        private Player ControlledPlayer { get; set; }
        private IStrategy Strategy { get; set; }

        // Function that generates the list of possible actions for the AI to choose from, it takes the current game state and the controlled player as parameters and returns a list of valid actions that the AI can choose from. This allows for flexibility in how the possible actions are generated, as different implementations of this function can be used to create different types of AI behavior.
        private Func<Game, Player, List<IAction>> ActionGenerator { get; set; }

        // Constructor that initializes the AI controller with a reference to the controlled player, a strategy for deciding which action to take, and a function for generating the list of possible actions. This allows for flexibility in how the AI makes decisions, as different strategies and action generators can be used to create different types of AI behavior.
        public AIController(
            Player player,
            IStrategy strategy,
            Func<Game, Player, List<IAction>> actionGenerator)
        {
            this.ControlledPlayer = player;
            this.Strategy = strategy;
            this.ActionGenerator = actionGenerator;
        }

        // Method to request an action from the AI, this method generates the list of possible actions using the provided action generator function, then uses the strategy to decide which action to take from the list of possible actions. If the chosen action is valid in the context of the current game state, it commits the action to the game state. If no valid action is chosen, it commits a PassPriorityAction for the controlled player. Finally, it notifies the game state that the priority player has been updated.
        public void RequestAction(Game gameState)
        {
            List<IAction> possibleActions = this.ActionGenerator(gameState, this.ControlledPlayer);
            IAction chosenAction = this.Strategy.decideAction(possibleActions, gameState);
            if (chosenAction != null && chosenAction.IsValid(gameState))
            {
                chosenAction.Commit(gameState);
            }
            else
            {
                new PassPriorityAction(this.ControlledPlayer).Commit(gameState);
            }
            gameState.NotifyPriorityPlayer();
        }

        // Method to set the strategy of the AI controller, this allows for changing the behavior of the AI at runtime by providing a new strategy implementation.
        public void setStrategy(IStrategy strategy)
        {
            this.Strategy = strategy;
        }

        // Method to study the strategy of the AI controller, this method can be used to analyze the current strategy and make adjustments if necessary. In this implementation, it simply returns the current strategy, but it can be extended to include more complex behavior such as learning from past actions or adapting to the opponent's strategy.
        public virtual IStrategy studyStrategy(Game gameState)
        {
            return this.Strategy;
        }
    }
}