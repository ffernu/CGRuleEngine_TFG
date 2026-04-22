using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Players.Controllers;
using CGRuleEngine_TFG.GameSystem;
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

        // Optional reference to the current game state, which can be used by effects to access and modify the game state as needed. Provided by the ActionStack
        public IGameState? GameState { get; set; }

        public EffectContext(ICard sourceCard, Player sourcePlayer)
        {
            this.SourceCard = sourceCard;
            this.SourcePlayer = sourcePlayer;
        }
    }
}
