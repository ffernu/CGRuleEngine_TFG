using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Zones;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Execution.Effects.StandardEffects
{
    public class DrawEffect : IEffect
    {
        public String? DebugName { get; set; }

        public int Amount { get; set; }
        public String SourceZoneName { get; set; }
        public String DestinationZoneName { get; set; }

        // Constructor for the DrawEffect, initializing the amount of cards to be drawn, the source zone from which the cards will be drawn, and the destination zone to which the cards will be moved
        public DrawEffect(String? debugName, int amount = 1, String source = "Deck", String destination = "Hand")
        {
            this.DebugName = debugName;

            this.Amount = amount;
            this.SourceZoneName = source;
            this.DestinationZoneName = destination;
        }

        // Method to execute the effect, which iterates through the targets in the context and moves the specified amount of cards from the source zone to the destination zone for each target that is a player
        public void execute(EffectContext context)
        {
            foreach (ITargetable target in context.Targets)
            {
                if (target is Player player)
                {
                    IZone source = player.getZone(this.SourceZoneName);
                    IZone dest = player.getZone(this.DestinationZoneName);

                    for (int i = 0; i < this.Amount; i++)
                    {
                        // Pull a card from the source zone and add it to the destination zone
                        ICard cardToMove = source.PullCard();

                        if (cardToMove != null)
                        {
                            dest.AddCard(cardToMove);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
