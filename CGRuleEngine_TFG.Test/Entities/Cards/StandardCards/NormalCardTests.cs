using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using CGRuleEngine_TFG.Entities.Cards.StandardCards;
using CGRuleEngine_TFG.Entities.Cards.DataBase;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Execution.Effects;

namespace CGRuleEngine_TFG.Tests.Entities.Cards.StandardCards
{
    public class NormalCardTests
    {
        // Helper method to generate a base template to avoid code repetition
        private CardTemplate CreateDefaultTemplate()
        {
            return new CardTemplate
            {
                Name = "Generic Normal Card",
                SideA = "Front Art",
                SideB = "Back Art",
                Tags = new List<string> { "Basic" },
                Properties = new Dictionary<string, object> { { "Cost", 1 } },
                Effects = null // Set to null to bypass EffectFactory dependency for simple instantiation tests
            };
        }

        [Fact]
        public void Constructor_ValidParameters_InitializesPropertiesCorrectly()
        {
            // Arrange
            var owner = new PlayerBuilder("TestOwner").Build();
            var template = CreateDefaultTemplate();
            int instanceId = 101;

            // Act
            // We pass null for EffectFactory since template.Effects is null and won't trigger its usage
            var card = new NormalCard(template, owner, instanceId, null);

            // Assert
            Assert.Same(template, card.BaseTemplate);
            Assert.Same(owner, card.Owner);
            Assert.Equal(instanceId, card.InstanceId);
            Assert.Equal("Generic Normal Card", card.Name);
            Assert.Equal("Front Art", card.SideA);
            Assert.Equal("Back Art", card.SideB);
            Assert.Contains("Basic", card.Tags);
            Assert.Equal(1, card.Properties["Cost"]);
            Assert.True(card.IsFaceUp); // Default from CardBase
            Assert.False(card.IsTapped); // Default from CardBase
        }

        [Fact]
        public void Constructor_NullSideBAndTags_InheritsBaseHandlingAndDoesNotThrow()
        {
            // Arrange
            var owner = new PlayerBuilder("TestOwner").Build();
            var template = new CardTemplate
            {
                Name = "Minimal Card",
                SideA = "Front Art",
                SideB = null,
                Tags = null,
                Properties = null,
                Effects = null
            };

            // Act
            var card = new NormalCard(template, owner, 1, null);

            // Assert
            Assert.Equal("Minimal Card", card.Name);
            Assert.Equal("", card.SideB); // CardBase defaults null SideB to ""
            Assert.NotNull(card.Tags);
            Assert.Empty(card.Tags); // CardBase initializes empty list if null
            Assert.NotNull(card.Properties);
            Assert.Empty(card.Properties); // CardBase initializes empty dictionary if null
        }

        [Fact]
        public void Flip_InheritedMethod_ChangesFaceUpState()
        {
            // Arrange
            var owner = new PlayerBuilder("TestOwner").Build();
            var card = new NormalCard(CreateDefaultTemplate(), owner, 1, null);

            // Ensure initial state
            Assert.True(card.IsFaceUp);

            // Act
            card.Flip();

            // Assert
            Assert.False(card.IsFaceUp);
        }

        [Fact]
        public void Tap_InheritedMethod_SetsIsTappedToTrue()
        {
            // Arrange
            var owner = new PlayerBuilder("TestOwner").Build();
            var card = new NormalCard(CreateDefaultTemplate(), owner, 1, null);

            // Act
            card.Tap();

            // Assert
            Assert.True(card.IsTapped);
        }

        [Fact]
        public void Reset_InheritedMethod_RestoresTemplateState()
        {
            // Arrange
            var owner = new PlayerBuilder("TestOwner").Build();
            var template = CreateDefaultTemplate();
            var card = new NormalCard(template, owner, 1, null);

            // Modify state
            card.AddTag("TemporaryBuff");
            card.SetProperty("Cost", 5); // Change existing property
            card.Flip(); // Sets IsFaceUp to false
            card.Tap();  // Sets IsTapped to true

            // Act
            card.Reset();

            // Assert
            Assert.True(card.IsFaceUp); // Restored
            Assert.False(card.IsTapped); // Restored
            Assert.Single(card.Tags); // "TemporaryBuff" should be removed
            Assert.Contains("Basic", card.Tags);
            Assert.Equal(1, card.Properties["Cost"]); // "Cost" should be restored to 1
        }
    }
}