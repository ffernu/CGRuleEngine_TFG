using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Zones;
using CGRuleEngine_TFG.Entities.Zones.StandardZones;

namespace CGRuleEngine_TFG.Tests.Entities.Zones.StandardZones
{
    public class HandTests
    {
        [Fact]
        public void Constructor_SingleOwner_SetsMaxSizeAndOwner()
        {
            // Arrange
            int maxSize = 5;
            var targetPlayer = new PlayerBuilder("TestPlayer").Build();

            // Act
            var hand = new Hand(maxSize, targetPlayer);

            // Assert
            Assert.Equal(maxSize, hand.MaxSize);
            Assert.Single(hand.Owners);
            Assert.Contains(targetPlayer, hand.Owners);
        }

        [Fact]
        public void Constructor_MultipleOwners_SetsMaxSizeAndOwners()
        {
            // Arrange
            int maxSize = 7;
            var targetPlayer1 = new PlayerBuilder("TestPlayer").Build();
            var targetPlayer2 = new PlayerBuilder("TestPlayer").Build();
            var ownersList = new List<Player> { targetPlayer1, targetPlayer2 };

            // Act
            var hand = new Hand(maxSize, ownersList);

            // Assert
            Assert.Equal(maxSize, hand.MaxSize);
            Assert.Equal(2, hand.Owners.Count);
            Assert.Contains(targetPlayer1, hand.Owners);
            Assert.Contains(targetPlayer2, hand.Owners);
        }

        [Fact]
        public void Initialize_Always_ReturnsWithoutExceptions()
        {
            // Arrange
            var hand = new Hand(5, new PlayerBuilder("TestPlayer").Build());

            // Act & Assert
            var exception = Record.Exception(() => hand.Initialize());
            Assert.Null(exception);
        }

        [Fact]
        public void AddCard_BelowMaxSize_AddsCard()
        {
            // Arrange
            var hand = new Hand(2, new PlayerBuilder("TestPlayer").Build());
            var cardMock = new Mock<ICard>();

            // Act
            hand.AddCard(cardMock.Object);

            // Assert
            Assert.Single(hand.Content);
            Assert.Contains(cardMock.Object, hand.Content);
        }

        [Fact]
        public void AddCard_AtMaxSize_ThrowsInvalidOperationException()
        {
            // Arrange
            var hand = new Hand(1, new PlayerBuilder("TestPlayer").Build());
            var firstCardMock = new Mock<ICard>();
            var secondCardMock = new Mock<ICard>();

            // Fill the hand to its maximum size
            hand.AddCard(firstCardMock.Object);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => hand.AddCard(secondCardMock.Object));
            Assert.Equal("The hand is full.", exception.Message);
            Assert.Single(hand.Content); // Ensure the second card was not added
        }

        [Fact]
        public void PullCard_EmptyHand_ReturnsNull()
        {
            // Arrange
            var hand = new Hand(5, new PlayerBuilder("TestPlayer").Build());

            // Act
            var result = hand.PullCard();

            // Assert
            Assert.Null(result);
            Assert.Empty(hand.Content);
        }

        [Fact]
        public void PullCard_HandHasCards_RemovesAndReturnsCard()
        {
            // Arrange
            var hand = new Hand(5, new PlayerBuilder("TestPlayer").Build());
            var cardMock1 = new Mock<ICard>();
            var cardMock2 = new Mock<ICard>();

            hand.AddCard(cardMock1.Object);
            hand.AddCard(cardMock2.Object);

            // Act
            var result = hand.PullCard();

            // Assert
            Assert.NotNull(result);
            // Since it's random, we check if the result is one of the originally added cards
            Assert.True(result == cardMock1.Object || result == cardMock2.Object);

            // Validate that the returned card is no longer in the hand's content
            Assert.DoesNotContain(result, hand.Content);

            // Hand size should be reduced by 1
            Assert.Single(hand.Content);
        }

        [Fact]
        public void HasReachedLimit_BelowMaxSize_ReturnsFalse()
        {
            // Arrange
            var hand = new Hand(3, new PlayerBuilder("TestPlayer").Build());
            hand.AddCard(new Mock<ICard>().Object);

            // Act
            bool result = hand.HasReachedLimit();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HasReachedLimit_AtMaxSize_ReturnsTrue()
        {
            // Arrange
            var hand = new Hand(1, new PlayerBuilder("TestPlayer").Build());
            hand.AddCard(new Mock<ICard>().Object);

            // Act
            bool result = hand.HasReachedLimit();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void MoveAll_HandHasCards_MovesAllToTargetZone()
        {
            // Arrange
            var hand = new Hand(5, new PlayerBuilder("TestPlayer").Build());
            var cardMock1 = new Mock<ICard>();
            var cardMock2 = new Mock<ICard>();

            hand.AddCard(cardMock1.Object);
            hand.AddCard(cardMock2.Object);

            var discardZoneMock = new Mock<IZone>();

            // Act
            hand.MoveAll(discardZoneMock.Object);

            // Assert
            Assert.Empty(hand.Content);
            discardZoneMock.Verify(z => z.AddCard(cardMock1.Object), Times.Once);
            discardZoneMock.Verify(z => z.AddCard(cardMock2.Object), Times.Once);
        }

        [Fact]
        public void MoveAll_EmptyHand_DoesNothing()
        {
            // Arrange
            var hand = new Hand(5, new PlayerBuilder("TestPlayer").Build());
            var discardZoneMock = new Mock<IZone>();

            // Act
            hand.MoveAll(discardZoneMock.Object);

            // Assert
            Assert.Empty(hand.Content);
            discardZoneMock.Verify(z => z.AddCard(It.IsAny<ICard>()), Times.Never);
        }
    }
}