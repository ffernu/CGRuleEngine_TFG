using CGRuleEngine_TFG.GameSystem;
using CGRuleEngine_TFG.Entities.Players.Controllers.Strategies;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Entities.Players.Controllers
{
    public class AIController
    {
        private IStrategy Strategy { get; set; }

        // Constructor that takes an IStrategy as a parameter and initializes the Strategy property of the AIController with the given strategy
        public AIController(IStrategy strategy)
        {
            this.Strategy = strategy;
        }

        // TODO:
        public void requestAction(Game gameState)
        {
            // TODO: Implement logic to decide which strategy to use based on the game state
        }

        // Method to set a strategy of the AIController
        public void setStrategy(IStrategy strategy)
        {
            this.Strategy = strategy;
        }

        public virtual IStrategy studyStrategy(Game gameState)
        {
            //TODO
            return this.Strategy; //TEMP 
        }
    }
}
