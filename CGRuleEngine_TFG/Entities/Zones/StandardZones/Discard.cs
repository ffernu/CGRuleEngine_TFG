using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Entities.Zones.StandardZones
{
    public class Discard : BaseZone
    {
        // Constructor to initialize the discard pile, it creates an empty content and no owners
        public Discard(Player owner)
        {
            this.Owners.Add(owner);
        }

        // Constructor to initialize the discard pile with multiple owners, it creates an empty content and sets the owners to the provided list
        public Discard(List<Player> owners)
        {
            this.Owners.AddRange(owners);
        }

        // Method to initialize the discard pile, it clears the content of the discard pile
        public override void Initialize()
        {
            this.Content.Clear();// TODO:
        }

        // Method to move all cards from the discard pile to another zone
        public void MoveAll(IZone targetZone)
        {
            foreach (var card in new List<ICard>(this.Content))
            {
                this.MoveCard(card, targetZone);
            }
        }

        // Method to pull a card from the discard pile, it removes the top card from the content and returns it
        public override ICard PullCard()
        {
            ICard card = this.GetTopCard();
            this.RemoveCard(card);
            return card;
        }

        // Method to get the top card of the discard pile, it returns the last card in the content list or throws an exception if the discard pile is empty
        public ICard GetTopCard()
        {
            if (this.Content.Count == 0)
            {
                throw new InvalidOperationException("The discard pile is empty.");
            }
            return this.Content[this.Content.Count - 1];
        }
    }
}
