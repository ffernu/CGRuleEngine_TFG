using System;
using System.Collections.Generic;
using Xunit;
using CGRuleEngine_TFG.GameSystem;
using CGRuleEngine_TFG.GameSystem.TurnSystem;

namespace CGRuleEngine_TFG.Tests.GameSystem.TurnSystem
{
    // Dummy implementation to test the abstract Phase class
    public class TestPhase : Phase
    {
        public bool OnEnterCalled { get; private set; }
        public bool OnExitCalled { get; private set; }

        public TestPhase(string name) : base(name) { }

        public override void OnEnter(Game game)
        {
            OnEnterCalled = true;
            base.OnEnter(game);
        }

        public override void OnExit(Game game)
        {
            OnExitCalled = true;
            base.OnExit(game);
        }
    }

    public class PhaseTests
    {
        [Fact]
        public void Constructor_SetsNameAndInitializesSubPhases()
        {
            // Arrange & Act
            var phase = new TestPhase("MainPhase");

            // Assert
            Assert.Equal("MainPhase", phase.Name);
            Assert.NotNull(phase.SubPhases);
            Assert.Empty(phase.SubPhases);
        }

        [Fact]
        public void OnEnter_ExecutesWithoutExceptions()
        {
            // Arrange
            var phase = new TestPhase("MainPhase");
            var game = new GameBuilder().Build();

            // Act
            phase.OnEnter(game);

            // Assert
            Assert.True(phase.OnEnterCalled);
        }

        [Fact]
        public void OnExit_ExecutesWithoutExceptions()
        {
            // Arrange
            var phase = new TestPhase("MainPhase");
            var game = new GameBuilder().Build();

            // Act
            phase.OnExit(game);

            // Assert
            Assert.True(phase.OnExitCalled);
        }
    }
}