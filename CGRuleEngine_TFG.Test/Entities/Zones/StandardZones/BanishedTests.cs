using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Zones.StandardZones;

namespace CGRuleEngine_TFG.Tests.Entities.Zones.StandardZones
{
    public class BanishedTests
    {
        [Fact]
        public void Constructor_SingleOwner_SetsOwner()
        {
            // Arrange
            var targetPlayer = new PlayerBuilder("TestPlayer").Build();

            // Act
            var banished = new Banished(targetPlayer);

            // Assert
            Assert.Single(banished.Owners);
            Assert.Contains(targetPlayer, banished.Owners);
            Assert.Empty(banished.Content);
        }

        [Fact]
        public void Constructor_MultipleOwners_SetsOwners()
        {
            // Arrange
            var targetPlayer1 = new PlayerBuilder("TestPlayer").Build();
            var targetPlayer2 = new PlayerBuilder("TestPlayer").Build();
            var ownersList = new List<Player> { targetPlayer1, targetPlayer2 };

            // Act
            var banished = new Banished(ownersList);

            // Assert
            Assert.Equal(2, banished.Owners.Count);
            Assert.Contains(targetPlayer1, banished.Owners);
            Assert.Contains(targetPlayer2, banished.Owners);
            Assert.Empty(banished.Content);
        }

        [Fact]
        public void Initialize_HasCards_ClearsContent()
        {
            // Arrange
            var banished = new Banished(new PlayerBuilder("TestPlayer").Build());
            var cardMock1 = new Mock<ICard>();
            var cardMock2 = new Mock<ICard>();
            banished.Content.Add(cardMock1.Object);
            banished.Content.Add(cardMock2.Object);

            // Act
            banished.Initialize();

            // Assert
            Assert.Empty(banished.Content);
        }

        [Fact]
        public void Initialize_EmptyZone_RemainsEmpty()
        {
            // Arrange
            var banished = new Banished(new PlayerBuilder("TestPlayer").Build());

            // Act
            banished.Initialize();

            // Assert
            Assert.Empty(banished.Content);
        }

        [Fact]
        public void AddCard_FaceUpTrue_SetsFaceUpAndAddsToContent()
        {
            // Arrange
            var banished = new Banished(new PlayerBuilder("TestPlayer").Build());
            var cardMock = new Mock<ICard>();

            // Act
            banished.AddCard(cardMock.Object, true);

            // Assert
            cardMock.VerifySet(c => c.IsFaceUp = true, Times.Once);
            Assert.Contains(cardMock.Object, banished.Content);
        }

        [Fact]
        public void AddCard_FaceUpFalse_SetsFaceDownAndAddsToContent()
        {
            // Arrange
            var banished = new Banished(new PlayerBuilder("TestPlayer").Build());
            var cardMock = new Mock<ICard>();

            // Act
            banished.AddCard(cardMock.Object, false);

            // Assert
            cardMock.VerifySet(c => c.IsFaceUp = false, Times.Once);
            Assert.Contains(cardMock.Object, banished.Content);
        }

        [Fact]
        public void PullCard_EmptyZone_ReturnsNull()
        {
            // Arrange
            var banished = new Banished(new PlayerBuilder("TestPlayer").Build());

            // Act
            var result = banished.PullCard();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void PullCard_HasCards_ReturnsAndRemovesLastCard()
        {
            // Arrange
            var banished = new Banished(new PlayerBuilder("TestPlayer").Build());
            var firstCardMock = new Mock<ICard>();
            var lastCardMock = new Mock<ICard>();

            banished.Content.Add(firstCardMock.Object);
            banished.Content.Add(lastCardMock.Object);

            // Act
            var result = banished.PullCard();

            // Assert
            Assert.Same(lastCardMock.Object, result);
            Assert.Single(banished.Content);
            Assert.Contains(firstCardMock.Object, banished.Content);
            Assert.DoesNotContain(lastCardMock.Object, banished.Content);
        }

        [Fact]
        public void GetFaceUp_EmptyZone_ThrowsInvalidOperationException()
        {
            // Arrange
            var banished = new Banished(new PlayerBuilder("TestPlayer").Build());

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => banished.GetFaceUp());
            Assert.Equal("The banished zone is empty.", exception.Message);
        }

        [Fact]
        public void GetFaceUp_HasCards_ReturnsOnlyFaceUpCards()
        {
            // Arrange
            var banished = new Banished(new PlayerBuilder("TestPlayer").Build());

            var faceUpCard1 = new Mock<ICard>();
            faceUpCard1.Setup(c => c.IsFaceUp).Returns(true);

            var faceDownCard = new Mock<ICard>();
            faceDownCard.Setup(c => c.IsFaceUp).Returns(false);

            var faceUpCard2 = new Mock<ICard>();
            faceUpCard2.Setup(c => c.IsFaceUp).Returns(true);

            banished.Content.Add(faceUpCard1.Object);
            banished.Content.Add(faceDownCard.Object);
            banished.Content.Add(faceUpCard2.Object);

            // Act
            var result = banished.GetFaceUp();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(faceUpCard1.Object, result);
            Assert.Contains(faceUpCard2.Object, result);
            Assert.DoesNotContain(faceDownCard.Object, result);
        }

        [Fact]
        public void GetFaceDown_EmptyZone_ThrowsInvalidOperationException()
        {
            // Arrange
            var banished = new Banished(new PlayerBuilder("TestPlayer").Build());

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => banished.GetFaceDown());
            Assert.Equal("The banished zone is empty.", exception.Message);
        }

        [Fact]
        public void GetFaceDown_HasCards_ReturnsOnlyFaceDownCards()
        {
            // Arrange
            var banished = new Banished(new PlayerBuilder("TestPlayer").Build());

            var faceUpCard = new Mock<ICard>();
            faceUpCard.Setup(c => c.IsFaceUp).Returns(true);

            var faceDownCard1 = new Mock<ICard>();
            faceDownCard1.Setup(c => c.IsFaceUp).Returns(false);

            var faceDownCard2 = new Mock<ICard>();
            faceDownCard2.Setup(c => c.IsFaceUp).Returns(false);

            banished.Content.Add(faceUpCard.Object);
            banished.Content.Add(faceDownCard1.Object);
            banished.Content.Add(faceDownCard2.Object);

            // Act
            var result = banished.GetFaceDown();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(faceDownCard1.Object, result);
            Assert.Contains(faceDownCard2.Object, result);
            Assert.DoesNotContain(faceUpCard.Object, result);
        }
    }
}