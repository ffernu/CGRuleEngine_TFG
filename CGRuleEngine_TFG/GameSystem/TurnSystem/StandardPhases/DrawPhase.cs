using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Zones;
using System;

namespace CGRuleEngine_TFG.GameSystem.TurnSystem.StandardPhases
{
    public class DrawPhase : Phase
    {
        public DrawPhase() : base("Draw phase") { }

        // This method is called when the phase starts. It handles the logic for drawing a card from the active player's deck to their hand
        public override void OnEnter(Game game)
        {
            // Check if there is an active player
            if (game.ActivePlayer == null) return;

            if (game.ActivePlayer.Zones.TryGetValue("Deck", out IZone deck) &&
                game.ActivePlayer.Zones.TryGetValue("Hand", out IZone hand))
            {

                if (deck.Content.Count > 0)
                {
                    // Draw a card from the deck and add it to the hand
                    ICard card = deck.PullCard();
                    hand.AddCard(card);
                }
                else
                {
                    // Logic for empty deck (e.g., lose the game, shuffle discard into deck, etc.)
                }
            }

            game.ResetPriority();
        }
    }
}