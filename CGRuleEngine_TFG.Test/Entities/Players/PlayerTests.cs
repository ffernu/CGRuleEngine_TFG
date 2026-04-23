using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Zones;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Test.Entities.Players
{
    public class PlayerTests
    {
        [Fact]
        public void Constructor_ValidBuilder_MapsPropertiesAndIncrementsId()
        {
            // Arrange
            var builder1 = new PlayerBuilder("Player1");
            var builder2 = new PlayerBuilder("Player2");

            // Act
            var player1 = new Player(builder1);
            var player2 = new Player(builder2);

            // Assert
            Assert.Equal("Player1", player1.Name);
            Assert.Equal("Player2", player2.Name);
            Assert.True(player2.Id > player1.Id); // Verifies static NextId incrementation
        }

        [Fact]
        public void GetZone_ZoneExists_ReturnsZone()
        {
            // Arrange
            var zoneMock = new Mock<IZone>();
            var player = new PlayerBuilder().WithZone("Graveyard", zoneMock.Object).Build();

            // Act
            var result = player.getZone("Graveyard");

            // Assert
            Assert.Same(zoneMock.Object, result);
        }

        [Fact]
        public void GetZone_ZoneDoesNotExist_ThrowsInvalidOperationException()
        {
            // Arrange
            var player = new PlayerBuilder().Build();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => player.getZone("NonExistentZone"));
            Assert.Equal("The player does not have a zone with that name", exception.Message);
        }

        [Fact]
        public void TryGetProperty_PropertyExistsAndCorrectType_ReturnsTrueAndValue()
        {
            // Arrange
            var player = new PlayerBuilder().WithStat("Health", 100).Build();

            // Act
            bool success = player.TryGetProperty<int>("Health", out int value);

            // Assert
            Assert.True(success);
            Assert.Equal(100, value);
        }

        [Fact]
        public void TryGetProperty_PropertyExistsButWrongType_ReturnsFalseAndDefault()
        {
            // Arrange
            var player = new PlayerBuilder().WithStat("Health", 100).Build();

            // Act
            bool success = player.TryGetProperty<string>("Health", out string value);

            // Assert
            Assert.False(success);
            Assert.Null(value); // Default for string is null
        }

        [Fact]
        public void TryGetProperty_PropertyDoesNotExist_ReturnsFalseAndDefault()
        {
            // Arrange
            var player = new PlayerBuilder().Build();

            // Act
            bool success = player.TryGetProperty<int>("Stamina", out int value);

            // Assert
            Assert.False(success);
            Assert.Equal(0, value); // Default for int is 0
        }

        [Fact]
        public void SetProperty_NewProperty_AddsPropertyToDictionary()
        {
            // Arrange
            var player = new PlayerBuilder().Build();

            // Act
            player.SetProperty("Gold", 500);

            // Assert
            Assert.True(player.Properties.ContainsKey("Gold"));
            Assert.Equal(500, player.Properties["Gold"]);
        }

        [Fact]
        public void SetProperty_ExistingProperty_UpdatesPropertyValue()
        {
            // Arrange
            var player = new PlayerBuilder().WithStat("Gold", 100).Build();

            // Act
            player.SetProperty("Gold", 300);

            // Assert
            Assert.Equal(300, player.Properties["Gold"]);
        }

        [Fact]
        public void HasTag_TagExists_ReturnsTrue()
        {
            // Arrange
            var player = new PlayerBuilder().WithTag("Poisoned").Build();

            // Act
            bool result = player.HasTag("Poisoned");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasTag_TagDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var player = new PlayerBuilder().Build();

            // Act
            bool result = player.HasTag("Flying");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void AddTag_NewTag_AddsTagToList()
        {
            // Arrange
            var player = new PlayerBuilder().Build();

            // Act
            player.AddTag("Stunned");

            // Assert
            Assert.Contains("Stunned", player.Tags);
        }

        [Fact]
        public void AddTag_ExistingTag_DoesNotDuplicateTag()
        {
            // Arrange
            var player = new PlayerBuilder().WithTag("Stunned").Build();

            // Act
            player.AddTag("Stunned");

            // Assert
            Assert.Single(player.Tags); // List should still only contain exactly one item
        }

        [Fact]
        public void RemoveTag_ExistingTag_RemovesTagFromList()
        {
            // Arrange
            var player = new PlayerBuilder().WithTag("Stunned").Build();

            // Act
            player.RemoveTag("Stunned");

            // Assert
            Assert.Empty(player.Tags);
        }

        [Fact]
        public void RemoveTag_NonExistingTag_DoesNothingAndDoesNotThrow()
        {
            // Arrange
            var player = new PlayerBuilder().WithTag("Stunned").Build();

            // Act
            var exception = Record.Exception(() => player.RemoveTag("Poisoned"));

            // Assert
            Assert.Null(exception);
            Assert.Single(player.Tags);
            Assert.Contains("Stunned", player.Tags);
        }
    }
}
