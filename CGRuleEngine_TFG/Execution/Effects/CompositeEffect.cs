using CGRuleEngine_TFG.GameSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CGRuleEngine_TFG.Execution.Effects
{
    public class CompositeEffect : IEffect
    {
        public String Name { get; set; }
        public String Description { get; set; }
        public List<IEffect> Children { get; set; } = new List<IEffect>();

        // Add a child effect to the composite effect
        public void Add(IEffect effect)
        {
            Children.Add(effect);
        }

        //Execute all child effects in the composite effect
        public void execute(EffectContext context, Game gameState)
        {
            foreach (IEffect child in this.Children)
            {
                child.execute(context, gameState);
            }
        }
    }
}
