using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Execution.Actions;
using CGRuleEngine_TFG.GameSystem;
using CGRuleEngine_TFG.Execution.Effects;
using System;
using System.Collections.Generic;

namespace CGRuleEngine_TFG.Execution
{
    public class PlayCardAction : IAction
    {
        public Player SourcePlayer { get; }
        public ICard CardToPlay { get; }

        // Validation and payment logic can be provided as delegates, allowing for flexible and customizable behavior when playing a card
        private readonly Func<Player, ICard, IGameState, bool> _validationLogic;
        private readonly Action<Player, ICard> _paymentLogic;

        // Constructor that initializes the action with the player, card, and optional validation and payment logic
        public PlayCardAction(
            Player player,
            ICard card,
            Func<Player, ICard, IGameState, bool> validationLogic = null,
            Action<Player, ICard> paymentLogic = null)
        {
            SourcePlayer = player;
            CardToPlay = card;
            _validationLogic = validationLogic;
            _paymentLogic = paymentLogic;
        }

        // Checks if the action is valid in the current game state, ensuring that the player has priority and that any additional validation logic
        public bool IsValid(IGameState gameState)
        {
            if (gameState.PriorityPlayer != SourcePlayer) return false;

            if (_validationLogic != null)
            {
                if (!_validationLogic(SourcePlayer, CardToPlay, gameState)) return false;
            }

            return true;
        }

        // Executes the action, applying payment logic, moving the card to the appropriate zone, and triggering any "OnPlay" effects associated with the card
        public void Commit(IActionHandler handler)
        {
            // Apply payment logic if provided
            _paymentLogic?.Invoke(SourcePlayer, CardToPlay);

            // Move the card to the appropriate zone
            if (CardToPlay.CurrentZone != null)
            {
                CardToPlay.CurrentZone.RemoveCard(CardToPlay);
                CardToPlay.CurrentZone = null;
            }

            // Collect "OnPlay" effects to be played
            List<IEffect> effectsToPlay = new List<IEffect>();
            if (CardToPlay.Effects != null) //
            {
                foreach (var effect in CardToPlay.Effects)
                {
                    if (effect.Timing == "OnPlay")
                    {
                        effectsToPlay.Add(effect);
                    }
                }
            }

            // If there are effects to play, create a sequence effect and push it onto the stack with the appropriate context
            if (effectsToPlay.Count > 0)
            {
                IEffect sequence = new SequenceEffect(effectsToPlay);
                EffectContext context = new EffectContext(CardToPlay, SourcePlayer);
                StackEntry entry = new StackEntry { Effect = sequence, Context = context };

                handler.Stack.Push(entry);
            }
        }
    }
}