using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Entities.Zones
{
    public class Discard : IZone
    {
        public List<Player> Owners { get; set; } = new List<Player>();
        public List<Card> Content { get; set; } = new List<Card>();

        // Constructor to initialize the discard pile, it creates an empty content and no owners
        public Discard() { }

        // Method to create a copy of the discard pile, it creates a new instance of the Disscard class with an empty content and no owners
        public IZone Clone()
        {
            return new Discard();
        }

        // Method to initialize the discard pile, it clears the content of the discard pile
        public void Initialize()
        {
            this.Content.Clear();
        }

        // Method to add cards to the discard pile
        public void AddCard(Card card)
        {
            if (Owners.Contains(card.Controller) == false)
            {
                throw new InvalidOperationException("The player is not an owner of the discard.");
            }
            Content.Add(card);
        }

        // Method to remove cards from the discard pile
        public void RemoveCard(Card card)
        {
            if (Owners.Contains(card.Controller) == false)
            {
                throw new InvalidOperationException("The player is not an owner of the discard.");
            }
            Content.Remove(card);
        }

        // Method to move cards from the discard pile to another zone
        public void MoveCard(Card card, IZone targetZone)
        {
            if (Content.Contains(card) == false)
            {
                throw new InvalidOperationException("The card is not in the discard.");
            }
            Content.Remove(card);
            targetZone.AddCard(card);
        }

        // Method to move all cards from the discard pile to another zone
        public void MoveAll(IZone targetZone)
        {
            foreach (var card in new List<Card>(Content))
            {
                this.MoveCard(card, targetZone);
            }
        }

        // Method to search for a card in the discard pile by name, it returns the first card that matches the name or null if no card is found
        public Card Search(string name)
        {
            return Content.Find(c => c.Name == name);
        }

        // Method to get the top card of the discard pile, it returns the last card in the content list or throws an exception if the discard pile is empty
        public Card GetTopCard()
        {
            if (Content.Count == 0)
            {
                throw new InvalidOperationException("The discard pile is empty.");
            }
            return Content[Content.Count - 1];
        }
    }
}
