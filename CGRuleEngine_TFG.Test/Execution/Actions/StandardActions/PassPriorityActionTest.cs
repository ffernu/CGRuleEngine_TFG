using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Execution;
using CGRuleEngine_TFG.Execution.Actions;
using CGRuleEngine_TFG.GameSystem;
using Moq;
using System;
using Xunit;

namespace CGRuleEngine_TFG.Tests.Execution
{
    public class PassPriorityActionTests
    {
        [Fact]
        public void Constructor_ValidPlayer_SetsSourcePlayerCorrectly()
        {
            // Arrange
            var player = new PlayerBuilder("TestPlayer").Build();

            // Act
            var action = new PassPriorityAction(player);

            // Assert
            Assert.Same(player, action.SourcePlayer);
        }

        [Fact]
        public void IsValid_PriorityPlayerIsSourcePlayer_ReturnsTrue()
        {
            // Arrange
            var player = new PlayerBuilder("TestPlayer").Build();
            var action = new PassPriorityAction(player);

            var gameStateMock = new Mock<IGameState>();
            gameStateMock.Setup(g => g.PriorityPlayer).Returns(player);

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
            var action = new PassPriorityAction(sourcePlayer);

            var gameStateMock = new Mock<IGameState>();
            gameStateMock.Setup(g => g.PriorityPlayer).Returns(priorityPlayer);

            // Act
            bool result = action.IsValid(gameStateMock.Object);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_PriorityPlayerIsNull_ReturnsFalse()
        {
            // Arrange
            var sourcePlayer = new PlayerBuilder("SourcePlayer").Build();
            var action = new PassPriorityAction(sourcePlayer);

            var gameStateMock = new Mock<IGameState>();
            gameStateMock.Setup(g => g.PriorityPlayer).Returns((Player)null);

            // Act
            bool result = action.IsValid(gameStateMock.Object);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Commit_ValidHandler_CallsPassPriorityWithSourcePlayer()
        {
            // Arrange
            var player = new PlayerBuilder("TestPlayer").Build();
            var action = new PassPriorityAction(player);

            var handlerMock = new Mock<IActionHandler>();

            // Act
            action.Commit(handlerMock.Object);

            // Assert
            handlerMock.Verify(h => h.PassPriority(player), Times.Once);
        }
    }
}