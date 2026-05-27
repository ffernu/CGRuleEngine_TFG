using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Players.Controllers;
using CGRuleEngine_TFG.Entities.Zones;

namespace CGRuleEngine_TFG.Tests.Entities.Players
{
    public class PlayerBuilderTests
    {
        [Fact]
        public void Constructor_WithName_SetsNameProperty()
        {
            // Arrange
            string expectedName = "Alice";

            // Act
            var builder = new PlayerBuilder(expectedName);

            // Assert
            Assert.Equal(expectedName, builder.Name);
        }

        [Fact]
        public void Constructor_Default_LeavesNameNull()
        {
            // Arrange & Act
            var builder = new PlayerBuilder();

            // Assert
            Assert.Null(builder.Name);
        }

        [Fact]
        public void SetName_ValidName_UpdatesNameAndReturnsBuilder()
        {
            // Arrange
            var builder = new PlayerBuilder();

            // Act
            var result = builder.SetName("Bob");

            // Assert
            Assert.Same(builder, result);
            Assert.Equal("Bob", builder.Name);
        }

        [Fact]
        public void SetController_ValidController_UpdatesControllerAndReturnsBuilder()
        {
            // Arrange
            var builder = new PlayerBuilder();
            var controllerMock = new Mock<IPlayerController>();

            // Act
            var result = builder.SetController(controllerMock.Object);

            // Assert
            Assert.Same(builder, result);
            Assert.Same(controllerMock.Object, builder.Controller);
        }

        [Fact]
        public void WithStat_NewStat_AddsToPropertiesDictionary()
        {
            // Arrange
            var builder = new PlayerBuilder();

            // Act
            var result = builder.WithProperty("Health", 100);

            // Assert
            Assert.Same(builder, result);
            Assert.True(builder.Properties.ContainsKey("Health"));
            Assert.Equal(100, builder.Properties["Health"]);
        }

        [Fact]
        public void WithTag_NewTag_AddsToTagsList()
        {
            // Arrange
            var builder = new PlayerBuilder();

            // Act
            var result = builder.WithTag("Aggressive");

            // Assert
            Assert.Same(builder, result);
            Assert.Contains("Aggressive", builder.Tags);
        }

        [Fact]
        public void WithZone_NewZone_AddsToZonesDictionary()
        {
            // Arrange
            var builder = new PlayerBuilder();
            var zoneMock = new Mock<IZone>();

            // Act
            var result = builder.WithZone("Hand", zoneMock.Object);

            // Assert
            Assert.Same(builder, result);
            Assert.True(builder.Zones.ContainsKey("Hand"));
            Assert.Same(zoneMock.Object, builder.Zones["Hand"]);
        }

        [Fact]
        public void Build_ConstructedBuilder_ReturnsPopulatedPlayer()
        {
            // Arrange
            var controllerMock = new Mock<IPlayerController>();
            var zoneMock = new Mock<IZone>();
            var builder = new PlayerBuilder("Charlie")
                .SetController(controllerMock.Object)
                .WithProperty("Mana", 50)
                .WithTag("Mage")
                .WithZone("Deck", zoneMock.Object);

            // Act
            var player = builder.Build();

            // Assert
            Assert.NotNull(player);
            Assert.Equal("Charlie", player.Name);
            Assert.True(player.Id > 0); // Id should be automatically assigned
            Assert.Equal(50, player.Properties["Mana"]);
            Assert.Contains("Mage", player.Tags);
            Assert.Same(zoneMock.Object, player.Zones["Deck"]);
        }
    }
}