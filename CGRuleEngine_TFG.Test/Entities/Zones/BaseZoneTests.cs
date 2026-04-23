using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Zones;

namespace CGRuleEngine_TFG.Tests.Entities.Zones
{
    // Dummy implementation of BaseZone to test its concrete methods
    public class TestZone : BaseZone
    {
        public override void Initialize() { }
        public override ICard PullCard() => null;
    }

    public class BaseZoneTests
    {
        [Fact]
        public void AddCard_ResetOnEntryTrue_AddsCardAndResetsIt()
        {
            // Arrange
            var zone = new TestZone { ResetOnEntry = true };
            var cardMock = new Mock<ICard>();

            // Act
            zone.AddCard(cardMock.Object);

            // Assert
            Assert.Contains(cardMock.Object, zone.Content);
            cardMock.Verify(c => c.Reset(), Times.Once);
            cardMock.VerifySet(c => c.CurrentZone = zone, Times.Once);
        }

        [Fact]
        public void AddCard_ResetOnEntryFalse_AddsCardWithoutResetting()
        {
            // Arrange
            var zone = new TestZone { ResetOnEntry = false };
            var cardMock = new Mock<ICard>();

            // Act
            zone.AddCard(cardMock.Object);

            // Assert
            Assert.Contains(cardMock.Object, zone.Content);
            cardMock.Verify(c => c.Reset(), Times.Never);
            cardMock.VerifySet(c => c.CurrentZone = zone, Times.Once);
        }

        [Fact]
        public void AddCard_WithOwners_SetsControllerToFirstOwner()
        {
            // Arrange
            var zone = new TestZone();
            var targetPlayer = new PlayerBuilder("TestPlayer").Build();
            zone.Owners.Add(targetPlayer);
            var cardMock = new Mock<ICard>();

            // Act
            zone.AddCard(cardMock.Object);

            // Assert
            cardMock.VerifySet(c => c.Controller = targetPlayer, Times.Once);
        }

        [Fact]
        public void AddCard_WithoutOwners_SetsControllerToNull()
        {
            // Arrange
            var zone = new TestZone();
            zone.Owners.Clear(); // Explicitly clear to ensure edge case
            var cardMock = new Mock<ICard>();

            // Act
            zone.AddCard(cardMock.Object);

            // Assert
            cardMock.VerifySet(c => c.Controller = null, Times.Once);
        }

        [Fact]
        public void RemoveCard_CardInZone_RemovesCardAndNullifiesProperties()
        {
            // Arrange
            var zone = new TestZone();
            var cardMock = new Mock<ICard>();
            zone.Content.Add(cardMock.Object); // Card is already in the zone

            // Act
            zone.RemoveCard(cardMock.Object);

            // Assert
            Assert.DoesNotContain(cardMock.Object, zone.Content);
            cardMock.VerifySet(c => c.Controller = null, Times.Once);
            cardMock.VerifySet(c => c.CurrentZone = null, Times.Once);
        }

        [Fact]
        public void RemoveCard_CardNotInZone_ThrowsInvalidOperationException()
        {
            // Arrange
            var zone = new TestZone();
            var cardMock = new Mock<ICard>();
            // Card is deliberately not added to the zone

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => zone.RemoveCard(cardMock.Object));
            Assert.Equal("The card is not in this zone.", exception.Message);
        }

        [Fact]
        public void MoveCard_CardInZone_RemovesFromCurrentAndAddsToTarget()
        {
            // Arrange
            var sourceZone = new TestZone();
            var cardMock = new Mock<ICard>();
            sourceZone.Content.Add(cardMock.Object); // Add card directly to avoid resetting/triggering AddCard logic

            var targetZoneMock = new Mock<IZone>();

            // Act
            sourceZone.MoveCard(cardMock.Object, targetZoneMock.Object);

            // Assert
            Assert.DoesNotContain(cardMock.Object, sourceZone.Content); // Verifies removal from source
            targetZoneMock.Verify(tz => tz.AddCard(cardMock.Object), Times.Once); // Verifies addition to target
        }

        [Fact]
        public void MoveCard_CardNotInZone_ThrowsInvalidOperationException()
        {
            // Arrange
            var sourceZone = new TestZone();
            var targetZoneMock = new Mock<IZone>();
            var cardMock = new Mock<ICard>();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => sourceZone.MoveCard(cardMock.Object, targetZoneMock.Object));
            targetZoneMock.Verify(tz => tz.AddCard(It.IsAny<ICard>()), Times.Never);
        }

        [Fact]
        public void Search_CardExists_ReturnsCard()
        {
            // Arrange
            var zone = new TestZone();
            var cardMock = new Mock<ICard>();
            cardMock.Setup(c => c.Name).Returns("Dragon");

            var otherCardMock = new Mock<ICard>();
            otherCardMock.Setup(c => c.Name).Returns("Goblin");

            zone.Content.Add(otherCardMock.Object);
            zone.Content.Add(cardMock.Object);

            // Act
            var result = zone.Search("Dragon");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Dragon", result.Name);
            Assert.Same(cardMock.Object, result);
        }

        [Fact]
        public void Search_CardDoesNotExist_ReturnsNull()
        {
            // Arrange
            var zone = new TestZone();
            var cardMock = new Mock<ICard>();
            cardMock.Setup(c => c.Name).Returns("Dragon");
            zone.Content.Add(cardMock.Object);

            // Act
            var result = zone.Search("Elf");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Search_EmptyZone_ReturnsNull()
        {
            // Arrange
            var zone = new TestZone();

            // Act
            var result = zone.Search("Dragon");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Search_NullName_ReturnsNull()
        {
            // Arrange
            var zone = new TestZone();
            var cardMock = new Mock<ICard>();
            cardMock.Setup(c => c.Name).Returns("Dragon");
            zone.Content.Add(cardMock.Object);

            // Act
            var result = zone.Search(null);

            // Assert
            Assert.Null(result);
        }
    }
}