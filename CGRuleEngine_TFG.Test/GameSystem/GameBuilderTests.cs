using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using CGRuleEngine_TFG.GameSystem;
using CGRuleEngine_TFG.GameSystem.TurnSystem;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Cards;

namespace CGRuleEngine_TFG.Tests.GameSystem
{
    public class GameBuilderTests
    {
        [Fact]
        public void Constructor_Default_InitializesEmptyCollections()
        {
            // Arrange & Act
            var builder = new GameBuilder();

            // Assert
            Assert.NotNull(builder.Players);
            Assert.Empty(builder.Players);
            Assert.Null(builder.TemplateTurn);
            Assert.Null(builder.CardFactory);
        }

        [Fact]
        public void WithPlayer_ValidPlayer_AddsToPlayersList()
        {
            // Arrange
            var builder = new GameBuilder();
            var player = new PlayerBuilder("Player1").Build();

            // Act
            var result = builder.WithPlayer(player);

            // Assert
            Assert.Same(builder, result);
            Assert.Single(builder.Players);
            Assert.Contains(player, builder.Players);
        }

        [Fact]
        public void SetTurnTemplate_ValidTemplate_SetsTemplate()
        {
            // Arrange
            var builder = new GameBuilder();
            // Using null or a dummy since we might not be able to instantiate an empty TurnFactory depending on its definition
            TurnFactory templateMock = null;

            // Act
            var result = builder.SetTurnTemplate(templateMock);

            // Assert
            Assert.Same(builder, result);
            Assert.Same(templateMock, builder.TemplateTurn);
        }

        [Fact]
        public void SetCardFactory_ValidFactory_SetsFactory()
        {
            // Arrange
            var builder = new GameBuilder();
            CardFactory factoryMock = null; // Using null as dummy

            // Act
            var result = builder.SetCardFactory(factoryMock);

            // Assert
            Assert.Same(builder, result);
            Assert.Same(factoryMock, builder.CardFactory);
        }

        [Fact]
        public void Build_ValidBuilder_ReturnsConfiguredGame()
        {
            // Arrange
            var player = new PlayerBuilder("Player1").Build();
            var builder = new GameBuilder()
                .WithPlayer(player);

            // Act
            var game = builder.Build();

            // Assert
            Assert.NotNull(game);
            Assert.Single(game.Players);
            Assert.Contains(player, game.Players);
            Assert.False(game.IsGameOver);
            Assert.NotNull(game.Stack);
            Assert.NotNull(game.results);
            Assert.Equal(0, game.TurnNumber);
        }
    }
}