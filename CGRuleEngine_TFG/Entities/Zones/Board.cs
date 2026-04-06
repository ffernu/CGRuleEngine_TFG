using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Entities.Zones
{
    public class Board : IZone
    {
        public List<Player> Owners { get; set; } = new List<Player>();
        public List<Card> Content { get; set; } = new List<Card>();
        public int maxSize { get; set; }
        public int maxSizePerPlayer { get; set; }

        // Constructor to initialize the board with a maximum size and a maximum size per player
        public Board(int maxSize, int maxSizePerPlayer)
        {
            this.maxSize = maxSize;
            this.maxSizePerPlayer = maxSizePerPlayer;
        }

        // Implementation of the Clone method from the IZone interface, it creates a new instance of the Board class with the same maximum size and maximum size per player, but with an empty content and no owners
        public IZone Clone()
        {
            Board clone = new Board(maxSize, maxSizePerPlayer);
            return clone;
        }

        // Implementation of the AddCard method from the IZone interface, it adds a card to the board if the player is an owner of the board, if the board is not full and if the player has not reached the maximum number of cards on the board
        public void AddCard(Card card)
        {
            if (Owners.Contains(card.Controller) == false)
            {
                throw new InvalidOperationException("The player is not an owner of the board.");
            }
            if (Content.Count >= maxSize)
            {
                throw new InvalidOperationException("The board is full.");
            }
            int playerCardCount = Content.FindAll(c => c.Controller == card.Controller).Count;
            if (playerCardCount >= maxSizePerPlayer)
            {
                throw new InvalidOperationException("The player has reached the maximum number of cards on the board.");
            }
            Content.Add(card);
        }

        // Implementation of the RemoveCard method from the IZone interface, it removes a card from the board if the card is on the board
        public void RemoveCard(Card card)
        {
            if (Content.Contains(card) == false)
            {
                throw new InvalidOperationException("The card is not on the board.");
            }
            Content.Remove(card);
        }

        // Method that removes all cards from the board and returns a list of the removed cards
        public List<Card> clear()
        {
            List<Card> removedCards = new List<Card>(Content);
            Content.Clear();
            return removedCards;
        }

        // Method that removes all cards from the board that are controlled by a specific player and returns a list of the removed cards
        public List<Card> clearByOwner(Player player)
        {
            List<Card> removedCards = Content.FindAll(c => c.Controller == player);
            Content.RemoveAll(c => c.Controller == player);
            return removedCards;
        }

        // Implementation of the MoveCard method from the IZone interface, it moves a card from the board to another zone if the card is on the board
        public void MoveCard(Card card, IZone targetZone)
        {
            if (Content.Contains(card) == false)
            {
                throw new InvalidOperationException("The card is not on the board.");
            }
            RemoveCard(card);
            targetZone.AddCard(card);
        }

        // Implementation of the Search method from the IZone interface, it searches for a card on the board by its name and returns it if found, otherwise it returns null
        public Card Search(string name)
        {
            return Content.Find(c => c.Name == name);
        }

        // Method that returns a list of cards on the board that are controlled by a specific player
        public List<Card> GetCardsByOwner(Player player)
        {
            return Content.FindAll(c => c.Controller == player);
        }
    }
}
