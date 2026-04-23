using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Execution.Actions;
using CGRuleEngine_TFG.GameSystem;

namespace CGRuleEngine_TFG.Execution
{
    public class PassPriorityAction : IAction
    {
        public Player SourcePlayer { get; }

        public PassPriorityAction(Player player)
        {
            SourcePlayer = player;
        }

        // This method checks if the action is valid, it checks if the player who is trying to pass priority is the actual priority player
        public bool IsValid(IGameState gameState)
        {
            return gameState.PriorityPlayer == SourcePlayer;
        }

        // This method executes the action, it calls the PassPriority method of the handler
        public void Commit(IActionHandler handler)
        {
            handler.PassPriority(SourcePlayer);
        }
    }
}