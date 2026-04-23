using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Cards.DataBase;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Zones;
using CGRuleEngine_TFG.Execution.Effects;

namespace CGRuleEngine_TFG.Tests.Entities.Cards
{
    // Concrete implementation of CardBase to facilitate testing of abstract class methods
    public class TestCard : CardBase
    {
        public TestCard(CardTemplate template, Player owner, int instanceId, EffectFactory effectFactory)
            : base(template, owner, instanceId, effectFactory)
        {
        }
    }

    public class CardBaseTests
    {
        // Helper method to create a basic TestCard instance
        // Uses PlayerBuilder to avoid mocking issues with the concrete Player class
        private TestCard CreateDefaultTestCard(CardTemplate template = null)
        {
            var owner = new PlayerBuilder("Test Owner").Build();

            if (template == null)
            {
                template = new CardTemplate
                {
                    Name = "Test Card",
                    SideA = "Front Art",
                    SideB = "Back Art",
                    Tags = new List<string>(),
                    Properties = new Dictionary<string, object>(),
                    Effects = null // Set to null to bypass EffectFactory dependency for basic tests
                };
            }

            // Passing null for EffectFactory as Effects is null in the default template
            return new TestCard(template, owner, 100, null);
        }

        [Fact]
        public void Constructor_ValidTemplate_InitializesPropertiesCorrectly()
        {
            // Arrange
            var owner = new PlayerBuilder("Owner").Build();
            var template = new CardTemplate
            {
                Name = "Goblin",
                SideA = "Goblin_Front",
                SideB = "Goblin_Back",
                Tags = new List<string> { "Monster" },
                Properties = new Dictionary<string, object> { { "Attack", 5 } },
                Effects = null
            };

            // Act
            var card = new TestCard(template, owner, 42, null);

            // Assert
            Assert.Same(template, card.BaseTemplate);
            Assert.Same(owner, card.Owner);
            Assert.Equal(42, card.InstanceId);
            Assert.Equal("Goblin", card.Name);
            Assert.Equal("Goblin_Front", card.SideA);
            Assert.Equal("Goblin_Back", card.SideB);
            Assert.Contains("Monster", card.Tags);
            Assert.Equal(5, card.Properties["Attack"]);
            Assert.True(card.IsFaceUp);
            Assert.False(card.IsTapped);
            Assert.Empty(card.Effects);
        }

        [Fact]
        public void Constructor_NullSideBAndTagsAndProperties_HandlesNullsGracefully()
        {
            // Arrange
            var owner = new PlayerBuilder("Owner").Build();
            var template = new CardTemplate
            {
                Name = "Spell",
                SideA = "Spell_Art",
                SideB = null, // Edge case: null SideB
                Tags = null,  // Edge case: null Tags
                Properties = null, // Edge case: null Properties
                Effects = null
            };

            // Act
            var card = new TestCard(template, owner, 1, null);

            // Assert
            Assert.Equal("", card.SideB);
            Assert.NotNull(card.Tags);
            Assert.Empty(card.Tags);
            Assert.NotNull(card.Properties);
            Assert.Empty(card.Properties);
        }

        [Fact]
        public void TryGetProperty_PropertyExistsWithCorrectType_ReturnsTrueAndValue()
        {
            // Arrange
            var template = new CardTemplate
            {
                Properties = new Dictionary<string, object> { { "Health", 100 } }
            };
            var card = CreateDefaultTestCard(template);

            // Act
            bool success = card.TryGetProperty<int>("Health", out int value);

            // Assert
            Assert.True(success);
            Assert.Equal(100, value);
        }

        [Fact]
        public void TryGetProperty_PropertyExistsWithIncorrectType_ReturnsFalseAndDefault()
        {
            // Arrange
            var template = new CardTemplate
            {
                Properties = new Dictionary<string, object> { { "Health", 100 } } // Stored as int
            };
            var card = CreateDefaultTestCard(template);

            // Act
            bool success = card.TryGetProperty<string>("Health", out string value); // Requested as string

            // Assert
            Assert.False(success);
            Assert.Null(value);
        }

        [Fact]
        public void TryGetProperty_PropertyDoesNotExist_ReturnsFalseAndDefault()
        {
            // Arrange
            var card = CreateDefaultTestCard();

            // Act
            bool success = card.TryGetProperty<int>("Mana", out int value);

            // Assert
            Assert.False(success);
            Assert.Equal(0, value);
        }

        [Fact]
        public void SetProperty_NewProperty_AddsToDictionary()
        {
            // Arrange
            var card = CreateDefaultTestCard();

            // Act
            card.SetProperty("Defense", 50);

            // Assert
            Assert.True(card.Properties.ContainsKey("Defense"));
            Assert.Equal(50, card.Properties["Defense"]);
        }

