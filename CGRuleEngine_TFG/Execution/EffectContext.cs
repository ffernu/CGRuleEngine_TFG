using CGRuleEngine_TFG.Entities.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Execution
{
    public class EffectContext
    {

        public Card SourceCard { get; set; }
        public List<ITargetable> Targets { get; set; } = new List<ITargetable>();
    }
}
