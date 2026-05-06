using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using CGRuleEngine_TFG.Execution;
using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Zones;
using CGRuleEngine_TFG.Execution.Effects.StandardEffects;

namespace CGRuleEngine_TFG.Tests.Execution.Effects.StandardEffects
{
    public class MoveCardEffectTests
    {
        [Fact]
        public void Constructor_SetsPropertiesCorrectly()
        {
            // Arrange
            string expectedDebugName = "Test Move";
            string expectedDestZone = "Graveyard";
            string expectedTiming = "OnPlay";

            // Act
            var effect = new MoveCardEffect(expectedDebugName, expectedDestZone, expectedTiming);

            // Assert
            Assert.Equal(expectedDebugName, effect.DebugName);
            Assert.Equal(expectedDestZone, effect.DestinationZoneName);
            Assert.Equal(expectedTiming, effect.Timing);
        }

        [Fact]
        public void Execute_TargetIsCardWithValidZones_MovesCardToDestination()
        {
            // Arrange
            string destZoneName = "Graveyard";
            var effect = new MoveCardEffect("Test Move", destZoneName, "OnPlay");

            var sourceZoneMock = new Mock<IZone>();
            var destZoneMock = new Mock<IZone>();

            // Setup the player with the destination zone
            var sourcePlayer = new PlayerBuilder("SourcePlayer")
                .WithZone(destZoneName, destZoneMock.Object)
                .Build();

            // Setup the card to be moved
            var targetCardMock = new Mock<ICard>();
            targetCardMock.Setup(c => c.CurrentZone).Returns(sourceZoneMock.Object);

            // Create context
            var context = new EffectContext(new Mock<ICard>().Object, sourcePlayer);
            context.Targets.Add(targetCardMock.Object);

            // Act
            effect.execute(context);

            // Assert
            sourceZoneMock.Verify(z => z.RemoveCard(targetCardMock.Object), Times.Once);
            destZoneMock.Verify(z => z.AddCard(targetCardMock.Object), Times.Once);
        }

        [Fact]
        public void Execute_TargetIsNotCard_DoesNothing()
        {
            // Arrange
            string destZoneName = "Hand";
            var effect = new MoveCardEffect("Test Move", destZoneName, "OnPlay");
            var destZoneMock = new Mock<IZone>();

            var sourcePlayer = new PlayerBuilder("SourcePlayer")
                .WithZone(destZoneName, destZoneMock.Object)
                .Build();

            // A player is ITargetable but not an ICard
            var nonCardTarget = new PlayerBuilder("TargetPlayer").Build();

            var context = new EffectContext(new Mock<ICard>().Object, sourcePlayer);
            context.Targets.Add(nonCardTarget);

            // Act
            effect.execute(context);

            // Assert
            destZoneMock.Verify(z => z.AddCard(It.IsAny<ICard>()), Times.Never);
        }

        [Fact]
        public void Execute_DestinationZoneDoesNotExist_DoesNothing()
        {
            // Arrange
            var effect = new MoveCardEffect("Test Move", "NonExistentZone", "OnPlay");

            // Player does not have the specified zone
            var sourcePlayer = new PlayerBuilder("SourcePlayer").Build();

            var sourceZoneMock = new Mock<IZone>();
            var targetCardMock = new Mock<ICard>();
            targetCardMock.Setup(c => c.CurrentZone).Returns(sourceZoneMock.Object);

            var context = new EffectContext(new Mock<ICard>().Object, sourcePlayer);
            context.Targets.Add(targetCardMock.Object);

            // Act
            var exception = Record.Exception(() => effect.execute(context));

            // Assert
            // Expecting InvalidOperationException from getZone if zone doesn't exist 
            // as defined in Player.cs. If it throws, the test captures it. 
            // (Note: The implementation of Player.getZone throws if not found)
            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
            sourceZoneMock.Verify(z => z.RemoveCard(It.IsAny<ICard>()), Times.Never);
        }

        [Fact]
        public void Execute_CardHasNoCurrentZone_DoesNothing()
        {
            // Arrange
            string destZoneName = "Graveyard";
            var effect = new MoveCardEffect("Test Move", destZoneName, "OnPlay");

            var destZoneMock = new Mock<IZone>();
            var sourcePlayer = new PlayerBuilder("SourcePlayer")
                .WithZone(destZoneName, destZoneMock.Object)
                .Build();

            var targetCardMock = new Mock<ICard>();
            targetCardMock.Setup(c => c.CurrentZone).Returns((IZone)null); // No current zone

            var context = new EffectContext(new Mock<ICard>().Object, sourcePlayer);
            context.Targets.Add(targetCardMock.Object);

            // Act
            effect.execute(context);

            // Assert
            destZoneMock.Verify(z => z.AddCard(It.IsAny<ICard>()), Times.Never);
        }
    }
}