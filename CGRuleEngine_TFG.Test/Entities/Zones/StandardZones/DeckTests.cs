using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Zones.StandardZones;

namespace CGRuleEngine_TFG.Tests.Entities.Zones.StandardZones
{
    public class DeckTests
    {
        [Fact]
        public void Constructor_SingleOwner_SetsMaxSizeAndOwner()
        {
            // Arrange
            int maxSize = 60;
            var targetPlayer = new PlayerBuilder("TestPlayer").Build();

            // Act
            var deck = new Deck(maxSize, targetPlayer);

            // Assert
            Assert.Equal(maxSize, deck.MaxSize);
            Assert.Single(deck.Owners);
            Assert.Contains(targetPlayer, deck.Owners);
        }

        [Fact]
        public void Constructor_MultipleOwners_SetsMaxSizeAndOwners()
        {
            // Arrange
            int maxSize = 40;
            var targetPlayer1 = new PlayerBuilder("TestPlayer").Build();
            var targetPlayer2 = new PlayerBuilder("TestPlayer").Build();
            var ownersList = new List<Player> { targetPlayer1, targetPlayer2 };

            // Act
            var deck = new Deck(maxSize, ownersList);

            // Assert
            Assert.Equal(maxSize, deck.MaxSize);
            Assert.Equal(2, deck.Owners.Count);
            Assert.Contains(targetPlayer1, deck.Owners);
            Assert.Contains(targetPlayer2, deck.Owners);
        }

        [Fact]
        public void Initialize_HasCards_ExecutesShuffleWithoutChangingCount()
        {
            // Arrange
            var deck = new Deck(60, new PlayerBuilder("TestPlayer").Build());
            deck.Content.Add(new Mock<ICard>().Object);
            deck.Content.Add(new Mock<ICard>().Object);
            deck.Content.Add(new Mock<ICard>().Object);
            int initialCount = deck.Content.Count;

            // Act
            deck.Initialize();

            // Assert
            Assert.Equal(initialCount, deck.Content.Count);
        }

        [Fact]
        public void AddCard_BelowMaxSize_AddsCardSuccessfully()
        {
            // Arrange
            var deck = new Deck(2, new PlayerBuilder("TestPlayer").Build());
            var cardMock = new Mock<ICard>();

            // Act
            deck.AddCard(cardMock.Object);

            // Assert
            Assert.Single(deck.Content);
            Assert.Contains(cardMock.Object, deck.Content);
        }

        [Fact]
        public void AddCard_AtMaxSize_ThrowsInvalidOperationException()
        {
            // Arrange
            var deck = new Deck(1, new PlayerBuilder("TestPlayer").Build());
            var cardMock1 = new Mock<ICard>();
            var cardMock2 = new Mock<ICard>();
            deck.AddCard(cardMock1.Object);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => deck.AddCard(cardMock2.Object));
            Assert.Equal("The deck is full.", exception.Message);
            Assert.Single(deck.Content);
        }

        [Fact]
        public void PullCard_HasCards_ReturnsAndRemovesTopCard()
        {
            // Arrange
            var deck = new Deck(60, new PlayerBuilder("TestPlayer").Build());
            var bottomCardMock = new Mock<ICard>();
            var topCardMock = new Mock<ICard>();
            deck.Content.Add(bottomCardMock.Object);
            deck.Content.Add(topCardMock.Object); // The last item is the top of the deck

            // Act
            var pulledCard = deck.PullCard();

            // Assert
            Assert.Same(topCardMock.Object, pulledCard);
            Assert.Single(deck.Content);
            Assert.DoesNotContain(topCardMock.Object, deck.Content);
            Assert.Contains(bottomCardMock.Object, deck.Content);
        }

