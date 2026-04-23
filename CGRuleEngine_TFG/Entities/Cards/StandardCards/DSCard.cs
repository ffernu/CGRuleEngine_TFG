using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Cards.DataBase;
using System;
using System.Collections.Generic;
using System.Text;
using CGRuleEngine_TFG.Execution.Effects;

namespace CGRuleEngine_TFG.Entities.Cards.StandardCards
{
    public class DSCard : CardBase
    {
        public DSCard(CardTemplate template, Player owner, int id, EffectFactory effectFactory)
            : base(template, owner, id, effectFactory)
        {

        }
    }
}
