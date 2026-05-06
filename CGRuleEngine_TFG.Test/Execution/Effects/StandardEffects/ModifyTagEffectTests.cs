using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using CGRuleEngine_TFG.Execution;
using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.GameSystem;
using CGRuleEngine_TFG.Execution.Effects.StandardEffects;

namespace CGRuleEngine_TFG.Tests.Execution.Effects.StandardEffects
{
    public class ModifyTagEffectTests
    {
        [Fact]
        public void Constructor_SetsPropertiesCorrectly()
        {
            // Arrange
            string expectedTag = "Poisoned";
            TagAction expectedAction = TagAction.Add;
            string expectedDebugName = "Add Poison";
            string expectedTiming = "OnPlay";

            // Act
            var effect = new ModifyTagEffect(expectedTag, expectedAction, expectedDebugName, expectedTiming);

            // Assert
            Assert.Equal(expectedTag, effect.AffectedTag);
            Assert.Equal(expectedAction, effect.Action);
            Assert.Equal(expectedDebugName, effect.DebugName);
            Assert.Equal(expectedTiming, effect.Timing);
        }

        [Fact]
        public void Execute_ActionAdd_CallsAddTagOnTargets()
        {
            // Arrange
            var effect = new ModifyTagEffect("Poisoned", TagAction.Add, "Add Poison", "OnPlay");

            var targetMock = new Mock<ITargetable>();
            var context = new EffectContext(new Mock<ICard>().Object, new PlayerBuilder("Source").Build());
            context.Targets.Add(targetMock.Object);

            // Act
            effect.execute(context);

            // Assert
            targetMock.Verify(t => t.AddTag("Poisoned"), Times.Once);
            targetMock.Verify(t => t.RemoveTag(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void Execute_ActionRemove_CallsRemoveTagOnTargets()
        {
            // Arrange
            var effect = new ModifyTagEffect("Stunned", TagAction.Remove, "Remove Stun", "OnPlay");

            var targetMock = new Mock<ITargetable>();
            var context = new EffectContext(new Mock<ICard>().Object, new PlayerBuilder("Source").Build());
            context.Targets.Add(targetMock.Object);

            // Act
            effect.execute(context);

            // Assert
            targetMock.Verify(t => t.RemoveTag("Stunned"), Times.Once);
            targetMock.Verify(t => t.AddTag(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void Execute_MultipleTargets_AppliesToAll()
        {
            // Arrange
            var effect = new ModifyTagEffect("Flying", TagAction.Add, "Add Flying", "OnPlay");

            var targetMock1 = new Mock<ITargetable>();
            var targetMock2 = new Mock<ITargetable>();
            var context = new EffectContext(new Mock<ICard>().Object, new PlayerBuilder("Source").Build());
            context.Targets.Add(targetMock1.Object);
            context.Targets.Add(targetMock2.Object);

            // Act
            effect.execute(context);

            // Assert
            targetMock1.Verify(t => t.AddTag("Flying"), Times.Once);
            targetMock2.Verify(t => t.AddTag("Flying"), Times.Once);
        }
    }
}