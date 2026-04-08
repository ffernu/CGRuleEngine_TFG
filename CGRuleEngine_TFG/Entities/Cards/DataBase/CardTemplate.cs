using CGRuleEngine_TFG.Execution.Effects;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Entities.Cards.DataBase
{
    public class CardTemplate
    {
        public String Type { get; set; } // Type of the card, e.g., "BasicCard", "DSCard", etc.

        public String Id { get; set; }
        public String Name { get; set; }
        public String SideA { get; set; }
        public String? SideB { get; set; } // Optional, for double-faced cards
        public String? TransformInto { get; set; } // ID of the card this one transforms into, if applicable
        public List<String> Tags { get; set; }
        public Dictionary<String, Object> Properties { get; set; }
        public List<String> Effects { get; set; } // Saving effect IDs instead of actual IEffect objects, to be resolved when instantiating the card
    }
}
