using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Zones;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Execution.Effects
{
    public class MoveCardEffect : IEffect
    {
        public String? DebugName { get; set; }

        public String DestinationZoneName { get; set; }

        // Constructor for the MoveCardEffect, initializing the destination zone to which the cards will be moved
        public MoveCardEffect(string? debugName, string destinationZoneName)
        {
            this.DebugName = debugName;

            this.DestinationZoneName = destinationZoneName;
        }

        // Method to execute the effect, which iterates through the targets in the context and moves each target that is a card to the specified destination zone of the source player
        public void execute(EffectContext context)
        {
            foreach (ITargetable target in context.Targets)
            {
                if (target is ICard card)
                {
                    IZone destZone = context.SourcePlayer.getZone(this.DestinationZoneName);

                    if (destZone == null) continue;

                    // Reset state of the card if the destination zone requires it
                    if (destZone.ResetOnEntry)
                    {
                        card.Reset();
                    }

                    
                    // TODO: sadjflaksjdflkajs leer bloc notas
                    destZone.AddCard(card);
                }
            }
        }
    }
}
