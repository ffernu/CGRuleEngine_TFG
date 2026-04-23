using System;
using System.Collections.Generic;
using Xunit;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.GameSystem;
using CGRuleEngine_TFG.GameSystem.TurnSystem;

namespace CGRuleEngine_TFG.Tests.GameSystem.TurnSystem
{
    public class TurnTests
    {
        [Fact]
        public void Constructor_SetsPlayerAndPhases()
        {
            // Arrange
            var player = new PlayerBuilder("Player1").Build();
            var phases = new List<Phase> { new TestPhase("Phase1") };

            // Act
            var turn = new Turn(player, phases);

            // Assert
            Assert.Same(player, turn.ActivePlayer);
            Assert.Same(phases, turn.Phases);
            Assert.Null(turn.CurrentPhase);
            Assert.Equal(0, turn.CurrentPhaseIndex);
        }

        [Fact]
        public void Start_EmptyPhases_ThrowsInvalidOperationException()
        {
            // Arrange
            var player = new PlayerBuilder("Player1").Build();
            var turn = new Turn(player, new List<Phase>());
            var game = new GameBuilder().Build();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => turn.Start(game));
            Assert.Equal("The turn must have at least one phase to start", exception.Message);
        }

        [Fact]
        public void Start_HasPhases_SetsCurrentPhaseAndCallsOnEnter()
        {
            // Arrange
            var player = new PlayerBuilder("Player1").Build();
            var phase = new TestPhase("Phase1");
            var turn = new Turn(player, new List<Phase> { phase });
            var game = new GameBuilder().Build();

            // Act
            turn.Start(game);

            // Assert
            Assert.Equal(0, turn.CurrentPhaseIndex);
            Assert.Same(phase, turn.CurrentPhase);
            Assert.True(phase.OnEnterCalled);
        }

        [Fact]
        public void NextPhase_HasMorePhases_ReturnsTrueAndAdvancesPhase()
        {
            // Arrange
            var player = new PlayerBuilder("Player1").Build();
            var phase1 = new TestPhase("Phase1");
            var phase2 = new TestPhase("Phase2");
            var turn = new Turn(player, new List<Phase> { phase1, phase2 });
            var game = new GameBuilder().Build();

            turn.Start(game);

            // Act
            bool result = turn.NextPhase(game);

            // Assert
            Assert.True(result);
            Assert.Equal(1, turn.CurrentPhaseIndex);
            Assert.Same(phase2, turn.CurrentPhase);
            Assert.True(phase1.OnExitCalled);
            Assert.True(phase2.OnEnterCalled);
        }

        [Fact]
        public void NextPhase_NoMorePhases_ReturnsFalseAndCallsOnExit()
        {
            // Arrange
            var player = new PlayerBuilder("Player1").Build();
            var phase1 = new TestPhase("Phase1");
            var turn = new Turn(player, new List<Phase> { phase1 });
            var game = new GameBuilder().Build();

            turn.Start(game);

            // Act
            bool result = turn.NextPhase(game);

            // Assert
            Assert.False(result);
            Assert.True(phase1.OnExitCalled);
            Assert.Equal(0, turn.CurrentPhaseIndex); // Index shouldn't advance past the last valid one
        }

        [Fact]
        public void GetCurrentPhase_ReturnsCurrentPhase()
        {
            // Arrange
            var player = new PlayerBuilder("Player1").Build();
            var phase = new TestPhase("Phase1");
            var turn = new Turn(player, new List<Phase> { phase });
            var game = new GameBuilder().Build();

            turn.Start(game);

            // Act
            var current = turn.GetCurrentPhase();

            // Assert
            Assert.Same(phase, current);
        }
    }
}