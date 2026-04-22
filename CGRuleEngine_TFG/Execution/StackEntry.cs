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

        // Execute the effect of the stack entry and its context
        public void Resolve(IGameState gameState)
        {
            Context.GameState = gameState;
            Effect.execute(this.Context);
        }
    }
}
