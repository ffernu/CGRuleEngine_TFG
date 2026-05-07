using CGRuleEngine_TFG.GameSystem;
using CGRuleEngine_TFG.Execution.Actions;
using System.Collections.Generic;

namespace CGRuleEngine_TFG.Entities.Players.Controllers.Strategies
{
    public interface IStrategy
    {
        public IAction decideAction(List<IAction> actions, Game gameState);
    }
}