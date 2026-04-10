using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Players.Controllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Execution
{
    public class EffectContext
    {

        public ICard SourceCard { get; set; }
        public Player SourcePlayer { get; set; }
        public List<ITargetable> Targets { get; set; } = new List<ITargetable>();

        public EffectContext(ICard sourceCard, Player sourcePlayer) //TODO: add GameState to the context, so effects can access it if needed
        {
            this.SourceCard = sourceCard;
            this.SourcePlayer = sourcePlayer;
        }
    }
}
