using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Players.Controllers;
using CGRuleEngine_TFG.Entities.Zones;
using CGRuleEngine_TFG.Execution;
using CGRuleEngine_TFG.Execution.Effects;
using CGRuleEngine_TFG.GameSystem;
using CGRuleEngine_TFG.GameSystem.TurnSystem;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace CGRuleEngine_TFG.Tests.GameSystem
{
    public class GameTests
    {
        // Helper class to be able to instance concrete phases for TurnFactory and Turn testing
        public class DummyPhase : Phase
        {
            public DummyPhase(string name) : base(name) { }
        }

        #region initializeGame Tests

        [Fact]
        public void InitializeGame_PlayersWithZones_InitializesAllZones()
        {
            // Arrange
            var zoneMock1 = new Mock<IZone>();
            var zoneMock2 = new Mock<IZone>();

            var player1 = new PlayerBuilder("Player1").WithZone("Deck", zoneMock1.Object).Build();
            var player2 = new PlayerBuilder("Player2").WithZone("Hand", zoneMock2.Object).Build();

            var game = new GameBuilder().WithPlayer(player1).WithPlayer(player2).Build();

            // Act
            game.initializeGame();

            // Assert
            zoneMock1.Verify(z => z.Initialize(), Times.Once);
            zoneMock2.Verify(z => z.Initialize(), Times.Once);
        }

        [Fact]
        public void InitializeGame_PlayersWithNoZones_DoesNotThrowException()
        {
            // Arrange
            var player1 = new PlayerBuilder("Player1").Build();
            var game = new GameBuilder().WithPlayer(player1).Build();

            // Act
            var exception = Record.Exception(() => game.initializeGame());

            // Assert
            Assert.Null(exception);
        }

        #endregion

        #region startGame Tests

        [Fact]
        public void StartGame_IsGameOver_ThrowsInvalidOperationException()
        {
            // Arrange
            var game = new GameBuilder().Build();
            game.IsGameOver = true;

            // Act
            var exception = Assert.Throws<InvalidOperationException>(() => game.startGame());

            // Assert
            Assert.Equal("The game is already over", exception.Message);
        }

        [Fact]
        public void StartGame_StackIsNotEmpty_ThrowsInvalidOperationException()
        {
            // Arrange
            var game = new GameBuilder().Build();
            game.Stack = new ActionStack(game);

            // Push a valid StackEntry to simulate a non-empty stack
            game.Stack.Push(new StackEntry());

            // Act
            var exception = Assert.Throws<InvalidOperationException>(() => game.startGame());

            // Assert
            Assert.Equal("The stack is not empty, cannot start the game", exception.Message);
        }

        #endregion

        #region nextTurn() Tests

        [Fact]
        public void NextTurn_IsGameOver_ThrowsInvalidOperationException()
        {
            // Arrange
            var game = new GameBuilder().Build();
            game.IsGameOver = true;

            // Act
            var exception = Assert.Throws<InvalidOperationException>(() => game.nextTurn());

            // Assert
            Assert.Equal("The game is already over", exception.Message);
        }

       

        #endregion

        #region nextTurn(Player) Tests

        [Fact]
        public void NextTurnWithPlayer_IsGameOver_ThrowsInvalidOperationException()
        {
            // Arrange
            var player = new PlayerBuilder("Player1").Build();
            var game = new GameBuilder().Build();
            game.IsGameOver = true;

            // Act
            var exception = Assert.Throws<InvalidOperationException>(() => game.nextTurn(player));

            // Assert
            Assert.Equal("The game is already over", exception.Message);
        }

        #endregion

        #region checkWinCondition Tests

        [Fact]
        public void CheckWinCondition_GameAlreadyOver_DoesNothing()
        {
            // Arrange
            var player1 = new PlayerBuilder("Player1").Build();
            var game = new GameBuilder().WithPlayer(player1).Build();
            game.IsGameOver = true;

            // Act
            game.checkWinCondition();

            // Assert
            Assert.Empty(game.results); // Ensure EndGame wasn't called again
        }

        #endregion

        #region getRank Tests

        [Fact]
        public void GetRank_PlayerInResults_ReturnsRank()
        {
            // Arrange
            var player = new PlayerBuilder("Player1").Build();
            var game = new GameBuilder().Build();
            game.results.Add(player, 1);

            // Act
            int rank = game.getRank(player);

            // Assert
            Assert.Equal(1, rank);
        }

        [Fact]
        public void GetRank_PlayerNotInResults_ThrowsKeyNotFoundException()
        {
            // Arrange
            var player = new PlayerBuilder("Player1").Build();
            var game = new GameBuilder().Build();

            // Act
            var exception = Assert.Throws<KeyNotFoundException>(() => game.getRank(player));

            // Assert
            Assert.NotNull(exception);
        }

        #endregion

        #region PassPriority Tests

        [Fact]
        public void PassPriority_NotPriorityPlayer_DoesNothing()
        {
            // Arrange
            var player1 = new PlayerBuilder("Player1").Build();
            var player2 = new PlayerBuilder("Player2").Build();

            var game = new GameBuilder().WithPlayer(player1).WithPlayer(player2).Build();
            game.PriorityPlayer = player1;

            // Act
            game.PassPriority(player2);

            // Assert
            Assert.Same(player1, game.PriorityPlayer); // Priority did not change
        }

        [Fact]
        public void PassPriority_IsPriorityPlayer_PassesToNextPlayer()
        {
            // Arrange
            var player1 = new PlayerBuilder("Player1").Build();
            var player2 = new PlayerBuilder("Player2").Build();

            var controllerMock = new Mock<IPlayerController>();
            player2.Controller = controllerMock.Object;

            var game = new GameBuilder().WithPlayer(player1).WithPlayer(player2).Build();
            game.PriorityPlayer = player1;

            // Act
            game.PassPriority(player1);

            // Assert
            Assert.Same(player2, game.PriorityPlayer);
            controllerMock.Verify(c => c.RequestAction(game), Times.Once); // Notified next player
        }

        #endregion

        #region EndGame Tests

        [Fact]
        public void EndGame_SetsIsGameOverToTrue_AssignsRanksCorrectly()
        {
            // Arrange
            var winner = new PlayerBuilder("Winner").Build();
            var loser1 = new PlayerBuilder("Loser1").Build();
            var loser2 = new PlayerBuilder("Loser2").Build();
            var game = new GameBuilder().WithPlayer(winner).WithPlayer(loser1).WithPlayer(loser2).Build();

            // Act
            game.EndGame(winner);

            // Assert
            Assert.True(game.IsGameOver);
            Assert.Equal(1, game.results[winner]);
            Assert.Equal(2, game.results[loser1]);
            Assert.Equal(3, game.results[loser2]);
        }

        #endregion

        #region ResetPriority Tests

        [Fact]
        public void ResetPriority_SetsPriorityPlayerToActivePlayer()
        {
            // Arrange
            var activePlayer = new PlayerBuilder("Active").Build();
            var otherPlayer = new PlayerBuilder("Other").Build();

            var game = new GameBuilder().WithPlayer(activePlayer).WithPlayer(otherPlayer).Build();
            game.ActivePlayer = activePlayer;
            game.PriorityPlayer = otherPlayer;

            // Act
            game.ResetPriority();

            // Assert
            Assert.Same(activePlayer, game.PriorityPlayer);
        }

        #endregion

        #region NotifyPriorityPlayer Tests

        [Fact]
        public void NotifyPriorityPlayer_GameActive_CallsRequestActionOnController()
        {
            // Arrange
            var player = new PlayerBuilder("Player1").Build();
            var controllerMock = new Mock<IPlayerController>();
            player.Controller = controllerMock.Object;

            var game = new GameBuilder().Build();
            game.PriorityPlayer = player;
            game.IsGameOver = false;

            // Act
            game.NotifyPriorityPlayer();

            // Assert
            controllerMock.Verify(c => c.RequestAction(game), Times.Once);
        }

        #endregion
    }
}