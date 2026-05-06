using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using CGRuleEngine_TFG.Execution;
using CGRuleEngine_TFG.Execution.Effects;
using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;

namespace CGRuleEngine_TFG.Tests.Execution.Effects
{
    public class SequenceEffectTests
    {
        [Fact]
        public void Constructor_NullEffectsList_InitializesEmptyList()
        {
            // Arrange
            List<IEffect> effects = null;

            // Act
            var sequenceEffect = new SequenceEffect(effects);

            // Assert
            Assert.Equal("Sequence Effect", sequenceEffect.DebugName);
            Assert.Equal("Sequence", sequenceEffect.Timing);

            // Verifying it handles execution without throwing exceptions due to null list
            var context = new EffectContext(new Mock<ICard>().Object, new PlayerBuilder("Source").Build());
            var exception = Record.Exception(() => sequenceEffect.execute(context));
            Assert.Null(exception);
        }

        [Fact]
        public void Constructor_ValidEffectsList_SetsPropertiesCorrectly()
        {
            // Arrange
            var effects = new List<IEffect> { new Mock<IEffect>().Object };

            // Act
            var sequenceEffect = new SequenceEffect(effects);

            // Assert
            Assert.Equal("Sequence Effect", sequenceEffect.DebugName);
            Assert.Equal("Sequence", sequenceEffect.Timing);
        }

        [Fact]
        public void Execute_MultipleEffects_ExecutesAllInOrder()
        {
            // Arrange
            var effectMock1 = new Mock<IEffect>();
            var effectMock2 = new Mock<IEffect>();
            var effectMock3 = new Mock<IEffect>();

            var executionOrder = new List<int>();

            // Setup callbacks to track execution order
            effectMock1.Setup(e => e.execute(It.IsAny<EffectContext>())).Callback(() => executionOrder.Add(1));
            effectMock2.Setup(e => e.execute(It.IsAny<EffectContext>())).Callback(() => executionOrder.Add(2));
            effectMock3.Setup(e => e.execute(It.IsAny<EffectContext>())).Callback(() => executionOrder.Add(3));

            var effects = new List<IEffect>
            {
                effectMock1.Object,
                effectMock2.Object,
                effectMock3.Object
            };

            var sequenceEffect = new SequenceEffect(effects);
            var context = new EffectContext(new Mock<ICard>().Object, new PlayerBuilder("Source").Build());

            // Act
            sequenceEffect.execute(context);

            // Assert
            effectMock1.Verify(e => e.execute(context), Times.Once);
            effectMock2.Verify(e => e.execute(context), Times.Once);
            effectMock3.Verify(e => e.execute(context), Times.Once);

            Assert.Equal(3, executionOrder.Count);
            Assert.Equal(1, executionOrder[0]);
            Assert.Equal(2, executionOrder[1]);
            Assert.Equal(3, executionOrder[2]);
        }

        [Fact]
        public void Execute_EmptyEffectsList_DoesNotThrow()
        {
            // Arrange
            var sequenceEffect = new SequenceEffect(new List<IEffect>());
            var context = new EffectContext(new Mock<ICard>().Object, new PlayerBuilder("Source").Build());

            // Act
            var exception = Record.Exception(() => sequenceEffect.execute(context));

            // Assert
            Assert.Null(exception);
        }
    }
}