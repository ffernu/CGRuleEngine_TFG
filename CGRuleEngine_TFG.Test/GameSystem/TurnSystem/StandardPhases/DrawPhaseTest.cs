using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using CGRuleEngine_TFG.GameSystem;
using CGRuleEngine_TFG.GameSystem.TurnSystem.StandardPhases;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Zones;
using CGRuleEngine_TFG.Entities.Cards;

namespace CGRuleEngine_TFG.Tests.GameSystem.TurnSystem.StandardPhases
{
    public class DrawPhaseTests
    {
        [Fact]
        public void Constructor_Always_SetsNameCorrectly()
        {
            // Arrange & Act
            var phase = new DrawPhase();

            // Assert
            Assert.Equal("Draw phase", phase.Name);
        }

        [Fact]
        public void OnEnter_ActivePlayerIsNull_ReturnsEarlyWithoutResettingPriority()
        {
            // Arrange
            var phase = new DrawPhase();
            var game = new GameBuilder().Build();
            game.ActivePlayer = null;

            var priorityPlayer = new PlayerBuilder("OtherPlayer").Build();
            game.PriorityPlayer = priorityPlayer;

            // Act
            phase.OnEnter(game);

            // Assert
            // PriorityPlayer should remain unchanged because it returns early
            Assert.Same(priorityPlayer, game.PriorityPlayer);
        }

        [Fact]
        public void OnEnter_MissingDeckZone_DoesNotDrawButResetsPriority()
        {
            // Arrange
            var phase = new DrawPhase();
            var handMock = new Mock<IZone>();

            // Player has "Hand" but no "Deck"
            var player = new PlayerBuilder("Player1")
                .WithZone("Hand", handMock.Object)
                .Build();

            var game = new GameBuilder().WithPlayer(player).Build();
            game.ActivePlayer = player;
            game.PriorityPlayer = null; // Ensuring it's null before the method call

            // Act
            phase.OnEnter(game);

            // Assert
            handMock.Verify(z => z.AddCard(It.IsAny<ICard>()), Times.Never);
            Assert.Same(player, game.PriorityPlayer); // Validates game.ResetPriority() was called
        }

        [Fact]
        public void OnEnter_MissingHandZone_DoesNotDrawButResetsPriority()
        {
            // Arrange
            var phase = new DrawPhase();
            var deckMock = new Mock<IZone>();

            // Player has "Deck" but no "Hand"
            var player = new PlayerBuilder("Player1")
                .WithZone("Deck", deckMock.Object)
                .Build();

            var game = new GameBuilder().WithPlayer(player).Build();
            game.ActivePlayer = player;
            game.PriorityPlayer = null;

            // Act
            phase.OnEnter(game);

            // Assert
            deckMock.Verify(z => z.PullCard(), Times.Never);
            Assert.Same(player, game.PriorityPlayer); // Validates game.ResetPriority() was called
        }

        [Fact]
        public void OnEnter_DeckIsEmpty_DoesNotDrawButResetsPriority()
        {
            // Arrange
            var phase = new DrawPhase();
            var deckMock = new Mock<IZone>();
            var handMock = new Mock<IZone>();

            // Simulate an empty deck
            deckMock.Setup(z => z.Content).Returns(new List<ICard>());

            var player = new PlayerBuilder("Player1")
                .WithZone("Deck", deckMock.Object)
                .WithZone("Hand", handMock.Object)
                .Build();

            var game = new GameBuilder().WithPlayer(player).Build();
            game.ActivePlayer = player;
            game.PriorityPlayer = null;

            // Act
            phase.OnEnter(game);

            // Assert
            deckMock.Verify(z => z.PullCard(), Times.Never);
            handMock.Verify(z => z.AddCard(It.IsAny<ICard>()), Times.Never);
            Assert.Same(player, game.PriorityPlayer); // Validates game.ResetPriority() was called
        }

        [Fact]
        public void OnEnter_DeckHasCards_PullsFromDeckAndAddsToHandAndResetsPriority()
        {
            // Arrange
            var phase = new DrawPhase();
            var deckMock = new Mock<IZone>();
            var handMock = new Mock<IZone>();
            var cardMock = new Mock<ICard>();

            // Simulate a deck with one card
            deckMock.Setup(z => z.Content).Returns(new List<ICard> { cardMock.Object });
            deckMock.Setup(z => z.PullCard()).Returns(cardMock.Object);

            var player = new PlayerBuilder("Player1")
                .WithZone("Deck", deckMock.Object)
                .WithZone("Hand", handMock.Object)
                .Build();

            var game = new GameBuilder().WithPlayer(player).Build();
            game.ActivePlayer = player;
            game.PriorityPlayer = null;

            // Act
            phase.OnEnter(game);

            // Assert
            deckMock.Verify(z => z.PullCard(), Times.Once);
            handMock.Verify(z => z.AddCard(cardMock.Object), Times.Once);
            Assert.Same(player, game.PriorityPlayer); // Validates game.ResetPriority() was called
        }
    }
}