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
        public List<ITargetable> Targets { get; }

        // Validation and payment logic can be provided as delegates, allowing for flexible and customizable behavior when playing a card
        private readonly Func<Player, ICard, IGameState, bool> _validationLogic;
        private readonly Action<Player, ICard> _paymentLogic;

        // Constructor that initializes the action with the player, card, and optional validation and payment logic
        public PlayCardAction(
            Player player,
            ICard card,
            List<ITargetable> targets,
        Func<Player, ICard, IGameState, bool> validationLogic = null,
            Action<Player, ICard> paymentLogic = null)
        {
            this.SourcePlayer = player;
            this.CardToPlay = card;
            this.Targets = targets ?? new List<ITargetable>();
            this._validationLogic = validationLogic;
            this._paymentLogic = paymentLogic;
        }

        // Checks if the action is valid in the current game state, ensuring that the player has priority and that any additional validation logic
        public bool IsValid(IGameState gameState)
        {
            if (gameState.PriorityPlayer != this.SourcePlayer) return false;

            if (this._validationLogic != null)
            {
                if (!_validationLogic(this.SourcePlayer, this.CardToPlay, gameState)) return false;
            }

            return true;
        }

        // Executes the action, applying payment logic, moving the card to the appropriate zone, and triggering any "OnPlay" effects associated with the card
        public void Commit(IActionHandler handler)
        {
            // Apply payment logic if provided
            this._paymentLogic?.Invoke(this.SourcePlayer, this.CardToPlay);

            // Move the card to the appropriate zone
            if (this.CardToPlay.CurrentZone != null)
            {
                this.CardToPlay.CurrentZone.RemoveCard(this.CardToPlay);
                this.CardToPlay.CurrentZone = null;
            }

            // Collect "OnPlay" effects to be played
            List<IEffect> effectsToPlay = new List<IEffect>();
            if (this.CardToPlay.Effects != null) //
            {
                foreach (var effect in this.CardToPlay.Effects)
                {
                    if (effect.Timing == "OnPlay")
                    {
                        effectsToPlay.Add(effect);
                    }
                }
            }

            // If there are effects to play, push the effect or create a sequence effect and push it onto the stack with the appropriate context
            if (effectsToPlay.Count > 0)
            {
                IEffect effectToPush = effectsToPlay.Count == 1
                    ? effectsToPlay[0]
                    : new SequenceEffect(effectsToPlay);

                EffectContext context = new EffectContext(this.CardToPlay, this.SourcePlayer)
                {
                    Targets = this.Targets
                };
                StackEntry entry = new StackEntry { Effect = effectToPush, Context = context };

                handler.Stack.Push(entry);
            }
        }
    }
}