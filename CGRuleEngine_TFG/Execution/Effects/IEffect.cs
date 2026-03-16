using CGRuleEngine_TFG.GameSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Execution.Effects
{
    public interface IEffect
    {
        public String Name { get; }
        public String Description { get; }
        public void execute(EffectContext context, Game gameState);
    }
}
