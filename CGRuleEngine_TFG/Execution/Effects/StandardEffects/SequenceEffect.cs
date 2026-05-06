using System.Collections.Generic;

namespace CGRuleEngine_TFG.Execution.Effects
{
    public class SequenceEffect : IEffect
    {
        public string? DebugName { get; set; }
        public string Timing { get; set; }

        private readonly List<IEffect> _effects;

        // Constructor that initializes the sequence effect with a list of effects to be executed in order
        public SequenceEffect(List<IEffect> effects)
        {
            this._effects = effects ?? new List<IEffect>();
            this.DebugName = "Sequence Effect";
            this.Timing = "Sequence";
        }

        // Executes each effect in the sequence in order, passing the same context to each effect
        public void execute(EffectContext context)
        {
            foreach (var effect in _effects)
            {
                effect.execute(context);
            }
        }
    }
}