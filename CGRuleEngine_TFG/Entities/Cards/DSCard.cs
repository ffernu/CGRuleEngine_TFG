using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Cards.DataBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Entities.Cards
{
    public class DSCard : CardBase
    {
        public DSCard(CardTemplate template, Player owner, int id)
            : base(template, owner, id)
        {

        }
    }
}
