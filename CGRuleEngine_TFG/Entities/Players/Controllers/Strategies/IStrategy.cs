using CGRuleEngine_TFG.GameSystem;
using CGRuleEngine_TFG.Execution.Actions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Entities.Players.Controllers.Strategies
{
    public interface IStrategy
    {
        public IAction decideAction(List<Action> actions, Game gameState);
    }
}
