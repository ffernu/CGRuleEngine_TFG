using CGRuleEngine_TFG.GameSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Execution.Effects
{
    // Enum to represent the type of operation that can be performed on a property, such as adding, subtracting, or setting a value
    public enum OperationType
    {
        Add,
        Subtract,
        Set
    }
    public class ModifyPropertyEffect : IEffect
    {
        public string? DebugName { get; set; }

        public String AffectedProperty { get; }
        public int Value { get; set; }
        public OperationType Operation { get; set; }

        // Constructor for the ModifyPropertyEffect, initializing the value to be applied, the property that will be affected, and the type of operation to be performed on the property
        public ModifyPropertyEffect(int value, String property, OperationType operation, String? debugName)
        {
            this.DebugName = debugName;

            this.Value = value;
            this.AffectedProperty = property;
            this.Operation = operation;
        }

        // Method to execute the effect, which iterates through the targets in the context and applies the specified operation to the affected property of each target
        public void execute(EffectContext context)
        {
            foreach (ITargetable target in context.Targets)
            {
                if (target.TryGetProperty<int>(this.AffectedProperty, out int statActual))
                {
                    int finalValue = statActual;

                    switch (this.Operation)
                    {
                        case OperationType.Add:
                            finalValue += this.Value;
                            break;
                        case OperationType.Subtract:
                            finalValue -= this.Value;
                            break;
                        case OperationType.Set:
                            finalValue = this.Value;
                            break;
                    }

                    target.SetProperty(this.AffectedProperty, finalValue);
                }
            }
        }
    }
}
