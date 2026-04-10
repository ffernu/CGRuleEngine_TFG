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
        public List<ICard> Content { get; set; } = new List<ICard>();
        public bool ResetOnEntry { get; set; } = true;

        // Constructor to initialize the discard pile, it creates an empty content and no owners
        public Discard() { }

        // Method to initialize the discard pile, it clears the content of the discard pile
        public void Initialize()
        {
            this.Content.Clear();
        }

        // Method to add cards to the discard pile
        public void AddCard(ICard card)
        {
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

        // Method to remove cards from the discard pile
        public void RemoveCard(ICard card)
        {
            if (Content.Contains(card) == false)
            {
                throw new InvalidOperationException("The card is not in the discard.");
            }
            Content.Remove(card);
            card.Controller = null;
        }

        // Method to move cards from the discard pile to another zone
        public void MoveCard(ICard card, IZone targetZone)
        {
            Content.Remove(card);
            targetZone.AddCard(card);
        }

        // Method to move all cards from the discard pile to another zone
        public void MoveAll(IZone targetZone)
        {
            foreach (var card in new List<ICard>(Content))
            {
                this.MoveCard(card, targetZone);
            }
        }

        // Method to search for a card in the discard pile by name, it returns the first card that matches the name or null if no card is found
        public ICard Search(string name)
        {
            return Content.Find(c => c.Name == name);
        }

        //
        public ICard PullCard()
        {
            // TODO.
            return null;
        }

        // Method to get the top card of the discard pile, it returns the last card in the content list or throws an exception if the discard pile is empty
        public ICard GetTopCard()
        {
            if (Content.Count == 0)
            {
                throw new InvalidOperationException("The discard pile is empty.");
            }
            return Content[Content.Count - 1];
        }
    }
}
