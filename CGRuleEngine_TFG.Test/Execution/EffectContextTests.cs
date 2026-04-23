using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using CGRuleEngine_TFG.Execution;
using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.GameSystem;

namespace CGRuleEngine_TFG.Tests.Execution
{
    public class EffectContextTests
    {
        [Fact]
        public void Constructor_ValidParameters_SetsPropertiesCorrectly()
        {
            // Arrange
            var cardMock = new Mock<ICard>();
            var player = new PlayerBuilder("TestPlayer").Build();

            // Act
            var context = new EffectContext(cardMock.Object, player);

            // Assert
            Assert.Same(cardMock.Object, context.SourceCard);
            Assert.Same(player, context.SourcePlayer);
            Assert.NotNull(context.Targets);
            Assert.Empty(context.Targets);
            Assert.Null(context.GameState);
        }

        [Fact]
        public void GameState_CanBeSetAndRetrieved()
        {
            // Arrange
            var cardMock = new Mock<ICard>();
            var player = new PlayerBuilder("TestPlayer").Build();
            var context = new EffectContext(cardMock.Object, player);
            var gameStateMock = new Mock<IGameState>();

            // Act
            context.GameState = gameStateMock.Object;

            // Assert
            Assert.Same(gameStateMock.Object, context.GameState);
        }

        [Fact]
        public void Targets_CanAddAndRetrieveTargets()
        {
            // Arrange
            var cardMock = new Mock<ICard>();
            var player = new PlayerBuilder("TestPlayer").Build();
            var context = new EffectContext(cardMock.Object, player);
            var targetMock = new Mock<ITargetable>();

            // Act
            context.Targets.Add(targetMock.Object);

            // Assert
            Assert.Single(context.Targets);
            Assert.Contains(targetMock.Object, context.Targets);
        }
    }
}