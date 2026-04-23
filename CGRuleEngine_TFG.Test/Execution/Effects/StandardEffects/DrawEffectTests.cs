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
    public class DrawEffectTests
    {
        [Fact]
        public void Execute_TargetIsPlayerWithEnoughCards_DrawsRequestedAmount()
        {
            // Arrange
            var effect = new DrawEffect("Draw 2", 2, "Deck", "Hand");

            var sourceZoneMock = new Mock<IZone>();
            var destZoneMock = new Mock<IZone>();

            var card1 = new Mock<ICard>().Object;
            var card2 = new Mock<ICard>().Object;

            // Setup sequence to return cards
            sourceZoneMock.SetupSequence(z => z.PullCard())
                .Returns(card1)
                .Returns(card2);

            var targetPlayer = new PlayerBuilder("TargetPlayer")
                .WithZone("Deck", sourceZoneMock.Object)
                .WithZone("Hand", destZoneMock.Object)
                .Build();

            var context = new EffectContext(new Mock<ICard>().Object, new PlayerBuilder("Source").Build());
            context.Targets.Add(targetPlayer);

            // Act
            effect.execute(context);

            // Assert
            sourceZoneMock.Verify(z => z.PullCard(), Times.Exactly(2));
            destZoneMock.Verify(z => z.AddCard(card1), Times.Once);
            destZoneMock.Verify(z => z.AddCard(card2), Times.Once);
        }

        [Fact]
        public void Execute_SourceZoneHasFewerCardsThanRequested_DrawsAvailableCardsAndStops()
        {
            // Arrange
            var effect = new DrawEffect("Draw 3", 3, "Deck", "Hand");

            var sourceZoneMock = new Mock<IZone>();
            var destZoneMock = new Mock<IZone>();

            var card1 = new Mock<ICard>().Object;

            // Setup sequence to return 1 card, then null (empty zone)
            sourceZoneMock.SetupSequence(z => z.PullCard())
                .Returns(card1)
                .Returns((ICard)null);

            var targetPlayer = new PlayerBuilder("TargetPlayer")
                .WithZone("Deck", sourceZoneMock.Object)
                .WithZone("Hand", destZoneMock.Object)
                .Build();

            var context = new EffectContext(new Mock<ICard>().Object, new PlayerBuilder("Source").Build());
            context.Targets.Add(targetPlayer);

            // Act
            effect.execute(context);

            // Assert
            sourceZoneMock.Verify(z => z.PullCard(), Times.Exactly(2)); // Called twice, second time returned null
            destZoneMock.Verify(z => z.AddCard(card1), Times.Once);
            destZoneMock.Verify(z => z.AddCard(It.IsAny<ICard>()), Times.Once); // Only 1 card was added total
        }

        [Fact]
        public void Execute_TargetIsNotPlayer_DoesNothing()
        {
            // Arrange
            var effect = new DrawEffect("Draw 1", 1, "Deck", "Hand");

            // ICard implements ITargetable but is not a Player
            var nonPlayerTargetMock = new Mock<ICard>();

            var context = new EffectContext(new Mock<ICard>().Object, new PlayerBuilder("Source").Build());
            context.Targets.Add(nonPlayerTargetMock.Object);

            // Act
            var exception = Record.Exception(() => effect.execute(context));

            // Assert
            Assert.Null(exception); // Should safely ignore non-player targets
        }
    }
}