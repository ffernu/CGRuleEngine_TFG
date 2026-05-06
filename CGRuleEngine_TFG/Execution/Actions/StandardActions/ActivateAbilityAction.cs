using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Execution.Actions;
using CGRuleEngine_TFG.GameSystem;
using CGRuleEngine_TFG.Execution.Effects;
using System;
using System.Collections.Generic;

namespace CGRuleEngine_TFG.Execution
{
    public class ActivateAbilityAction : IAction
    {
        public Player SourcePlayer { get; }
        public ICard SourceCard { get; }
        public List<ITargetable> Targets { get; }

        private readonly Func<Player, ICard, IGameState, bool> _validationLogic;
        private readonly Action<Player, ICard> _paymentLogic;

        public ActivateAbilityAction(
            Player player,
            ICard card,
            List<ITargetable> targets,
            Func<Player, ICard, IGameState, bool> validationLogic = null,
            Action<Player, ICard> paymentLogic = null)
        {
            this.SourcePlayer = player;
            this.SourceCard = card;
            this.Targets = targets ?? new List<ITargetable>();
            this._validationLogic = validationLogic;
            this._paymentLogic = paymentLogic;
        }

        // Checks if the action is valid in the current game state, ensuring that the player has priority and that any additional validation logic specific to the activated ability is satisfied
        public bool IsValid(IGameState gameState)
        {
            if (gameState.PriorityPlayer != this.SourcePlayer) return false;

            if (this._validationLogic != null)
            {
                if (!_validationLogic(this.SourcePlayer, this.SourceCard, gameState)) return false;
            }

            return true;
        }

        // Executes the action, applying payment logic and pushing the activated ability's effects onto the stack
        public void Commit(IActionHandler handler)
        {
            this._paymentLogic?.Invoke(this.SourcePlayer, this.SourceCard);

            // Collect all effects from the card that match the activated ability's timing
            List<IEffect> effectsToPlay = new List<IEffect>();
            if (this.SourceCard.Effects != null)
            {
                foreach (var effect in this.SourceCard.Effects)
                {
                    if (effect.Timing == "Activated")
                    {
                        effectsToPlay.Add(effect);
                    }
                }
            }

            // Push the collected effects onto the stack as a single entry, using a SequenceEffect if there are multiple effects to play
            if (effectsToPlay.Count > 0)
            {
                IEffect effectToPush = effectsToPlay.Count == 1
                    ? effectsToPlay[0]
                    : new SequenceEffect(effectsToPlay);

                EffectContext context = new EffectContext(this.SourceCard, this.SourcePlayer)
                {
                    Targets = this.Targets
                };
                StackEntry entry = new StackEntry { Effect = effectToPush, Context = context };

                handler.Stack.Push(entry);
            }
        }
    }
}