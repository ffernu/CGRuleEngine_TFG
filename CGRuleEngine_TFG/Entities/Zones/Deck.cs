using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Entities.Zones
{
    public class Deck : IZone
    {
        public List<Player> Owners { get; set; } = new List<Player>();
        public List<ICard> Content { get; set; } = new List<ICard>();
        public bool ResetOnEntry { get; set; } = true;
        public int maxSize { get; set; }

        // Static random number generator to be used for shuffling the deck
        private static readonly Random _rng = new Random();

        // Constructor to initialize the deck with a maximum size and a minimum size
        public Deck(int maxSize)
        {
            this.maxSize = maxSize;
        }

        // Method to initialize the deck, it can be used to set up the initial state of the deck, such as shuffling the cards in the content
        public void Initialize()
        {
            this.Suffle();
        }

        // Method to add a card to the deck, it checks if the player is an owner of the deck and if the deck is not full before adding the card to the content
        public void AddCard(ICard card)
        {
            if (Content.Count >= maxSize)
            {
                throw new InvalidOperationException("The deck is full.");
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
            card.CurrentZone = this;
        }

        // Method to remove a card from the deck, it checks if the card is in the content before removing it
        public void RemoveCard(ICard card)
        {
            if (Content.Contains(card) == false)
            {
                throw new InvalidOperationException("The card is not in the deck.");
            }
            Content.Remove(card);
            card.Controller = null;
            card.CurrentZone = null;
        }

        // Method to move a card from the deck to another zone, it checks if the card is in the content before removing it and adding it to the target zone
        public void MoveCard(ICard card, IZone targetZone)
        {
            RemoveCard(card);
            targetZone.AddCard(card);
        }

        // Method to search for a card in the deck by its name, it uses the Find method of the List class to find the first card with the specified name and returns it
        public ICard Search(string name)
        {
            return Content.Find(c => c.Name == name);
        }

        // Method to shuffle the deck, it uses the Fisher-Yates algorithm to randomly shuffle the cards in the content list
        public void Suffle()
        {
            int n = Content.Count;
            while (n > 1)
            {
                n--;
                int k = _rng.Next(n + 1);
                ICard value = Content[k];
                Content[k] = Content[n];
                Content[n] = value;
            }
        }

        // Method to seek the top card of the deck, it checks if the content is not empty before returning the last card in the list, which represents the top of the deck
        public ICard Seek()
        {
            if (Content.Count == 0)
            {
                throw new InvalidOperationException("The deck is empty.");
            }
            int top = Content.Count - 1;
            ICard card = Content[top];
            return card;
        }

        // Method to pull a card from the deck, it is an alias for the Draw method
        public ICard PullCard()
        {
            ICard card = this.Seek();
            this.RemoveCard(card);
            return card;
        }

        // Method to seek a specified amount of cards from the top of the deck, it checks if the content is not empty and if the amount is less than or equal to the number of cards in the content before returning a list of cards from the top of the deck
        public List<ICard> Seek(int amount)
        {
            if (Content.Count == 0)
            {
                throw new InvalidOperationException("The deck is empty.");
            }
            if (amount > Content.Count)
            {
                List<ICard> allCards = new List<ICard>(this.Content);
                allCards.Reverse();
                return allCards;
            }
            List<ICard> cards = new List<ICard>(Content.GetRange(Content.Count - amount, amount));
            cards.Reverse();
            return cards;
        }

        // Method to put a card on top of the deck, it checks if the content is not empty and if the card is in the content before removing it and adding it to the end of the list, which represents the top of the deck
        public void PutOnTop(ICard card)
        {
            if (Content.Count == 0)
            {
                throw new InvalidOperationException("The deck is empty.");
            }
            if (Content.Contains(card))
            {
                Content.Remove(card);
                Content.Add(card);
            }
            else
            {
                throw new InvalidOperationException("The card is not in the deck.");
            }
        }

        // Method to put a card on the bottom of the deck, it checks if the content is not empty and if the card is in the content before removing it and adding it to the beginning of the list, which represents the bottom of the deck
        public void PutOnBottom(ICard card)
        {
            if (Content.Count == 0)
            {
                throw new InvalidOperationException("The deck is empty.");
            }
            if (Content.Contains(card))
            {
                Content.Remove(card);
                Content.Insert(0, card);
            }
            else
            {
                throw new InvalidOperationException("The card is not in the deck.");
            }
        }
    }
}
