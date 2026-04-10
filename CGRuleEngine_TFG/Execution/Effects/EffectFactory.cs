using CGRuleEngine_TFG.Entities.Cards.DataBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Execution.Effects
{
    public class EffectFactory
    {
        // Dictionary to store the registered effect builders, where the key is the effect type as a string and the value is a function that takes EffectData and returns an IEffect instance
        private readonly Dictionary<String, Func<EffectData, IEffect>> _builders = new Dictionary<String, Func<EffectData, IEffect>>();

        // Constructor for the EffectFactory, it registers the default effect builders for different effect types such as ModifyPropertyEffect, ModifyTagEffect, DrawEffect, and MoveCardEffect
        public EffectFactory()
        {
            // Registering default effect builders for different effect types
            RegisterBuilder("ModifyPropertyEffect", BuildModifyProperty);
            RegisterBuilder("ModifyTagEffect", BuildModifyTag);
            RegisterBuilder("DrawEffect", BuildDraw);
            RegisterBuilder("MoveCardEffect", BuildMoveCard);
        }

        // Method to register a new effect builder, it takes the effect type as a string and a function that constructs an IEffect instance from EffectData, and adds it to the _builders dictionary
        public void RegisterBuilder(String effectType, Func<EffectData, IEffect> builderFunction)
        {
            _builders[effectType] = builderFunction;
        }

        // Method to create an effect instance based on the provided EffectData, it looks up the effect type in the _builders dictionary and calls the corresponding builder function to create and return an IEffect instance, if the effect type is not registered, it throws an exception
        public IEffect CreateEffect(EffectData data)
        {
            if (_builders.TryGetValue(data.Type, out var constructor))
            {
                return constructor(data);
            }

            throw new Exception($"The effect '{data.Type}' is not registered in the EffectFactory.");
        }

        // Private method to build a ModifyPropertyEffect instance from the provided EffectData, it extracts the necessary parameters such as DebugName, AffectedProperty, Value, and Operation from the EffectData and uses them to construct and return a new ModifyPropertyEffect instance
        private IEffect BuildModifyProperty(EffectData data)
        {
            String debugName = GetParam(data.Parameters, "DebugName", "Modify Property");
            String property = GetParam(data.Parameters, "AffectedProperty", "");
            int value = GetParam(data.Parameters, "Value", 0);

            String opString = GetParam(data.Parameters, "Operation", "Subtract");
            Enum.TryParse(opString, out OperationType operation);

            return new ModifyPropertyEffect(value, property, operation, debugName);
        }

        // Private method to build a ModifyTagEffect instance from the provided EffectData, it extracts the necessary parameters such as DebugName, AffectedTag, and Operation from the EffectData and uses them to construct and return a new ModifyTagEffect instance
        private IEffect BuildModifyTag(EffectData data)
        {
            String debugName = GetParam(data.Parameters, "DebugName", "Modify Tag");
            String tag = GetParam(data.Parameters, "AffectedTag", "");

            String opString = GetParam(data.Parameters, "Operation", "Add");
            Enum.TryParse(opString, out TagAction action);
            
            return new ModifyTagEffect(tag, action, debugName);
        }

        // Private method to build a DrawEffect instance from the provided EffectData, it extracts the necessary parameters such as DebugName, Amount, SourceZoneName, and DestinationZoneName from the EffectData and uses them to construct and return a new DrawEffect instance
        private IEffect BuildDraw(EffectData data)
        {
            String debugName = GetParam(data.Parameters, "DebugName", "Draw");
            int drawAmount = GetParam(data.Parameters, "Amount", 1);
            String source = GetParam(data.Parameters, "SourceZoneName", "Deck");
            String dest = GetParam(data.Parameters, "DestinationZoneName", "Hand");

            return new DrawEffect(drawAmount, source, dest, debugName);
        }

        // Private method to build a MoveCardEffect instance from the provided EffectData, it extracts the necessary parameters such as DebugName and DestinationZoneName from the EffectData and uses them to construct and return a new MoveCardEffect instance
        private IEffect BuildMoveCard(EffectData data)
        {
            String debugName = GetParam(data.Parameters, "DebugName", "Move Card");
            String destinationZone = GetParam(data.Parameters, "DestinationZoneName", "Graveyard");

            return new MoveCardEffect(destinationZone, debugName);
        }

        // Generic private method to extract a parameter of a specified type from the parameters dictionary, it takes the parameters dictionary, the key for the desired parameter, and a default value to return if the key is not found or if the value cannot be converted to the specified type, it returns the extracted parameter value or the default value
        private static T GetParam<T>(Dictionary<string, object> parameters, string key, T defaultValue)
        {
            if (parameters != null && parameters.TryGetValue(key, out object value))
            {
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }
    }
}
