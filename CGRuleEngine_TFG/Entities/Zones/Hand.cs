using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Entities.Zones
{
    public class Hand : IZone
    {
        public List<Player> Owners { get; set; } = new List<Player>();
        public List<ICard> Content { get; set; } = new List<ICard>();
        public int MaxSize { get; set; }

        // Constructor to initialize the hand with a maximum size
        public Hand(int maxSize)
        {
            this.MaxSize = maxSize;
        }

        // Method to initialize the hand, it does nothing but is necessary due to Game's initializeGame method, it can be used to set up the initial state of the hand if needed
        public virtual void Initialize()
        {
            return;
        }

        // Method to add a card to the hand, it checks if the hand has reached its maximum size before adding the card to the content
        public void AddCard(ICard card)
        {
            if (Content.Count >= MaxSize)
            {
                throw new InvalidOperationException("The hand is full.");
            }
            Content.Add(card);
            if (this.Owners.Count > 0)
            {
                card.Controller = this.Owners[0];
            }
            else
            {
                card.Controller = null;
            }
        }

        // Method to remove a card from the hand, it checks if the card is in the content before removing it from the hand
        public void RemoveCard(ICard card)
        {
            if (!Content.Contains(card))
            {
                throw new InvalidOperationException("The card is not in the hand.");
            }
            Content.Remove(card);
            card.Controller = null;
        }

        // Method to move a card from the hand to another zone, it changes the controllers of the card to match the owners of the target zone
        public void MoveCard(ICard card, IZone targetZone)
        {
            RemoveCard(card);
            targetZone.AddCard(card);
        }

        // Method to search for a card in the hand by its name, it returns the first card that matches the name or null if no card is found
        public ICard Search(string name)
        {
            return Content.Find(c => c.Name == name);
        }

        // Method to check if the hand has reached its maximum size, it returns true if the number of cards in the content is greater than or equal to the maximum size, otherwise it returns false
        public bool HasReachedLimit()
        {
            return Content.Count >= MaxSize;
        }

        // Method to move all cards from the hand to another zone
        public void MoveAll(IZone discard)
        {
            foreach (var card in new List<ICard>(Content))
            {
                this.MoveCard(card, discard);
            }
        }
    }
}
