using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Entities.Zones.StandardZones
{
    public class Deck : BaseZone
    {
        public int MaxSize { get; set; }

        // Static random number generator to be used for shuffling the deck
        private static readonly Random _rng = new Random();

        // Constructor to initialize the deck with a maximum size and a minimum size
        public Deck(int maxSize, Player owner)
        {
            this.MaxSize = maxSize;
            this.Owners.Add(owner);
        }

        // Constructor to initialize the deck with a maximum size and a list of owners
        public Deck(int maxSize, List<Player> owners)
        {
            this.MaxSize = maxSize;
            this.Owners.AddRange(owners);
        }

        // Method to initialize the deck, it can be used to set up the initial state of the deck, such as shuffling the cards in the content
        public override void Initialize()
        {
            this.Suffle();
        }

        // Method to add a card to the deck, it checks if the player is an owner of the deck and if the deck is not full before adding the card to the content
        public override void AddCard(ICard card)
        {
            if (this.Content.Count >= MaxSize)
            {
                throw new InvalidOperationException("The deck is full.");
            }
            base.AddCard(card);
        }

        // Method to pull a card from the deck, it is an alias for the Draw method
        public override ICard PullCard()
        {
            ICard card = this.Seek();
            this.RemoveCard(card);
            return card;
        }

        // Method to shuffle the deck, it uses the Fisher-Yates algorithm to randomly shuffle the cards in the content list
        public void Suffle()
        {
            int n = this.Content.Count;
            while (n > 1)
            {
                n--;
                int k = _rng.Next(n + 1);
                ICard value = this.Content[k];
                this.Content[k] = this.Content[n];
                this.Content[n] = value;
            }
        }

        // Method to seek the top card of the deck, it checks if the content is not empty before returning the last card in the list, which represents the top of the deck
        public ICard Seek()
        {
            if (this.Content.Count == 0)
            {
                throw new InvalidOperationException("The deck is empty.");
            }
            int top = this.Content.Count - 1;
            ICard card = this.Content[top];
            return card;
        }

        // Method to seek a specified amount of cards from the top of the deck, it checks if the content is not empty and if the amount is less than or equal to the number of cards in the content before returning a list of cards from the top of the deck
        public List<ICard> Seek(int amount)
        {
            if (this.Content.Count == 0)
            {
                throw new InvalidOperationException("The deck is empty.");
            }
            if (amount > this.Content.Count)
            {
                List<ICard> allCards = new List<ICard>(this.Content);
                allCards.Reverse();
                return allCards;
            }
            List<ICard> cards = new List<ICard>(this.Content.GetRange(this.Content.Count - amount, amount));
            cards.Reverse();
            return cards;
        }

        // Method to put a card on top of the deck, it checks if the content is not empty and if the card is in the content before removing it and adding it to the end of the list, which represents the top of the deck
        public void PutOnTop(ICard card)
        {
            if (this.Content.Count == 0)
            {
                throw new InvalidOperationException("The deck is empty.");
            }
            if (this.Content.Contains(card))
            {
                this.Content.Remove(card);
                this.Content.Add(card);
            }
            else
            {
                throw new InvalidOperationException("The card is not in the deck.");
            }
        }

        // Method to put a card on the bottom of the deck, it checks if the content is not empty and if the card is in the content before removing it and adding it to the beginning of the list, which represents the bottom of the deck
        public void PutOnBottom(ICard card)
        {
            if (this.Content.Count == 0)
            {
                throw new InvalidOperationException("The deck is empty.");
            }
            if (this.Content.Contains(card))
            {
                this.Content.Remove(card);
                this.Content.Insert(0, card);
            }
            else
            {
                throw new InvalidOperationException("The card is not in the deck.");
            }
        }
    }
}
