using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Entities.Zones
{
    public class Hand : BaseZone
    {
        public int MaxSize { get; set; }

        // Static random number generator to be used for selecting a random card from the hand
        private static readonly Random _rng = new Random();

        // Constructor to initialize the hand with a maximum size
        public Hand(int maxSize, Player owner)
        {
            this.MaxSize = maxSize;
            this.Owners.Add(owner);
        }

        // Constructor to initialize the hand with a maximum size and multiple owners
        public Hand(int maxSize, List<Player> owners)
        {
            this.MaxSize = maxSize;
            this.Owners.AddRange(owners);
        }

        // Method to initialize the hand, it does nothing but is necessary due to Game's initializeGame method, it can be used to set up the initial state of the hand if needed
        public override void Initialize()
        {
            return;// TODO:
        }

        // Method to add a card to the hand, it checks if the hand has reached its maximum size before adding the card to the content
        public override void AddCard(ICard card)
        {
            if (this.Content.Count >= this.MaxSize)
            {
                throw new InvalidOperationException("The hand is full.");
            }
            base.AddCard(card);
        }

        // Method to pull a card from the hand, it checks if the hand is empty before selecting a random card from the content
        public override ICard PullCard()
        {
            if (this.Content.Count == 0) return null;

            int randomIndex = _rng.Next(this.Content.Count);
            ICard card = this.Content[randomIndex];

            this.RemoveCard(card);
            return card;
        }

        // Method to check if the hand has reached its maximum size, it returns true if the number of cards in the content is greater than or equal to the maximum size, otherwise it returns false
        public bool HasReachedLimit()
        {
            return this.Content.Count >= MaxSize;
        }

        // Method to move all cards from the hand to another zone
        public void MoveAll(IZone discard)
        {
            foreach (var card in new List<ICard>(this.Content))
            {
                this.MoveCard(card, discard);
            }
        }
    }
}