        [Fact]
        public void SetProperty_ExistingProperty_UpdatesValue()
        {
            // Arrange
            var template = new CardTemplate
            {
                Properties = new Dictionary<string, object> { { "Defense", 10 } }
            };
            var card = CreateDefaultTestCard(template);

            // Act
            card.SetProperty("Defense", 20);

            // Assert
            Assert.Equal(20, card.Properties["Defense"]);
        }

        [Fact]
        public void HasTag_TagExists_ReturnsTrue()
        {
            // Arrange
            var template = new CardTemplate
            {
                Tags = new List<string> { "Flying" }
            };
            var card = CreateDefaultTestCard(template);

            // Act
            bool result = card.HasTag("Flying");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasTag_TagDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var card = CreateDefaultTestCard();

            // Act
            bool result = card.HasTag("Undead");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void AddTag_NewTag_AddsToList()
        {
            // Arrange
            var card = CreateDefaultTestCard();

            // Act
            card.AddTag("Poisonous");

            // Assert
            Assert.Contains("Poisonous", card.Tags);
        }

        [Fact]
        public void AddTag_ExistingTag_DoesNotDuplicate()
        {
            // Arrange
            var template = new CardTemplate
            {
                Tags = new List<string> { "Fast" }
            };
            var card = CreateDefaultTestCard(template);

            // Act
            card.AddTag("Fast");

            // Assert
            Assert.Single(card.Tags); // Count should remain 1
        }

        [Fact]
        public void RemoveTag_ExistingTag_RemovesFromList()
        {
            // Arrange
            var template = new CardTemplate
            {
                Tags = new List<string> { "Fast", "Fragile" }
            };
            var card = CreateDefaultTestCard(template);

            // Act
            card.RemoveTag("Fast");

            // Assert
            Assert.Single(card.Tags);
            Assert.DoesNotContain("Fast", card.Tags);
            Assert.Contains("Fragile", card.Tags);
        }

        [Fact]
        public void RemoveTag_NonExistingTag_DoesNothing()
        {
            // Arrange
            var template = new CardTemplate
            {
                Tags = new List<string> { "Fragile" }
            };
            var card = CreateDefaultTestCard(template);

            // Act
            card.RemoveTag("Fast"); // Tag does not exist

            // Assert
            Assert.Single(card.Tags);
            Assert.Contains("Fragile", card.Tags);
        }

        [Fact]
        public void Reset_Always_RestoresTagsPropertiesAndStateFromTemplate()
        {
            // Arrange
            var template = new CardTemplate
            {
                Tags = new List<string> { "OriginalTag" },
                Properties = new Dictionary<string, object> { { "BasePower", 10 } }
            };
            var card = CreateDefaultTestCard(template);

            // Modify state to deviate from template
            card.AddTag("TemporaryBuff");
            card.SetProperty("BasePower", 15);
            card.SetProperty("TemporaryStat", 5);
            card.IsFaceUp = false;
            card.IsTapped = true;

            // Act
            card.Reset();

            // Assert
            Assert.True(card.IsFaceUp); // Restored to true
            Assert.False(card.IsTapped); // Restored to false

            // Verify Tags restored
            Assert.Single(card.Tags);
            Assert.Contains("OriginalTag", card.Tags);
            Assert.DoesNotContain("TemporaryBuff", card.Tags);

            // Verify Properties restored
            Assert.Single(card.Properties);
            Assert.Equal(10, card.Properties["BasePower"]);
            Assert.False(card.Properties.ContainsKey("TemporaryStat"));

            // Verify Effects list is empty (as per current TODO implementation)
            Assert.Empty(card.Effects);
        }

        [Fact]
        public void Flip_IsFaceUpTrue_SetsToFalse()
        {
            // Arrange
            var card = CreateDefaultTestCard();
            card.IsFaceUp = true;

            // Act
            card.Flip();

            // Assert
            Assert.False(card.IsFaceUp);
        }

        [Fact]
        public void Flip_IsFaceUpFalse_SetsToTrue()
        {
            // Arrange
            var card = CreateDefaultTestCard();
            card.IsFaceUp = false;

            // Act
            card.Flip();

            // Assert
            Assert.True(card.IsFaceUp);
        }

        [Fact]
        public void Tap_Always_SetsIsTappedToTrue()
        {
            // Arrange
            var card = CreateDefaultTestCard();
            card.IsTapped = false;

            // Act
            card.Tap();

            // Assert
            Assert.True(card.IsTapped);
        }

        [Fact]
        public void Untap_Always_SetsIsTappedToFalse()
        {
            // Arrange
            var card = CreateDefaultTestCard();
            card.IsTapped = true;

            // Act
            card.Untap();

            // Assert
            Assert.False(card.IsTapped);
        }
    }
}