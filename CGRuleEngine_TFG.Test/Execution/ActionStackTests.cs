using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using CGRuleEngine_TFG.Execution;
using CGRuleEngine_TFG.GameSystem;

namespace CGRuleEngine_TFG.Tests.Execution
{
    public class ActionStackTests
    {
        [Fact]
        public void Constructor_ValidGameState_SetsGameStateAndInitializesEmptyStack()
        {
            // Arrange
            var gameStateMock = new Mock<IGameState>();

            // Act
            var stack = new ActionStack(gameStateMock.Object);

            // Assert
            Assert.Same(gameStateMock.Object, stack.gameState);
            Assert.NotNull(stack.Entries);
            Assert.Empty(stack.Entries);
        }

        [Fact]
        public void Push_ValidEntry_AddsEntryToTop()
        {
            // Arrange
            var gameStateMock = new Mock<IGameState>();
            var stack = new ActionStack(gameStateMock.Object);
            var entry = new StackEntry();

            // Act
            stack.Push(entry);

            // Assert
            Assert.Single(stack.Entries);
            Assert.Same(entry, stack.Peek());
        }

        [Fact]
        public void Push_MultipleEntries_MaintainsLIFOOrder()
        {
            // Arrange
            var stack = new ActionStack(new Mock<IGameState>().Object);
            var entry1 = new StackEntry();
            var entry2 = new StackEntry();

            // Act
            stack.Push(entry1);
            stack.Push(entry2);

            // Assert
            Assert.Equal(2, stack.Entries.Count);
            Assert.Same(entry2, stack.Peek()); // The last pushed should be on top
        }

        [Fact]
        public void Pop_HasEntries_RemovesTopEntryAndResolvesIt()
        {
            // Arrange
            var gameStateMock = new Mock<IGameState>();
            var stack = new ActionStack(gameStateMock.Object);

            // We use a mock of an effect just to verify Resolve calls execute properly internally
            var effectMock = new Mock<CGRuleEngine_TFG.Execution.Effects.IEffect>();
            var context = new EffectContext(new Mock<CGRuleEngine_TFG.Entities.Cards.ICard>().Object, new CGRuleEngine_TFG.Entities.Players.PlayerBuilder().Build());
            var entry = new StackEntry { Effect = effectMock.Object, Context = context };

            stack.Push(entry);

            // Act
            stack.Pop();

            // Assert
            Assert.Empty(stack.Entries);
            Assert.Same(gameStateMock.Object, context.GameState); // Validate Resolve was called
            effectMock.Verify(e => e.execute(context), Times.Once); // Validate inner execution
        }

        [Fact]
        public void Pop_EmptyStack_ThrowsInvalidOperationException()
        {
            // Arrange
            var stack = new ActionStack(new Mock<IGameState>().Object);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => stack.Pop());
        }

        [Fact]
        public void Peek_HasEntries_ReturnsTopEntryWithoutRemovingIt()
        {
            // Arrange
            var stack = new ActionStack(new Mock<IGameState>().Object);
            var entry = new StackEntry();
            stack.Push(entry);

            // Act
            var topEntry = stack.Peek();

            // Assert
            Assert.Same(entry, topEntry);
            Assert.Single(stack.Entries); // Stack size remains unchanged
        }

        [Fact]
        public void Peek_EmptyStack_ThrowsInvalidOperationException()
        {
            // Arrange
            var stack = new ActionStack(new Mock<IGameState>().Object);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => stack.Peek());
        }

        [Fact]
        public void IsEmpty_EmptyStack_ReturnsTrue()
        {
            // Arrange
            var stack = new ActionStack(new Mock<IGameState>().Object);

            // Act
            bool result = stack.IsEmpty();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsEmpty_HasEntries_ReturnsFalse()
        {
            // Arrange
            var stack = new ActionStack(new Mock<IGameState>().Object);
            stack.Push(new StackEntry());

            // Act
            bool result = stack.IsEmpty();

            // Assert
            Assert.False(result);
        }
    }
}