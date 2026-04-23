using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Zones.StandardZones;

namespace CGRuleEngine_TFG.Tests.Entities.Zones.StandardZones
{
    public class BoardTests
    {
        [Fact]
        public void Constructor_SingleOwner_SetsPropertiesAndOwner()
        {
            // Arrange
            int maxSize = 10;
            int maxSizePerPlayer = 5;
            var targetPlayer = new PlayerBuilder("TestPlayer").Build();

            // Act
            var board = new Board(maxSize, maxSizePerPlayer, targetPlayer);

            // Assert
            Assert.Equal(maxSize, board.MaxSize);
            Assert.Equal(maxSizePerPlayer, board.MaxSizePerPlayer);
            Assert.Single(board.Owners);
            Assert.Contains(targetPlayer, board.Owners);
        }

        [Fact]
        public void Constructor_MultipleOwners_SetsPropertiesAndOwners()
        {
            // Arrange
            int maxSize = 14;
            int maxSizePerPlayer = 7;
            var targetPlayer1 = new PlayerBuilder("TestPlayer").Build();
            var targetPlayer2 = new PlayerBuilder("TestPlayer").Build();
            var ownersList = new List<Player> { targetPlayer1, targetPlayer2 };

            // Act
            var board = new Board(maxSize, maxSizePerPlayer, ownersList);

            // Assert
            Assert.Equal(maxSize, board.MaxSize);
            Assert.Equal(maxSizePerPlayer, board.MaxSizePerPlayer);
            Assert.Equal(2, board.Owners.Count);
            Assert.Contains(targetPlayer1, board.Owners);
            Assert.Contains(targetPlayer2, board.Owners);
        }

        [Fact]
        public void Initialize_BoardHasCards_ClearsContent()
        {
            // Arrange
            var board = new Board(10, 5, new PlayerBuilder("TestPlayer").Build());
            board.Content.Add(new Mock<ICard>().Object);

            // Act
            board.Initialize();

            // Assert
            Assert.Empty(board.Content);
        }

        [Fact]
        public void AddCard_BoardIsFull_ThrowsInvalidOperationException()
        {
            // Arrange
            var targetPlayer = new PlayerBuilder("TestPlayer").Build();
            var board = new Board(1, 5, targetPlayer);

            var card1Mock = new Mock<ICard>();
            card1Mock.Setup(c => c.Controller).Returns(targetPlayer);
            var card2Mock = new Mock<ICard>();
            card2Mock.Setup(c => c.Controller).Returns(targetPlayer);

            // Fill the board
            board.Content.Add(card1Mock.Object);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => board.AddCard(card2Mock.Object));
            Assert.Equal("The board is full.", exception.Message);
        }

