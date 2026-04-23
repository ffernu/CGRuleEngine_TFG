using System;
using Xunit;
using Moq;
using CGRuleEngine_TFG.Execution;
using CGRuleEngine_TFG.Execution.Effects;
using CGRuleEngine_TFG.GameSystem;
using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;

namespace CGRuleEngine_TFG.Tests.Execution
{
    public class StackEntryTests
    {
        [Fact]
        public void Resolve_ValidGameState_SetsContextAndExecutesEffect()
        {
            // Arrange
            var effectMock = new Mock<IEffect>();
            var sourceCardMock = new Mock<ICard>();
            var sourcePlayer = new PlayerBuilder("TestPlayer").Build();
            var context = new EffectContext(sourceCardMock.Object, sourcePlayer);

            var entry = new StackEntry
            {
                Effect = effectMock.Object,
                Context = context
            };

            var gameStateMock = new Mock<IGameState>();

            // Act
            entry.Resolve(gameStateMock.Object);

            // Assert
            Assert.Same(gameStateMock.Object, entry.Context.GameState);
            effectMock.Verify(e => e.execute(entry.Context), Times.Once);
        }

        [Fact]
        public void Resolve_NullGameState_ExecutesEffectWithNullGameState()
        {
            // Arrange
            var effectMock = new Mock<IEffect>();
            var sourceCardMock = new Mock<ICard>();
            var sourcePlayer = new PlayerBuilder("TestPlayer").Build();
            var context = new EffectContext(sourceCardMock.Object, sourcePlayer);

            var entry = new StackEntry
            {
                Effect = effectMock.Object,
                Context = context
            };

            // Act
            entry.Resolve(null);

            // Assert
            Assert.Null(entry.Context.GameState);
            effectMock.Verify(e => e.execute(entry.Context), Times.Once);
        }
    }
}