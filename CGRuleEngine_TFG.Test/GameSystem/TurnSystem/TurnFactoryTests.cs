using System;
using System.Collections.Generic;
using Xunit;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.GameSystem.TurnSystem;

namespace CGRuleEngine_TFG.Tests.GameSystem.TurnSystem
{
    public class TurnFactoryTests
    {
        [Fact]
        public void Constructor_FlatPhases_KeepsListFlat()
        {
            // Arrange
            var phase1 = new TestPhase("Phase1");
            var phase2 = new TestPhase("Phase2");
            var phases = new List<Phase> { phase1, phase2 };

            // Act
            var factory = new TurnFactory(phases);

            // Assert
            Assert.Equal(2, factory.Phases.Count);
            Assert.Contains(phase1, factory.Phases);
            Assert.Contains(phase2, factory.Phases);
        }

        [Fact]
        public void Constructor_NestedPhases_FlattensHierarchy()
        {
            // Arrange
            var subPhase = new TestPhase("SubPhase");
            var mainPhase = new TestPhase("MainPhase")
            {
                SubPhases = new List<Phase> { subPhase }
            };

            var phases = new List<Phase> { mainPhase };

            // Act
            var factory = new TurnFactory(phases);

            // Assert
            Assert.Equal(2, factory.Phases.Count);
            // It should add the parent first, then the children
            Assert.Same(mainPhase, factory.Phases[0]);
            Assert.Same(subPhase, factory.Phases[1]);
        }

        [Fact]
        public void CreateTurn_ValidPlayer_ReturnsTurnWithFlattendedPhases()
        {
            // Arrange
            var subPhase = new TestPhase("SubPhase");
            var mainPhase = new TestPhase("MainPhase") { SubPhases = new List<Phase> { subPhase } };

            var factory = new TurnFactory(new List<Phase> { mainPhase });
            var player = new PlayerBuilder("Player1").Build();

            // Act
            var turn = factory.CreateTurn(player);

            // Assert
            Assert.NotNull(turn);
            Assert.Same(player, turn.ActivePlayer);
            Assert.Equal(2, turn.Phases.Count); // Ensure it got the flattened list
        }
    }
}