using CGRuleEngine_TFG.Execution.Effects;
using CGRuleEngine_TFG.GameSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Execution
{
    public class StackEntry
    {
        public IEffect Effect { get; set; }
        public EffectContext Context { get; set; }

        // Execute the effect of the stack entry and its context, modifying the game state as necessary
        public void Resolve(Game gameState)
        {
            Effect.execute(this.Context, gameState);
        }
    }
}
