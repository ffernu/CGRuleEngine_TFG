using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
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
    public class ActivateAbilityActionTests
    {
        [Fact]
        public void Constructor_ValidInputs_SetsPropertiesCorrectly()
        {
            // Arrange
            var player = new PlayerBuilder("TestPlayer").Build();
            var cardMock = new Mock<ICard>();
            var targets = new List<ITargetable>();

            // Act
            var action = new ActivateAbilityAction(player, cardMock.Object, targets);

            // Assert
            Assert.Same(player, action.SourcePlayer);
            Assert.Same(cardMock.Object, action.SourceCard);
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

            var action = new ActivateAbilityAction(player, cardMock.Object, null, validationLogic);

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

            var action = new ActivateAbilityAction(sourcePlayer, cardMock.Object, null);

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

            var action = new ActivateAbilityAction(player, cardMock.Object, null, validationLogic);

            // Act
            bool result = action.IsValid(gameStateMock.Object);

            // Assert
            Assert.False(result);
        }
    }
}