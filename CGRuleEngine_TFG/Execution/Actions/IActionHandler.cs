using CGRuleEngine_TFG.Entities.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Execution.Actions
{
    public interface IActionHandler
    {
        ActionStack Stack { get; }

        void PassPriority(Player player);

        void EndGame(Player loser);
    }
}
