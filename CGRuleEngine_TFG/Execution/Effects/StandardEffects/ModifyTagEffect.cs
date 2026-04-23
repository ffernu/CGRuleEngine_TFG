using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Execution.Effects.StandardEffects
{
    // Enum to represent the type of action that can be performed on a tag, such as adding or removing a tag
    public enum TagAction
    {
        Add,
        Remove
    }
    public class ModifyTagEffect : IEffect
    {
        public String? DebugName { get; set; }

        public String AffectedTag { get; }
        public TagAction Action { get; set; }

        // Constructor for the ModifyTagEffect, initializing the value to be applied and the Tag that will be affected
        public ModifyTagEffect(String tag, TagAction action, String? debugName)
        {
            this.DebugName = debugName;

            this.AffectedTag = tag;
            this.Action = action;
        }

        // Method to execute the effect, which iterates through the targets in the context and applies the specified action to the affected tag of each target
        public void execute(EffectContext context)
        {
            foreach (ITargetable target in context.Targets)
            {
                if (this.Action == TagAction.Add)
                {
                    target.AddTag(this.AffectedTag);
                }
                else if (this.Action == TagAction.Remove)
                {
                    target.RemoveTag(this.AffectedTag);
                }
            }
        }
    }
}
