using CGRuleEngine_TFG.GameSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Execution.Effects
{
    public interface IEffect
    {
        public String? DebugName { get; set; }
        public void execute(EffectContext context);
    }
}
