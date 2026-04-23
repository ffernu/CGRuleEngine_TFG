using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using CGRuleEngine_TFG.GameSystem;
using CGRuleEngine_TFG.GameSystem.TurnSystem;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Zones;

namespace CGRuleEngine_TFG.Tests.GameSystem
{
    public class GameTests
    {
        // Helper class to be able to instance concrete phases for TurnFactory and Turn testing
        public class DummyPhase : Phase
        {
            public DummyPhase(string name) : base(name) { }
        }

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
        public void StartGame_IsGameOver_ThrowsInvalidOperationException()
        {
            // Arrange
            var game = new GameBuilder().Build();
            game.IsGameOver = true;

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => game.startGame());
            Assert.Equal("The game is already over", exception.Message);
        }

        [Fact]
        public void StartGame_ValidState_StartsGameAndSetsActivePlayer()
        {
            // Arrange
            var player1 = new PlayerBuilder("Player1").Build();
            var player2 = new PlayerBuilder("Player2").Build();

            // Using real TurnFactory to avoid NotSupportedException from Moq
            var turnFactory = new TurnFactory(new List<Phase> { new DummyPhase("Phase1") });

            var game = new GameBuilder()
                .WithPlayer(player1)
                .WithPlayer(player2)
                .SetTurnTemplate(turnFactory)
                .Build();

            // Act
            game.startGame();

            // Assert
            Assert.Same(player1, game.ActivePlayer);
            Assert.Equal(1, game.TurnNumber);
            Assert.NotNull(game.CurrentTurn);
            Assert.Equal(0, game.CurrentTurn.CurrentPhaseIndex); // Verify turn actually started
        }

        [Fact]
        public void NextTurn_IsGameOver_ThrowsInvalidOperationException()
        {
            // Arrange
            var game = new GameBuilder().Build();
            game.IsGameOver = true;

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => game.nextTurn());
            Assert.Equal("The game is already over", exception.Message);
        }

        [Fact]
        public void NextTurn_CyclesToNextPlayerAndStartsTurn()
        {
            // Arrange
            var player1 = new PlayerBuilder("Player1").Build();
            var player2 = new PlayerBuilder("Player2").Build();

            var turnFactory = new TurnFactory(new List<Phase> { new DummyPhase("Phase1") });

            var game = new GameBuilder()
                .WithPlayer(player1)
                .WithPlayer(player2)
                .SetTurnTemplate(turnFactory)
                .Build();

            game.ActivePlayer = player1;
            game.TurnNumber = 1;

            // Act
            game.nextTurn();

            // Assert
            Assert.Same(player2, game.ActivePlayer); // Cycled to player 2
            Assert.Equal(2, game.TurnNumber);
            Assert.NotNull(game.CurrentTurn);
            Assert.Same(player2, game.CurrentTurn.ActivePlayer);
        }

        [Fact]
        public void NextTurnWithPlayer_IsGameOver_ThrowsInvalidOperationException()
        {
            // Arrange
            var player = new PlayerBuilder("Player1").Build();
            var game = new GameBuilder().Build();
            game.IsGameOver = true;

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => game.nextTurn(player));
            Assert.Equal("The game is already over", exception.Message);
        }

        [Fact]
        public void NextTurnWithPlayer_ValidPlayer_SetsActivePlayerAndStartsTurn()
        {
            // Arrange
            var targetPlayer = new PlayerBuilder("TargetPlayer").Build();
            var turnFactory = new TurnFactory(new List<Phase> { new DummyPhase("Phase1") });

            var game = new GameBuilder()
                .SetTurnTemplate(turnFactory)
                .Build();

            game.TurnNumber = 5;

            // Act
            game.nextTurn(targetPlayer);

            // Assert
            Assert.Same(targetPlayer, game.ActivePlayer);
            Assert.Equal(6, game.TurnNumber);
            Assert.NotNull(game.CurrentTurn);
            Assert.Same(targetPlayer, game.CurrentTurn.ActivePlayer);
        }

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

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => game.getRank(player));
        }

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
            Assert.Same(player1, game.PriorityPlayer);
        }

        [Fact]
        public void PassPriority_IsPriorityPlayer_PassesToNextPlayer()
        {
            // Arrange
            var player1 = new PlayerBuilder("Player1").Build();
            var player2 = new PlayerBuilder("Player2").Build();

            var game = new GameBuilder().WithPlayer(player1).WithPlayer(player2).Build();
            game.PriorityPlayer = player1;

            // Act
            game.PassPriority(player1);

            // Assert
            Assert.Same(player2, game.PriorityPlayer);
        }

        [Fact]
        public void PassPriority_AllPlayersPassed_HasNextPhase_AdvancesPhase()
        {
            // Arrange
            var player1 = new PlayerBuilder("Player1").Build();
            var player2 = new PlayerBuilder("Player2").Build();

            // Turn with 2 phases so it doesn't end immediately
            var turn = new Turn(player1, new List<Phase> { new DummyPhase("P1"), new DummyPhase("P2") });

            var game = new GameBuilder().WithPlayer(player1).WithPlayer(player2).Build();
            turn.Start(game); // Starts at phase 0

            game.CurrentTurn = turn;
            game.ActivePlayer = player1;
            game.PriorityPlayer = player1;

            // Act
            game.PassPriority(player1); // pass 1
            game.PassPriority(player2); // pass 2 (triggers next phase logic)

            // Assert
            Assert.Same(player1, game.PriorityPlayer); // Priority reset to ActivePlayer
            Assert.Equal(1, turn.CurrentPhaseIndex); // Phase advanced to 1
        }

        [Fact]
        public void PassPriority_AllPlayersPassed_NoMorePhases_StartsNextTurn()
        {
            // Arrange
            var player1 = new PlayerBuilder("Player1").Build();
            var player2 = new PlayerBuilder("Player2").Build();

            // Real TurnFactory to be used by game.nextTurn()
            var turnFactory = new TurnFactory(new List<Phase> { new DummyPhase("NewTurnPhase") });

            // Initial turn with only 1 phase so it ends immediately
            var turn = new Turn(player1, new List<Phase> { new DummyPhase("OldTurnPhase") });

            var game = new GameBuilder()
                .WithPlayer(player1)
                .WithPlayer(player2)
                .SetTurnTemplate(turnFactory)
                .Build();

            turn.Start(game);

            game.CurrentTurn = turn;
            game.ActivePlayer = player1;
            game.PriorityPlayer = player1;
            game.TurnNumber = 1;

            // Act
            game.PassPriority(player1); // pass 1
            game.PassPriority(player2); // pass 2 (triggers end of phase, then end of turn)

            // Assert
            Assert.Equal(2, game.TurnNumber); // Verify that nextTurn() was successfully triggered
            Assert.Same(player2, game.ActivePlayer); // Next player is now active
            Assert.NotSame(turn, game.CurrentTurn); // Current turn object has been replaced by TurnFactory
        }

        [Fact]
        public void EndGame_SetsIsGameOverToTrue()
        {
            // Arrange
            var player = new PlayerBuilder("Player1").Build();
            var game = new GameBuilder().Build();

            // Act
            game.EndGame(player);

            // Assert
            Assert.True(game.IsGameOver);
        }

        [Fact]
        public void ResetPriority_SetsPriorityPlayerToActivePlayerAndResetsPasses()
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
    }
}