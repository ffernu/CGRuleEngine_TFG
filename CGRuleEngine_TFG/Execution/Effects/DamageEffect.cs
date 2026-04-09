using CGRuleEngine_TFG.GameSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Execution.Effects
{
    public class DamageEffect : IEffect
    {
        public String Name { get; }
        public String Description { get; }
        public int Amount { get; set; }

        public DamageEffect(String name, String description, int amount)
        {
            Name = name;
            Description = description;
            Amount = amount;
        }

        public void execute(EffectContext context, Game gameState)
        {

        }
    }
}
