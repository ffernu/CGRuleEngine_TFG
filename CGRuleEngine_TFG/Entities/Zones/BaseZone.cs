using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using System;
using System.Collections.Generic;

namespace CGRuleEngine_TFG.Entities.Zones
{
    // Abstract class that implements the IZone interface, it provides common functionality for all zones and defines some abstract methods that must be implemented by the derived classes
    public abstract class BaseZone : IZone
    {
        public List<Player> Owners { get; set; } = new List<Player>();
        public List<ICard> Content { get; set; } = new List<ICard>();
        public virtual bool ResetOnEntry { get; set; } = true;

        // Abstract method to initialize the zone, it must be implemented by the derived classes to define how the zone is initialized
        public abstract void Initialize();

        // Method to add a card to the zone, it adds the card to the content and sets its controller and current zone, it also resets the card if ResetOnEntry is true
        public virtual void AddCard(ICard card)
        {
            this.Content.Add(card);

            if (this.ResetOnEntry)
            {
                card.Reset();
            }
            if (this.Owners.Count > 0)
            {
                card.Controller = this.Owners[0];
            }
            else
            {
                card.Controller = null;
            }

            card.CurrentZone = this;
        }

        // Method to remove a card from the zone, it checks if the card is in the content before removing it and sets its controller and current zone to null
        public virtual void RemoveCard(ICard card)
        {
            if (!this.Content.Contains(card))
            {
                throw new InvalidOperationException("The card is not in this zone.");
            }

            this.Content.Remove(card);
            card.Controller = null;
            card.CurrentZone = null;
        }

        // Method to move a card from this zone to another zone, it checks if the card is in the content before removing it and adding it to the target zone
        public virtual void MoveCard(ICard card, IZone targetZone)
        {
            this.RemoveCard(card);
            targetZone.AddCard(card);
        }

        // Method to search for a card in the zone by its name, it returns the first card that matches the name or null if no card is found
        public virtual ICard Search(string name)
        {
            return this.Content.Find(c => c.Name == name);
        }

        // Abstract method to pull a card from the zone, it must be implemented by the derived classes to define how a card is pulled from the zone
        public abstract ICard PullCard();
    }
}