        [Fact]
        public void AddCard_PlayerReachedMax_ThrowsInvalidOperationException()
        {
            // Arrange
            var targetPlayer = new PlayerBuilder("TestPlayer").Build();
            var board = new Board(10, 1, targetPlayer);

            var card1Mock = new Mock<ICard>();
            card1Mock.Setup(c => c.Controller).Returns(targetPlayer);
            var card2Mock = new Mock<ICard>();
            card2Mock.Setup(c => c.Controller).Returns(targetPlayer);

            // Add one card for the player (reaching their max limit)
            board.Content.Add(card1Mock.Object);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => board.AddCard(card2Mock.Object));
            Assert.Equal("The player has reached the maximum number of cards on the board.", exception.Message);
        }

        [Fact]
        public void AddCard_ValidCardAndResetOnEntryTrue_AddsCardAndResetsIt()
        {
            // Arrange
            var targetPlayer = new PlayerBuilder("TestPlayer").Build();
            var board = new Board(10, 5, targetPlayer) { ResetOnEntry = true };
            var cardMock = new Mock<ICard>();
            cardMock.Setup(c => c.Controller).Returns(targetPlayer);

            // Act
            board.AddCard(cardMock.Object);

            // Assert
            Assert.Contains(cardMock.Object, board.Content);
            cardMock.Verify(c => c.Reset(), Times.Once);
            cardMock.VerifySet(c => c.CurrentZone = board, Times.Once);
            cardMock.VerifySet(c => c.Controller = It.IsAny<Player>(), Times.Never); // Verifies Controller is NOT changed
        }

        [Fact]
        public void AddCard_ValidCardAndResetOnEntryFalse_AddsCardWithoutResetting()
        {
            // Arrange
            var targetPlayer = new PlayerBuilder("TestPlayer").Build();
            var board = new Board(10, 5, targetPlayer) { ResetOnEntry = false };
            var cardMock = new Mock<ICard>();
            cardMock.Setup(c => c.Controller).Returns(targetPlayer);

            // Act
            board.AddCard(cardMock.Object);

            // Assert
            Assert.Contains(cardMock.Object, board.Content);
            cardMock.Verify(c => c.Reset(), Times.Never);
            cardMock.VerifySet(c => c.CurrentZone = board, Times.Once);
        }

        [Fact]
        public void RemoveCard_CardNotOnBoard_ThrowsInvalidOperationException()
        {
            // Arrange
            var board = new Board(10, 5, new PlayerBuilder("TestPlayer").Build());
            var cardMock = new Mock<ICard>();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => board.RemoveCard(cardMock.Object));
            Assert.Equal("The card is not on the board.", exception.Message);
        }

        [Fact]
        public void RemoveCard_CardOnBoard_RemovesCardAndNullifiesCurrentZone()
        {
            // Arrange
            var board = new Board(10, 5, new PlayerBuilder("TestPlayer").Build());
            var cardMock = new Mock<ICard>();
            board.Content.Add(cardMock.Object);

            // Act
            board.RemoveCard(cardMock.Object);

            // Assert
            Assert.DoesNotContain(cardMock.Object, board.Content);
            cardMock.VerifySet(c => c.CurrentZone = null, Times.Once);
            cardMock.VerifySet(c => c.Controller = It.IsAny<Player>(), Times.Never); // Verifies Controller is NOT changed
        }

        [Fact]
        public void PullCard_EmptyBoard_ReturnsNull()
        {
            // Arrange
            var board = new Board(10, 5, new PlayerBuilder("TestPlayer").Build());

            // Act
            var result = board.PullCard();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void PullCard_BoardHasCards_ReturnsAndRemovesLastCard()
        {
            // Arrange
            var board = new Board(10, 5, new PlayerBuilder("TestPlayer").Build());
            var firstCardMock = new Mock<ICard>();
            var lastCardMock = new Mock<ICard>();

            board.Content.Add(firstCardMock.Object);
            board.Content.Add(lastCardMock.Object);

            // Act
            var result = board.PullCard();

            // Assert
            Assert.Same(lastCardMock.Object, result);
            Assert.Single(board.Content);
            Assert.Contains(firstCardMock.Object, board.Content);
            lastCardMock.VerifySet(c => c.CurrentZone = null, Times.Once);
        }

        [Fact]
        public void CountByController_CardsExistForPlayer_ReturnsCorrectCount()
        {
            // Arrange
            var targetPlayer = new PlayerBuilder("TestPlayer").Build();
            var otherPlayer = new PlayerBuilder("TestPlayer").Build();
            var board = new Board(10, 5, targetPlayer);

            var card1 = new Mock<ICard>();
            card1.Setup(c => c.Controller).Returns(targetPlayer);
            var card2 = new Mock<ICard>();
            card2.Setup(c => c.Controller).Returns(otherPlayer);
            var card3 = new Mock<ICard>();
            card3.Setup(c => c.Controller).Returns(targetPlayer);

            board.Content.Add(card1.Object);
            board.Content.Add(card2.Object);
            board.Content.Add(card3.Object);

            // Act
            int count = board.CountByController(targetPlayer);

            // Assert
            Assert.Equal(2, count);
        }

        [Fact]
        public void ClearByController_CardsExistForPlayer_RemovesAndReturnsPlayerCards()
        {
            // Arrange
            var targetPlayer = new PlayerBuilder("TestPlayer").Build();
            var otherPlayer = new PlayerBuilder("TestPlayer").Build();
            var board = new Board(10, 5, targetPlayer);

            var card1 = new Mock<ICard>();
            card1.Setup(c => c.Controller).Returns(targetPlayer);
            var card2 = new Mock<ICard>();
            card2.Setup(c => c.Controller).Returns(otherPlayer);
            var card3 = new Mock<ICard>();
            card3.Setup(c => c.Controller).Returns(targetPlayer);

            board.Content.Add(card1.Object);
            board.Content.Add(card2.Object);
            board.Content.Add(card3.Object);

            // Act
            var removedCards = board.ClearByController(targetPlayer);

            // Assert
            Assert.Equal(2, removedCards.Count);
            Assert.Contains(card1.Object, removedCards);
            Assert.Contains(card3.Object, removedCards);

            Assert.Single(board.Content);
            Assert.Contains(card2.Object, board.Content); // Other player's card remains
        }

        [Fact]
        public void GetCardsByController_CardsExist_ReturnsOnlyPlayerCards()
        {
            // Arrange
            var targetPlayer = new PlayerBuilder("TestPlayer").Build();
            var otherPlayer = new PlayerBuilder("TestPlayer").Build();
            var board = new Board(10, 5, targetPlayer);

            var card1 = new Mock<ICard>();
            card1.Setup(c => c.Controller).Returns(targetPlayer);
            var card2 = new Mock<ICard>();
            card2.Setup(c => c.Controller).Returns(otherPlayer);

            board.Content.Add(card1.Object);
            board.Content.Add(card2.Object);

            // Act
            var playerCards = board.GetCardsByController(targetPlayer);

            // Assert
            Assert.Single(playerCards);
            Assert.Same(card1.Object, playerCards[0]);
            Assert.Equal(2, board.Content.Count); // Ensure no cards were removed
        }

        [Fact]
        public void Clear_BoardHasCards_RemovesAllCardsAndReturnsThem()
        {
            // Arrange
            var board = new Board(10, 5, new PlayerBuilder("TestPlayer").Build());
            var card1Mock = new Mock<ICard>();
            var card2Mock = new Mock<ICard>();

            board.Content.Add(card1Mock.Object);
            board.Content.Add(card2Mock.Object);

            // Act
            var clearedCards = board.Clear();

            // Assert
            Assert.Equal(2, clearedCards.Count);
            Assert.Contains(card1Mock.Object, clearedCards);
            Assert.Contains(card2Mock.Object, clearedCards);
            Assert.Empty(board.Content);

            // Verify RemoveCard logic was applied to both
            card1Mock.VerifySet(c => c.CurrentZone = null, Times.Once);
            card2Mock.VerifySet(c => c.CurrentZone = null, Times.Once);
        }

        [Fact]
        public void Clear_EmptyBoard_ReturnsEmptyList()
        {
            // Arrange
            var board = new Board(10, 5, new PlayerBuilder("TestPlayer").Build());

            // Act
            var clearedCards = board.Clear();

            // Assert
            Assert.Empty(clearedCards);
            Assert.Empty(board.Content);
        }
    }
}