using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Execution.Effects;
using CGRuleEngine_TFG.GameSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Execution.Actions
{
    public interface IAction
    {
        public Player Player { get; }
        public String Name { get; }
        public Object Source { get; }
        public List<ITargetable> Targets { get; }

        // Convert the action into a list of effects that can be added to the stack for execution
        public List<IEffect> ToEffects(Game gameState)
        {
            List<IEffect> effects = new List<IEffect>();
            // TODO: Implement logic to convert the action into a list of effects based on the game state and the action's properties.
            return effects;
        }
    }
}
