using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Execution.Effects;
using CGRuleEngine_TFG.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Buffers;
using CGRuleEngine_TFG.Entities.Cards.DataBase;

namespace CGRuleEngine_TFG.Entities.Cards
{
    public class NormalCard : CardBase
    {

        // Constructor that takes a CardTemplate, a Player, and an instance ID as parameters and calls the base constructor to initialize the properties of the NormalCard class with the values from the template and the owner
        public NormalCard(CardTemplate template, Player owner, int instanceId, EffectFactory effectFactory)
            : base(template, owner, instanceId, effectFactory)
        {
            
        }
    }
}
