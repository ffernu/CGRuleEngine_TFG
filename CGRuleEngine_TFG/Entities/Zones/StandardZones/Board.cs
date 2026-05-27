using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Entities.Zones.StandardZones
{
    public class Board : BaseZone
    {
        public int MaxSize { get; set; }
        public int MaxSizePerPlayer { get; set; }

        // Default constructor to initialize the board without setting Owners
        public Board(int maxSize, int maxSizePerPlayer)
        {
            this.MaxSize = maxSize;
            this.MaxSizePerPlayer = maxSizePerPlayer;
        }

        // Constructor to initialize the board with a maximum size and a maximum size per player
        public Board(int maxSize, int maxSizePerPlayer, Player owner)
        {
            this.MaxSize = maxSize;
            this.MaxSizePerPlayer = maxSizePerPlayer;
            this.Owners.Add(owner);
        }

        // Constructor to initialize the board with a maximum size, a maximum size per player and a list of owners
        public Board(int maxSize, int maxSizePerPlayer, List<Player> owners)
        {
            this.MaxSize = maxSize;
            this.MaxSizePerPlayer = maxSizePerPlayer;
            this.Owners.AddRange(owners);
        }

        // Method to initialize the board, it can be used to set up the initial state of the board, such as clearing the content
        public override void Initialize()
        {
            this.Content.Clear();// TODO
        }

        // Implementation of the AddCard method from the IZone interface, it adds a card to the board if the player is an owner of the board, if the board is not full and if the player has not reached the maximum number of cards on the board
        public override void AddCard(ICard card)
        {
            if (this.Content.Count >= MaxSize)
            {
                throw new InvalidOperationException("The board is full.");
            }
            if (this.CountByController(card.Controller) >= this.MaxSizePerPlayer)
            {
                throw new InvalidOperationException("The player has reached the maximum number of cards on the board.");
            }
            Content.Add(card);
            if (this.ResetOnEntry)
            {
                card.Reset();
            }
            card.CurrentZone = this;
            // The controller of the card is not changed when the card is added to the board, it is assumed the board is shared by all players 
        }

        // Implementation of the RemoveCard method from the IZone interface, it removes a card from the board if the card is on the board
        public override void RemoveCard(ICard card)
        {
            if (this.Content.Contains(card) == false)
            {
                throw new InvalidOperationException("The card is not on the board.");
            }
            this.Content.Remove(card);
            card.CurrentZone = null;
            // Because when the card was added to the board its controller was not changed, the controller of the card is not changed when the card is removed from the board
        }

        // Method that removes the last card added to the board and returns it
        public override ICard PullCard()
        {
            if (this.Content.Count == 0) return null;

            ICard card = this.Content[this.Content.Count - 1];
            this.RemoveCard(card);
            return card;
        }

        // Method that returns the number of cards on the board that are controlled by a specific player
        public int CountByController(Player player)
        {
            return this.Content.FindAll(c => c.Controller == player).Count;
        }

        // Method that removes all cards from the board that are owned by a specific player and returns a list of the removed cards
        public List<ICard> ClearByController(Player player)
        {
            List<ICard> removedCards = this.GetCardsByController(player);
            Content.RemoveAll(c => c.Controller == player);
            return removedCards;
        }

        // Method that returns a list of cards on the board that are controlled by a specific player
        public List<ICard> GetCardsByController(Player player)
        {
            return this.Content.FindAll(c => c.Controller == player);
        }

        // Method that removes all cards from the board and returns a list of the removed cards, it uses a copy of the content to avoid modifying the collection while iterating over it
        public List<ICard> Clear()
        {
            List<ICard> removedCards = new List<ICard>(this.Content);
            foreach (var card in removedCards)
            {
                this.RemoveCard(card);
            }
            return removedCards;
        }
    }
}