        [Fact]
        public void PullCard_EmptyDeck_ThrowsInvalidOperationException()
        {
            // Arrange
            var deck = new Deck(60, new PlayerBuilder("TestPlayer").Build());

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => deck.PullCard());
            Assert.Equal("The deck is empty.", exception.Message);
        }

        [Fact]
        public void Suffle_HasMultipleCards_MaintainsContentCount()
        {
            // Arrange
            var deck = new Deck(60, new PlayerBuilder("TestPlayer").Build());
            deck.Content.Add(new Mock<ICard>().Object);
            deck.Content.Add(new Mock<ICard>().Object);
            deck.Content.Add(new Mock<ICard>().Object);
            deck.Content.Add(new Mock<ICard>().Object);
            deck.Content.Add(new Mock<ICard>().Object);
            int initialCount = deck.Content.Count;

            // Act
            deck.Suffle();

            // Assert
            Assert.Equal(initialCount, deck.Content.Count);
        }

        [Fact]
        public void Seek_HasCards_ReturnsTopCardWithoutRemovingIt()
        {
            // Arrange
            var deck = new Deck(60, new PlayerBuilder("TestPlayer").Build());
            var bottomCardMock = new Mock<ICard>();
            var topCardMock = new Mock<ICard>();
            deck.Content.Add(bottomCardMock.Object);
            deck.Content.Add(topCardMock.Object);

            // Act
            var seekedCard = deck.Seek();

            // Assert
            Assert.Same(topCardMock.Object, seekedCard);
            Assert.Equal(2, deck.Content.Count); // Ensure no cards were removed
        }

        [Fact]
        public void Seek_EmptyDeck_ThrowsInvalidOperationException()
        {
            // Arrange
            var deck = new Deck(60, new PlayerBuilder("TestPlayer").Build());

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => deck.Seek());
            Assert.Equal("The deck is empty.", exception.Message);
        }

        [Fact]
        public void SeekAmount_HasEnoughCards_ReturnsRequestedAmountReversed()
        {
            // Arrange
            var deck = new Deck(60, new PlayerBuilder("TestPlayer").Build());
            var card1 = new Mock<ICard>();
            var card2 = new Mock<ICard>();
            var card3 = new Mock<ICard>(); // Top card
            deck.Content.Add(card1.Object);
            deck.Content.Add(card2.Object);
            deck.Content.Add(card3.Object);

            // Act
            var seekedCards = deck.Seek(2);

            // Assert
            Assert.Equal(2, seekedCards.Count);
            Assert.Same(card3.Object, seekedCards[0]); // Top card should be first in the reversed list
            Assert.Same(card2.Object, seekedCards[1]);
            Assert.Equal(3, deck.Content.Count); // Deck size should remain the same
        }

        [Fact]
        public void SeekAmount_RequestsMoreThanAvailable_ReturnsAllCardsReversed()
        {
            // Arrange
            var deck = new Deck(60, new PlayerBuilder("TestPlayer").Build());
            var card1 = new Mock<ICard>();
            var card2 = new Mock<ICard>(); // Top card
            deck.Content.Add(card1.Object);
            deck.Content.Add(card2.Object);

            // Act
            var seekedCards = deck.Seek(5); // Requesting more than available

            // Assert
            Assert.Equal(2, seekedCards.Count);
            Assert.Same(card2.Object, seekedCards[0]); // Top card should be first
            Assert.Same(card1.Object, seekedCards[1]);
            Assert.Equal(2, deck.Content.Count);
        }

        [Fact]
        public void SeekAmount_EmptyDeck_ThrowsInvalidOperationException()
        {
            // Arrange
            var deck = new Deck(60, new PlayerBuilder("TestPlayer").Build());

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => deck.Seek(2));
            Assert.Equal("The deck is empty.", exception.Message);
        }

        [Fact]
        public void PutOnTop_CardInDeck_MovesCardToEndOfList()
        {
            // Arrange
            var deck = new Deck(60, new PlayerBuilder("TestPlayer").Build());
            var card1 = new Mock<ICard>();
            var card2 = new Mock<ICard>();
            var targetCard = new Mock<ICard>();
            deck.Content.Add(targetCard.Object); // Currently at the bottom
            deck.Content.Add(card1.Object);
            deck.Content.Add(card2.Object); // Currently at the top

            // Act
            deck.PutOnTop(targetCard.Object);

            // Assert
            Assert.Equal(3, deck.Content.Count);
            Assert.Same(targetCard.Object, deck.Content[deck.Content.Count - 1]); // Verify it's at the end (top)
        }

        [Fact]
        public void PutOnTop_CardNotInDeck_ThrowsInvalidOperationException()
        {
            // Arrange
            var deck = new Deck(60, new PlayerBuilder("TestPlayer").Build());
            deck.Content.Add(new Mock<ICard>().Object);
            var externalCard = new Mock<ICard>();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => deck.PutOnTop(externalCard.Object));
            Assert.Equal("The card is not in the deck.", exception.Message);
        }

        [Fact]
        public void PutOnTop_EmptyDeck_ThrowsInvalidOperationException()
        {
            // Arrange
            var deck = new Deck(60, new PlayerBuilder("TestPlayer").Build());
            var cardMock = new Mock<ICard>();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => deck.PutOnTop(cardMock.Object));
            Assert.Equal("The deck is empty.", exception.Message);
        }

        [Fact]
        public void PutOnBottom_CardInDeck_MovesCardToBeginningOfList()
        {
            // Arrange
            var deck = new Deck(60, new PlayerBuilder("TestPlayer").Build());
            var targetCard = new Mock<ICard>();
            var card1 = new Mock<ICard>();
            var card2 = new Mock<ICard>();
            deck.Content.Add(card1.Object);
            deck.Content.Add(card2.Object);
            deck.Content.Add(targetCard.Object); // Currently at the top

            // Act
            deck.PutOnBottom(targetCard.Object);

            // Assert
            Assert.Equal(3, deck.Content.Count);
            Assert.Same(targetCard.Object, deck.Content[0]); // Verify it's at the beginning (bottom)
        }

        [Fact]
        public void PutOnBottom_CardNotInDeck_ThrowsInvalidOperationException()
        {
            // Arrange
            var deck = new Deck(60, new PlayerBuilder("TestPlayer").Build());
            deck.Content.Add(new Mock<ICard>().Object);
            var externalCard = new Mock<ICard>();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => deck.PutOnBottom(externalCard.Object));
            Assert.Equal("The card is not in the deck.", exception.Message);
        }

        [Fact]
        public void PutOnBottom_EmptyDeck_ThrowsInvalidOperationException()
        {
            // Arrange
            var deck = new Deck(60, new PlayerBuilder("TestPlayer").Build());
            var cardMock = new Mock<ICard>();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => deck.PutOnBottom(cardMock.Object));
            Assert.Equal("The deck is empty.", exception.Message);
        }
    }
}