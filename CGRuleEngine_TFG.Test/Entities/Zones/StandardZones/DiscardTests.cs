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
    public class DiscardTests
    {
        [Fact]
        public void Constructor_SingleOwner_SetsOwner()
        {
            // Arrange
            var targetPlayer = new PlayerBuilder("TestPlayer").Build();

            // Act
            var discard = new Discard(targetPlayer);

            // Assert
            Assert.Single(discard.Owners);
            Assert.Contains(targetPlayer, discard.Owners);
            Assert.Empty(discard.Content);
        }

        [Fact]
        public void Constructor_MultipleOwners_SetsOwners()
        {
            // Arrange
            var targetPlayer1 = new PlayerBuilder("TestPlayer").Build();
            var targetPlayer2 = new PlayerBuilder("TestPlayer").Build();
            var ownersList = new List<Player> { targetPlayer1, targetPlayer2 };

            // Act
            var discard = new Discard(ownersList);

            // Assert
            Assert.Equal(2, discard.Owners.Count);
            Assert.Contains(targetPlayer1, discard.Owners);
            Assert.Contains(targetPlayer2, discard.Owners);
            Assert.Empty(discard.Content);
        }

        [Fact]
        public void Initialize_HasCards_ClearsContent()
        {
            // Arrange
            var discard = new Discard(new PlayerBuilder("TestPlayer").Build());
            var cardMock1 = new Mock<ICard>();
            var cardMock2 = new Mock<ICard>();
            discard.Content.Add(cardMock1.Object);
            discard.Content.Add(cardMock2.Object);

            // Act
            discard.Initialize();

            // Assert
            Assert.Empty(discard.Content);
        }

        [Fact]
        public void Initialize_EmptyDiscard_RemainsEmpty()
        {
            // Arrange
            var discard = new Discard(new PlayerBuilder("TestPlayer").Build());

            // Act
            discard.Initialize();

            // Assert
            Assert.Empty(discard.Content);
        }

        [Fact]
        public void MoveAll_HasCards_MovesAllToTargetZone()
        {
            // Arrange
            var discard = new Discard(new PlayerBuilder("TestPlayer").Build());
            var cardMock1 = new Mock<ICard>();
            var cardMock2 = new Mock<ICard>();

            // Add cards directly to content to setup the state
            discard.Content.Add(cardMock1.Object);
            discard.Content.Add(cardMock2.Object);

            var targetZoneMock = new Mock<IZone>();

            // Act
            discard.MoveAll(targetZoneMock.Object);

            // Assert
            Assert.Empty(discard.Content); // Verifies cards were removed from discard
            targetZoneMock.Verify(z => z.AddCard(cardMock1.Object), Times.Once);
            targetZoneMock.Verify(z => z.AddCard(cardMock2.Object), Times.Once);
        }

        [Fact]
        public void MoveAll_EmptyDiscard_DoesNothing()
        {
            // Arrange
            var discard = new Discard(new PlayerBuilder("TestPlayer").Build());
            var targetZoneMock = new Mock<IZone>();

            // Act
            discard.MoveAll(targetZoneMock.Object);

            // Assert
            Assert.Empty(discard.Content);
            targetZoneMock.Verify(z => z.AddCard(It.IsAny<ICard>()), Times.Never);
        }

        [Fact]
        public void GetTopCard_HasCards_ReturnsLastCard()
        {
            // Arrange
            var discard = new Discard(new PlayerBuilder("TestPlayer").Build());
            var cardMock1 = new Mock<ICard>();
            var cardMock2 = new Mock<ICard>();

            discard.Content.Add(cardMock1.Object);
            discard.Content.Add(cardMock2.Object);

            // Act
            var result = discard.GetTopCard();

            // Assert
            Assert.NotNull(result);
            Assert.Same(cardMock2.Object, result);
            Assert.Equal(2, discard.Content.Count); // Ensures the card is not removed
        }

        [Fact]
        public void GetTopCard_EmptyDiscard_ThrowsInvalidOperationException()
        {
            // Arrange
            var discard = new Discard(new PlayerBuilder("TestPlayer").Build());

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => discard.GetTopCard());
            Assert.Equal("The discard pile is empty.", exception.Message);
        }

        [Fact]
        public void PullCard_HasCards_ReturnsAndRemovesLastCard()
        {
            // Arrange
            var discard = new Discard(new PlayerBuilder("TestPlayer").Build());
            var cardMock1 = new Mock<ICard>();
            var cardMock2 = new Mock<ICard>();

            discard.Content.Add(cardMock1.Object);
            discard.Content.Add(cardMock2.Object);

            // Act
            var result = discard.PullCard();

            // Assert
            Assert.NotNull(result);
            Assert.Same(cardMock2.Object, result);
            Assert.Single(discard.Content); // Size reduced by 1
            Assert.DoesNotContain(cardMock2.Object, discard.Content); // Ensure it was removed
        }

        [Fact]
        public void PullCard_EmptyDiscard_ThrowsInvalidOperationException()
        {
            // Arrange
            var discard = new Discard(new PlayerBuilder("TestPlayer").Build());

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => discard.PullCard());
            Assert.Equal("The discard pile is empty.", exception.Message);
        }
    }
}