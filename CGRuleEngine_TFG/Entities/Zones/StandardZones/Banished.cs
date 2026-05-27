using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Entities.Zones.StandardZones
{
    public class Banished : BaseZone
    {
        // Default constructor to initialize the banished zone without setting Owners
        public Banished()
        {
        }

        // Constructor to initialize the banished zone, it creates an empty content and no owners
        public Banished(Player owner)
        {
            this.Owners.Add(owner);
        }

        // Constructor to initialize the banished zone with multiple owners, it creates an empty content and sets the owners to the provided list
        public Banished(List<Player> owners)
        {
            this.Owners.AddRange(owners);
        }

        // Method to initialize the banished zone, it can be used to set up the initial state of the banished zone, such as clearing the content
        public override void Initialize()
        {
            this.Content.Clear();// TODO
        }

        // Method to add a card to the banished zone, it adds the card to the content and sets its controller and current zone to the banished zone
        public void AddCard(ICard card, bool isFaceUp)
        {
            card.IsFaceUp = isFaceUp;
            base.AddCard(card);
        }

        // Method to pull a card from the banished zone, it removes the last card from the content and returns it
        public override ICard PullCard()
        {
            if (this.Content.Count == 0) return null;

            ICard card = this.Content[this.Content.Count - 1];
            this.RemoveCard(card);
            return card;
        }

        // Method to get all the face-up cards in the banished zone, it returns a list of cards that are face-up or throws an exception if the banished zone is empty
        public List<ICard> GetFaceUp()
        {
            if (this.Content.Count == 0)
            {
                throw new InvalidOperationException("The banished zone is empty.");
            }
            List<ICard> ret = new List<ICard>(this.Content.FindAll(c => c.IsFaceUp));
            return ret;
        }

        // Method to get all the face-down cards in the banished zone, it returns a list of cards that are face-down or throws an exception if the banished zone is empty
        public List<ICard> GetFaceDown()
        {
            if (this.Content.Count == 0)
            {
                throw new InvalidOperationException("The banished zone is empty.");
            }
            List<ICard> ret = new List<ICard>(this.Content.FindAll(c => !c.IsFaceUp));
            return ret;
        }
    }
}
