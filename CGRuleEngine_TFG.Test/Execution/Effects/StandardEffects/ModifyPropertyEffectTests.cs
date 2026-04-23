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
    public class ModifyPropertyEffectTests
    {
        [Fact]
        public void Execute_OperationAdd_AddsValueToProperty()
        {
            // Arrange
            var effect = new ModifyPropertyEffect(5, "Health", OperationType.Add, "Heal");

            int initialValue = 10;
            var targetMock = new Mock<ITargetable>();
            targetMock.Setup(t => t.TryGetProperty<int>("Health", out initialValue)).Returns(true);

            var context = new EffectContext(new Mock<ICard>().Object, new PlayerBuilder("Source").Build());
            context.Targets.Add(targetMock.Object);

            // Act
            effect.execute(context);

            // Assert
            targetMock.Verify(t => t.SetProperty("Health", 15), Times.Once); // 10 + 5
        }

        [Fact]
        public void Execute_OperationSubtract_SubtractsValueFromProperty()
        {
            // Arrange
            var effect = new ModifyPropertyEffect(3, "Armor", OperationType.Subtract, "Damage Armor");

            int initialValue = 10;
            var targetMock = new Mock<ITargetable>();
            targetMock.Setup(t => t.TryGetProperty<int>("Armor", out initialValue)).Returns(true);

            var context = new EffectContext(new Mock<ICard>().Object, new PlayerBuilder("Source").Build());
            context.Targets.Add(targetMock.Object);

            // Act
            effect.execute(context);

            // Assert
            targetMock.Verify(t => t.SetProperty("Armor", 7), Times.Once); // 10 - 3
        }

        [Fact]
        public void Execute_OperationSet_SetsPropertyToExactValue()
        {
            // Arrange
            var effect = new ModifyPropertyEffect(50, "Mana", OperationType.Set, "Set Mana");

            int initialValue = 10;
            var targetMock = new Mock<ITargetable>();
            targetMock.Setup(t => t.TryGetProperty<int>("Mana", out initialValue)).Returns(true);

            var context = new EffectContext(new Mock<ICard>().Object, new PlayerBuilder("Source").Build());
            context.Targets.Add(targetMock.Object);

            // Act
            effect.execute(context);

            // Assert
            targetMock.Verify(t => t.SetProperty("Mana", 50), Times.Once); // Overwrites 10 with 50
        }

        [Fact]
        public void Execute_PropertyDoesNotExist_DoesNotAttemptToModify()
        {
            // Arrange
            var effect = new ModifyPropertyEffect(5, "Speed", OperationType.Add, "Buff Speed");

            int initialValue = 0;
            var targetMock = new Mock<ITargetable>();
            targetMock.Setup(t => t.TryGetProperty<int>("Speed", out initialValue)).Returns(false); // Fails to get

            var context = new EffectContext(new Mock<ICard>().Object, new PlayerBuilder("Source").Build());
            context.Targets.Add(targetMock.Object);

            // Act
            effect.execute(context);

            // Assert
            targetMock.Verify(t => t.SetProperty(It.IsAny<string>(), It.IsAny<object>()), Times.Never);
        }
    }
}