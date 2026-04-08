using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Entities.Zones
{
    public class Banished : IZone
    {
        public List<Player> Owners { get; set; } = new List<Player>();
        public List<ICard> Content { get; set; } = new List<ICard>();

        // Constructor to initialize the banished zone, it creates an empty content and no owners
        public Banished() { }

        // Method to initialize the banished zone, it can be used to set up the initial state of the banished zone, such as clearing the content
        public void Initialize()
        {
            this.Content.Clear();
        }

        // Method to add a card to the banished zone, it checks if the player is an owner of the banished zone and if so, it adds the card to the content
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

        // Overloaded method to add a card to the banished zone with a specified face-up state, it sets the card's face-up state and then calls the AddCard method
        public void AddCard(ICard card, bool isFaceUp)
        {
            card.isFaceUp = isFaceUp;
            AddCard(card);
        }

        // Method to remove a card from the banished zone, it checks if the player is an owner of the banished zone and if so, it removes the card from the content
        public void RemoveCard(ICard card)
        {
            Content.Remove(card);
            card.Controller = null;
        }

        // Method to move a card from the banished zone to another zone, it removes the card from the content and adds it to the target zone
        public void MoveCard(ICard card, IZone targetZone)
        {
            if (Content.Contains(card) == false)
            {
                throw new InvalidOperationException("The card is not in the banished.");
            }
            Content.Remove(card);
            targetZone.AddCard(card);
        }

        // Method to search for a card in the banished zone by its name, it returns the first card that matches the name or null if no card is found
        public ICard Search(string name)
        {
            return Content.Find(c => c.Name == name);
        }

        // Method to get all the face-up cards in the banished zone, it returns a list of cards that are face-up or throws an exception if the banished zone is empty
        public List<ICard> GetFaceUp()
        {
            if (this.Content.Count == 0)
            {
                throw new InvalidOperationException("The banished zone is empty.");
            }
            List<ICard> ret = new List<ICard>(Content.FindAll(c => c.isFaceUp));
            return ret;
        }

        // Method to get all the face-down cards in the banished zone, it returns a list of cards that are face-down or throws an exception if the banished zone is empty
        public List<ICard> GetFaceDown()
        {
            if (this.Content.Count == 0)
            {
                throw new InvalidOperationException("The banished zone is empty.");
            }
            List<ICard> ret = new List<ICard>(Content.FindAll(c => !c.isFaceUp));
            return ret;
        }
    }
}
