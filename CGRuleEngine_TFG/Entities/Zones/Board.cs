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
        public List<ICard> Content { get; set; } = new List<ICard>();
        public int maxSize { get; set; }
        public int maxSizePerPlayer { get; set; }

        // Constructor to initialize the board with a maximum size and a maximum size per player
        public Board(int maxSize, int maxSizePerPlayer)
        {
            this.maxSize = maxSize;
            this.maxSizePerPlayer = maxSizePerPlayer;
        }

        // Method to initialize the board, it can be used to set up the initial state of the board, such as clearing the content
        public void Initialize()
        {
            this.Content.Clear();
        }

        // Implementation of the AddCard method from the IZone interface, it adds a card to the board if the player is an owner of the board, if the board is not full and if the player has not reached the maximum number of cards on the board
        public void AddCard(ICard card)
        {
            if (Content.Count >= maxSize)
            {
                throw new InvalidOperationException("The board is full.");
            }
            if (this.CountByController(card.Controller) >= maxSizePerPlayer)
            {
                throw new InvalidOperationException("The player has reached the maximum number of cards on the board.");
            }
            Content.Add(card);
            // The controller of the card is not changed when the card is added to the board, it is assumed the board is shared by all players 
        }

        // Method that returns the number of cards on the board that are controlled by a specific player
        public int CountByController(Player player)
        {
            return Content.FindAll(c => c.Controller == player).Count;
        }

        // Implementation of the RemoveCard method from the IZone interface, it removes a card from the board if the card is on the board
        public void RemoveCard(ICard card)
        {
            if (Content.Contains(card) == false)
            {
                throw new InvalidOperationException("The card is not on the board.");
            }
            Content.Remove(card);
            // Because when the card was added to the board its controller was not changed, the controller of the card is not changed when the card is removed from the board
        }

        // Method that removes all cards from the board and returns a list of the removed cards
        public List<ICard> Clear()
        {
            List<ICard> removedCards = new List<ICard>(Content);
            Content.Clear();
            return removedCards;
        }

        // Method that removes all cards from the board that are owned by a specific player and returns a list of the removed cards
        public List<ICard> ClearByController(Player player)
        {
            List<ICard> removedCards = this.GetCardsByController(player);
            Content.RemoveAll(c => c.Controller == player);
            return removedCards;
        }

        // Implementation of the MoveCard method from the IZone interface, it moves a card from the board to another zone if the card is on the board
        public void MoveCard(ICard card, IZone targetZone)
        {
            RemoveCard(card);
            targetZone.AddCard(card);
        }

        // Implementation of the Search method from the IZone interface, it searches for a card on the board by its name and returns it if found, otherwise it returns null
        public ICard Search(string name)
        {
            return Content.Find(c => c.Name == name);
        }

        // Method that returns a list of cards on the board that are controlled by a specific player
        public List<ICard> GetCardsByController(Player player)
        {
            return Content.FindAll(c => c.Controller == player);
        }
    }
}
