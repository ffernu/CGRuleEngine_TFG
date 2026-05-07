using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Zones;
using CGRuleEngine_TFG.Execution;
using CGRuleEngine_TFG.Execution.Actions;
using CGRuleEngine_TFG.Execution.Effects;
using CGRuleEngine_TFG.GameSystem;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace CGRuleEngine_TFG.Tests.Execution
{
    public class PlayCardActionTests
    {
        [Fact]
        public void Constructor_ValidInputs_SetsPropertiesCorrectly()
        {
            // Arrange
            var player = new PlayerBuilder("TestPlayer").Build();
            var cardMock = new Mock<ICard>();
            var targets = new List<ITargetable>();

            // Act
            var action = new PlayCardAction(player, cardMock.Object, targets);

            // Assert
            Assert.Same(player, action.SourcePlayer);
            Assert.Same(cardMock.Object, action.CardToPlay);
            Assert.Same(targets, action.Targets);
        }

        [Fact]
        public void IsValid_PriorityPlayerIsSourcePlayerAndValidationPasses_ReturnsTrue()
        {
            // Arrange
            var player = new PlayerBuilder("TestPlayer").Build();
            var cardMock = new Mock<ICard>();
            var gameStateMock = new Mock<IGameState>();
            gameStateMock.Setup(g => g.PriorityPlayer).Returns(player);

            Func<Player, ICard, IGameState, bool> validationLogic = (p, c, g) => true;

            var action = new PlayCardAction(player, cardMock.Object, null, validationLogic);

            // Act
            bool result = action.IsValid(gameStateMock.Object);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValid_PriorityPlayerIsNotSourcePlayer_ReturnsFalse()
        {
            // Arrange
            var sourcePlayer = new PlayerBuilder("SourcePlayer").Build();
            var priorityPlayer = new PlayerBuilder("PriorityPlayer").Build();
            var cardMock = new Mock<ICard>();

            var gameStateMock = new Mock<IGameState>();
            gameStateMock.Setup(g => g.PriorityPlayer).Returns(priorityPlayer);

            var action = new PlayCardAction(sourcePlayer, cardMock.Object, null);

            // Act
            bool result = action.IsValid(gameStateMock.Object);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_ValidationLogicFails_ReturnsFalse()
        {
            // Arrange
            var player = new PlayerBuilder("TestPlayer").Build();
            var cardMock = new Mock<ICard>();
            var gameStateMock = new Mock<IGameState>();
            gameStateMock.Setup(g => g.PriorityPlayer).Returns(player);

            Func<Player, ICard, IGameState, bool> validationLogic = (p, c, g) => false;

            var action = new PlayCardAction(player, cardMock.Object, null, validationLogic);

            // Act
            bool result = action.IsValid(gameStateMock.Object);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Commit_RemovesCardFromZoneAndInvokesPayment()
        {
            // Arrange
            var player = new PlayerBuilder("TestPlayer").Build();
            var cardMock = new Mock<ICard>();

            var zoneMock = new Mock<IZone>();
            cardMock.SetupProperty(c => c.CurrentZone, zoneMock.Object);

            bool paymentInvoked = false;
            Action<Player, ICard> paymentLogic = (p, c) => paymentInvoked = true;

            var action = new PlayCardAction(player, cardMock.Object, null, null, paymentLogic);
            var handlerMock = new Mock<IActionHandler>();

            // Act
            action.Commit(handlerMock.Object);

            // Assert
            Assert.True(paymentInvoked);
            zoneMock.Verify(z => z.RemoveCard(cardMock.Object), Times.Once);
            Assert.Null(cardMock.Object.CurrentZone);
        }
    }
}