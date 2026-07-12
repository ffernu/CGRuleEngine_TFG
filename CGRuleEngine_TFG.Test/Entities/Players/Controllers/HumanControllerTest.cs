using CGRuleEngine_TFG.Entities.Players.Controllers;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Execution;
using CGRuleEngine_TFG.Execution.Actions;
using CGRuleEngine_TFG.GameSystem;
using Moq;
using System;
using Xunit;

namespace CGRuleEngine_TFG.Tests.Entities.Players.Controllers
{
    public class HumanControllerTests
    {
        [Fact]
        public void Constructor_Initialization_SetsDefaultValuesCorrectly()
        {
            // Arrange & Act
            var controller = new HumanController();

            // Assert
            Assert.False(controller.IsWaitingInput);
            Assert.Null(controller.CurrentAction);
        }

        [Fact]
        public void RequestAction_ValidGameState_SetsIsWaitingInputToTrue()
        {
            // Arrange
            var controller = new HumanController();
            var game = new GameBuilder().Build();

            // Act
            controller.RequestAction(game);

            // Assert
            Assert.True(controller.IsWaitingInput);
        }

        [Fact]
        public void RequestAction_NullGameState_SetsIsWaitingInputToTrue()
        {
            // Arrange
            var controller = new HumanController();

            // Act
            controller.RequestAction(null);

            // Assert
            Assert.True(controller.IsWaitingInput);
        }

        [Fact]
        public void UIInputReceived_NotWaitingInput_ReturnsEarlyAndDoesNothing()
        {
            // Arrange
            var controller = new HumanController();
            var actionMock = new Mock<IAction>();

            // Act
            controller.UIInputReceived(actionMock.Object);

            // Assert
            Assert.False(controller.IsWaitingInput);
            actionMock.Verify(a => a.IsValid(It.IsAny<Game>()), Times.Never);
            actionMock.Verify(a => a.Commit(It.IsAny<Game>()), Times.Never);
        }

        [Fact]
        public void UIInputReceived_WaitingInputButActionIsNull_ThrowsNullReferenceException()
        {
            // Arrange
            var controller = new HumanController();
            var game = new GameBuilder().Build();
            controller.RequestAction(game);

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => controller.UIInputReceived(null));
        }

        /*[Fact]
        public void UIInputReceived_WaitingInputAndActionIsInvalid_DoesNotCommitButNotifiesPriorityPlayer()
        {
            // Arrange
            var controller = new HumanController();

            // We set up a priority player with a mocked controller to verify game.NotifyPriorityPlayer() is called
            var mockPlayerController = new Mock<IPlayerController>();
            var priorityPlayer = new PlayerBuilder("PriorityPlayer").Build();
            priorityPlayer.Controller = mockPlayerController.Object;

            var game = new GameBuilder().WithPlayer(priorityPlayer).Build();
            game.PriorityPlayer = priorityPlayer;

            controller.RequestAction(game);

            var actionMock = new Mock<IAction>();
            actionMock.Setup(a => a.IsValid(game)).Returns(false);

            // Act
            controller.UIInputReceived(actionMock.Object);

            // Assert
            Assert.True(controller.IsWaitingInput); // State should not change to false
            actionMock.Verify(a => a.Commit(It.IsAny<Game>()), Times.Never);
            mockPlayerController.Verify(c => c.RequestAction(game), Times.Once); // Verifies _currentGameRef.NotifyPriorityPlayer()
        }*/

        /*[Fact]
        public void UIInputReceived_WaitingInputAndActionIsValid_CommitsActionResetsStateAndNotifiesPriorityPlayer()
        {
            // Arrange
            var controller = new HumanController();

            // We set up a priority player with a mocked controller to verify game.NotifyPriorityPlayer() is called
            var mockPlayerController = new Mock<IPlayerController>();
            var priorityPlayer = new PlayerBuilder("PriorityPlayer").Build();
            priorityPlayer.Controller = mockPlayerController.Object;

            var game = new GameBuilder().WithPlayer(priorityPlayer).Build();
            game.PriorityPlayer = priorityPlayer;

            controller.RequestAction(game);

            var actionMock = new Mock<IAction>();
            actionMock.Setup(a => a.IsValid(game)).Returns(true);

            // Act
            controller.UIInputReceived(actionMock.Object);

            // Assert
            Assert.False(controller.IsWaitingInput); // State should reset
            Assert.Null(controller.CurrentAction); // Action is reset to null after commit
            actionMock.Verify(a => a.Commit(game), Times.Once); // Action was successfully committed
            mockPlayerController.Verify(c => c.RequestAction(game), Times.Once); // Verifies _currentGameRef.NotifyPriorityPlayer()
        }*/
    }
}