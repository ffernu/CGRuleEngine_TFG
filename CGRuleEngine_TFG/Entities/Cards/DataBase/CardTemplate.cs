using CGRuleEngine_TFG.Execution.Effects;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Entities.Cards.DataBase
{
    public class EffectData
    {
        public string Type { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }

    public class CardTemplate
    {
        public String Type { get; set; } // Type of the card, e.g., "NormalCard", "DSCard", etc.

        public String Id { get; set; }
        public String Name { get; set; }
        public String SideA { get; set; }
        public String? SideB { get; set; } // Optional, for double-faced cards
        public String? TransformInto { get; set; } // ID of the card this one transforms into, if applicable
        public List<String> Tags { get; set; }
        public Dictionary<String, Object> Properties { get; set; }
        public List<EffectData> Effects { get; set; } // List of effects associated with the card, represented as EffectData objects containing the type and parameters of each effect
    }
}
