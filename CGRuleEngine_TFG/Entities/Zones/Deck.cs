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
        public List<Card> Content { get; set; } = new List<Card>();
        public int maxSize { get; set; }
        public int minSize { get; set; }

        // Static random number generator to be used for shuffling the deck
        private static readonly Random _rng = new Random();

        // Constructor to initialize the deck with a maximum size and a minimum size
        public Deck(int maxSize, int minSize)
        {
            this.maxSize = maxSize;
            this.minSize = minSize;
        }

        // Implementation of the Clone method from the IZone interface, it creates a new instance of the Deck class with the same maximum size and minimum size, but with an empty content and no owners
        public IZone Clone()
        {
            Deck clone = new Deck(maxSize, minSize);
            return clone;
        }

        // Method to initialize the deck, it can be used to set up the initial state of the deck, such as shuffling the cards in the content
        public void Initialize()
        {
            this.Suffle();
        }

        // Method to add a card to the deck, it checks if the player is an owner of the deck and if the deck is not full before adding the card to the content
        public void AddCard(Card card)
        {
            if (Owners.Contains(card.Controller) == false)
            {
                throw new InvalidOperationException("The player is not an owner of the deck.");
            }
            if (Content.Count >= maxSize)
            {
                throw new InvalidOperationException("The deck is full.");
            }
            Content.Add(card);
        }

        // Method to remove a card from the deck, it checks if the card is in the content before removing it
        public void RemoveCard(Card card)
        {
            if (Content.Contains(card) == false)
            {
                throw new InvalidOperationException("The card is not in the deck.");
            }
            Content.Remove(card);
        }

        // Method to move a card from the deck to another zone, it checks if the card is in the content before removing it and adding it to the target zone
        public void MoveCard(Card card, IZone targetZone)
        {
            if (Content.Contains(card) == false)
            {
                throw new InvalidOperationException("The card is not in the deck.");
            }
            RemoveCard(card);
            targetZone.AddCard(card);
        }

        // Method to search for a card in the deck by its name, it uses the Find method of the List class to find the first card with the specified name and returns it
        public Card Search(string name)
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
                Card value = Content[k];
                Content[k] = Content[n];
                Content[n] = value;
            }
        }

        // Method to draw a card from the top of the deck, it checks if the content is not empty before removing and returning the top card
        public Card Draw()
        {
            if (Content.Count == 0)
            {
                throw new InvalidOperationException("The deck is empty.");
            }
            int top = Content.Count - 1;
            Card card = Content[top];
            Content.RemoveAt(top);
            return card;
        }

        // Method to seek a specified amount of cards from the top of the deck, it checks if the content is not empty and if the amount is less than or equal to the number of cards in the content before returning a list of cards from the top of the deck
        public List<Card> Seek(int amount)
        {
            if (Content.Count == 0)
            {
                throw new InvalidOperationException("The deck is empty.");
            }
            if (amount > Content.Count)
            {
                List<Card> allCards = new List<Card>(this.Content);
                allCards.Reverse();
                return allCards;
            }
            List<Card> cards = new List<Card>(Content.GetRange(Content.Count - amount, amount));
            cards.Reverse();
            return cards;
        }

        // Method to put a card on top of the deck, it checks if the content is not empty and if the card is in the content before removing it and adding it to the end of the list, which represents the top of the deck
        public void PutOnTop(Card card)
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
        public void PutOnBottom(Card card)
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
