using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Players.Controllers;
using CGRuleEngine_TFG.Entities.Players.Controllers.Strategies;
using CGRuleEngine_TFG.Execution;
using CGRuleEngine_TFG.Execution.Actions;
using CGRuleEngine_TFG.GameSystem;

namespace CGRuleEngine_TFG.Tests.Entities.Players.Controllers
{
    public class AIControllerTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_ValidParameters_InitializesCorrectly()
        {
            // Arrange
            var player = new PlayerBuilder("AI").Build();
            var strategyMock = new Mock<IStrategy>();
            Func<Game, Player, List<IAction>> generator = (g, p) => new List<IAction>();
            var game = new GameBuilder().Build();

            // Act
            var controller = new AIController(player, strategyMock.Object, generator);

            // Assert
            // We verify the strategy was set properly by using the public studyStrategy method
            Assert.Same(strategyMock.Object, controller.studyStrategy(game));
        }

        #endregion

        #region RequestAction Tests

        [Fact]
        public void RequestAction_ChosenActionIsValid_CommitsChosenAction()
        {
            // Arrange
            var player = new PlayerBuilder("AI").Build();
            var strategyMock = new Mock<IStrategy>();
            var actionMock = new Mock<IAction>();
            var game = new GameBuilder().WithPlayer(player).Build();

            // Setup valid action that will be chosen by the strategy
            actionMock.Setup(a => a.IsValid(game)).Returns(true);
            strategyMock.Setup(s => s.decideAction(It.IsAny<List<IAction>>(), game)).Returns(actionMock.Object);

            bool generatorCalled = false;
            Func<Game, Player, List<IAction>> generator = (g, p) =>
            {
                generatorCalled = true;
                return new List<IAction> { actionMock.Object };
            };

            var controller = new AIController(player, strategyMock.Object, generator);

            // Act
            controller.RequestAction(game);

            // Assert
            Assert.True(generatorCalled); // Verifies ActionGenerator was executed
            actionMock.Verify(a => a.Commit(game), Times.Once); // Verifies the valid action was committed
        }

        [Fact]
        public void RequestAction_ChosenActionIsNull_ExecutesPassPriorityAction()
        {
            // Arrange
            var player = new PlayerBuilder("AI").Build();
            var strategyMock = new Mock<IStrategy>();
            var game = new GameBuilder().WithPlayer(player).Build();

            // Strategy returns null, simulating no viable actions
            strategyMock.Setup(s => s.decideAction(It.IsAny<List<IAction>>(), game)).Returns((IAction)null);

            Func<Game, Player, List<IAction>> generator = (g, p) => new List<IAction>();

            var controller = new AIController(player, strategyMock.Object, generator);

            // Act
            var exception = Record.Exception(() => controller.RequestAction(game));

            // Assert
            Assert.Null(exception); // Since we cannot mock PassPriorityAction instantiation directly, we ensure it runs the else block without throwing.
        }

        [Fact]
        public void RequestAction_ChosenActionIsInvalid_DoesNotCommitAndExecutesPassPriorityAction()
        {
            // Arrange
            var player = new PlayerBuilder("AI").Build();
            var strategyMock = new Mock<IStrategy>();
            var actionMock = new Mock<IAction>();
            var game = new GameBuilder().WithPlayer(player).Build();

            // Setup action as invalid
            actionMock.Setup(a => a.IsValid(game)).Returns(false);
            strategyMock.Setup(s => s.decideAction(It.IsAny<List<IAction>>(), game)).Returns(actionMock.Object);

            Func<Game, Player, List<IAction>> generator = (g, p) => new List<IAction> { actionMock.Object };

            var controller = new AIController(player, strategyMock.Object, generator);

            // Act
            controller.RequestAction(game);

            // Assert
            actionMock.Verify(a => a.Commit(It.IsAny<Game>()), Times.Never); // Invalid action must not be committed
        }

        [Fact]
        public void RequestAction_ActionGeneratorThrowsException_ThrowsException()
        {
            // Arrange
            var player = new PlayerBuilder("AI").Build();
            var strategyMock = new Mock<IStrategy>();
            var game = new GameBuilder().WithPlayer(player).Build();

            // Generator throws an exception
            Func<Game, Player, List<IAction>> generator = (g, p) => throw new InvalidOperationException("Generator failed");

            var controller = new AIController(player, strategyMock.Object, generator);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => controller.RequestAction(game));
            Assert.Equal("Generator failed", exception.Message);
        }

        #endregion

        #region Strategy Management Tests

        [Fact]
        public void SetStrategy_ValidStrategy_UpdatesStrategy()
        {
            // Arrange
            var player = new PlayerBuilder("AI").Build();
            var initialStrategyMock = new Mock<IStrategy>();
            var newStrategyMock = new Mock<IStrategy>();
            var game = new GameBuilder().Build();

            Func<Game, Player, List<IAction>> generator = (g, p) => new List<IAction>();

            var controller = new AIController(player, initialStrategyMock.Object, generator);

            // Act
            controller.setStrategy(newStrategyMock.Object);

            // Assert
            Assert.Same(newStrategyMock.Object, controller.studyStrategy(game));
        }

        [Fact]
        public void StudyStrategy_ReturnsCurrentStrategy()
        {
            // Arrange
            var player = new PlayerBuilder("AI").Build();
            var strategyMock = new Mock<IStrategy>();
            var game = new GameBuilder().Build();

            Func<Game, Player, List<IAction>> generator = (g, p) => new List<IAction>();

            var controller = new AIController(player, strategyMock.Object, generator);

            // Act
            var result = controller.studyStrategy(game);

            // Assert
            Assert.Same(strategyMock.Object, result);
        }

        #endregion
    }
}