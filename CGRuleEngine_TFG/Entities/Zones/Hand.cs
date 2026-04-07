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
        public List<Card> Content { get; set; } = new List<Card>();
        public int MaxSize { get; set; }

        // Constructor to initialize the hand with a maximum size
        public Hand(int maxSize)
        {
            this.MaxSize = maxSize;
        }

        // Implementation of the Clone method from the IZone interface, it creates a new instance of the Hand class with the same maximum size, but with an empty content and no owners
        public IZone Clone()
        {
            Hand clone = new Hand(MaxSize);
            return clone;
        }

        // Method to initialize the hand, it does nothing but is necessary due to Game's initializeGame method, it can be used to set up the initial state of the hand if needed
        public void Initialize()
        {
            return;
        }

        // Method to add a card to the hand, it checks if the player is an owner of the hand and if the hand is not full before adding the card to the content
        public void AddCard(Card card)
        {
            if (Owners.Contains(card.Controller) == false)
            {
                throw new InvalidOperationException("The player is not an owner of the hand.");
            }
            if (Content.Count >= MaxSize)
            {
                throw new InvalidOperationException("The hand is full.");
            }
            Content.Add(card);
        }

        // Method to remove a card from the hand, it checks if the player is an owner of the hand and if the card is in the content before removing it
        public void RemoveCard(Card card)
        {
            if (Owners.Contains(card.Controller) == false)
            {
                throw new InvalidOperationException("The player is not an owner of the hand.");
            }
            if (!Content.Contains(card))
            {
                throw new InvalidOperationException("The card is not in the hand.");
            }
            Content.Remove(card);
        }

        // Method to move a card from the hand to another zone, it checks if the player is an owner of the hand and if the card is in the content before removing it from the hand and adding it to the target zone
        public void MoveCard(Card card, IZone targetZone)
        {
            if (!Content.Contains(card))
            {
                throw new InvalidOperationException("The card is not in the hand.");
            }
            RemoveCard(card);
            targetZone.AddCard(card);
        }

        // Method to search for a card in the hand by its name, it returns the first card that matches the name or null if no card is found
        public Card Search(string name)
        {
            return Content.Find(c => c.Name == name);
        }

        // Method to check if the hand has reached its maximum size, it returns true if the number of cards in the content is greater than or equal to the maximum size, otherwise it returns false
        public bool HasReachedLimit()
        {
            return Content.Count >= MaxSize;
        }

        // Method to discard a card from the hand to a discard zone, it checks if the player is an owner of the hand and if the card is in the content before removing it from the hand and adding it to the discard zone
        public void Discard(Card card, IZone discard)
        {
            this.MoveCard(card, discard);
        }

        // Method to discard all cards from the hand to a discard zone, it iterates through all the cards in the content and moves them to the discard zone
        public void DiscardAll(IZone discard)
        {
            foreach (var card in new List<Card>(Content))
            {
                this.MoveCard(card, discard);
            }
        }
    }
}
