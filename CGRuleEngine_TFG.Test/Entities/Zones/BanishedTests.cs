using System;
using System.Collections;
using System.Collections.Generic;

using CGRuleEngine_TFG.Entities;
using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Zones;
using Moq;
using Xunit;

namespace CGRuleEngine_TFG.Entities.Zones.UnitTests
{
    /// <summary>
    /// Unit tests for CGRuleEngine_TFG.Entities.Zones.Banished focusing on RemoveCard behavior.
    /// </summary>
    public class BanishedTests
    {
/// <summary>
    /// Verifies that RemoveCard throws InvalidOperationException when the card's controller is NOT listed as an owner.
    /// Input conditions: Banished with no owners; a Card whose Controller is a Player instance (mocked) not present in Owners; card is present in Content.
    /// Expected result: InvalidOperationException is thrown with the message "The player is not an owner of the banished." and the card remains in Content.
    /// </summary>
    [Fact]
    public void RemoveCard_WhenControllerIsNotOwner_ThrowsInvalidOperationException()
    {
        // Arrange
        var banished = new Banished();
        var controller = new CGRuleEngine_TFG.Entities.Players.PlayerBuilder("TestPlayer").Build();

        var card = new Card
        {
            Controller = controller,
            Name = "TestCard"
        };

        // Add card to content to ensure RemoveCard would remove it if allowed
        banished.Content.Add(card);

        // Act
        var ex = Assert.Throws<InvalidOperationException>(() => banished.RemoveCard(card));

        // Assert
        Assert.Equal("The player is not an owner of the banished.", ex.Message);
        // Ensure the card was not removed due to the exception
        Assert.Contains(card, banished.Content);
    }

        /// <summary>
        /// Verifies that when a card with the requested name exists in Content the first matching instance is returned.
        /// Input conditions: a Banished.Content populated with cards (including duplicate names and special-string names).
        /// Expected result: Search returns the exact Card instance at the expected index (first match).
        /// </summary>
        [Theory]
        [MemberData(nameof(ExistingNameCases))]
        public void Search_NameExists_ReturnsFirstMatch(List<string> cardNames, string searchName, int expectedIndex)
        {
            // Arrange
            var banished = new Banished();
            for (int i = 0; i < cardNames.Count; i++)
            {
                var c = new Card { Name = cardNames[i] };
                banished.Content.Add(c);
            }

            // Act
            var result = banished.Search(searchName);

            // Assert
            Assert.Same(banished.Content[expectedIndex], result);
        }

        /// <summary>
        /// Verifies that Search returns null when no card with the requested name exists.
        /// Input conditions: various Content lists that do not include the searched name or differ by case.
        /// Expected result: Search returns null.
        /// </summary>
        [Theory]
        [MemberData(nameof(NonExistingNameCases))]
        public void Search_NameDoesNotExist_ReturnsNull(List<string> cardNames, string searchName)
        {
            // Arrange
            var banished = new Banished();
            foreach (var n in cardNames)
            {
                banished.Content.Add(new Card { Name = n });
            }

            // Act
            var result = banished.Search(searchName);

            // Assert
            Assert.Null(result);
        }

        // MemberData for existing-name scenarios (covers duplicates, whitespace, very long strings, special chars, newline)
        public static IEnumerable<object[]> ExistingNameCases
        {
            get
            {
                // Simple single match
                yield return new object[] { new List<string> { "Alpha" }, "Alpha", 0 };

                // Duplicates: first occurrence must be returned
                yield return new object[] { new List<string> { "Alpha", "Beta", "Alpha" }, "Alpha", 0 };

                // Whitespace-only name
                yield return new object[] { new List<string> { "   " }, "   ", 0 };

                // Very long string
                string longName = new string('x', 1000);
                yield return new object[] { new List<string> { longName }, longName, 0 };

                // Special/control characters (including newline)
                yield return new object[] { new List<string> { "Line\nFeed" }, "Line\nFeed", 0 };
                yield return new object[] { new List<string> { "Special@!#%^&*()" }, "Special@!#%^&*()", 0 };
            }
        }

        // MemberData for non-existing-name scenarios (case sensitivity, empty string search, and different names)
        public static IEnumerable<object[]> NonExistingNameCases
        {
            get
            {
                // Case sensitivity: "alpha" vs "Alpha"
                yield return new object[] { new List<string> { "alpha" }, "Alpha" };

                // Different single element
                yield return new object[] { new List<string> { "Alpha" }, "Beta" };

                // Empty string search when no empty-named card exists
                yield return new object[] { new List<string> { "Alpha", "Beta" }, string.Empty };
            }
        }

        //
        // Helper: minimal PlayerBuilder wrapper for tests to construct Player.
        // This helper is nested inside the test class per generation rules and avoids adding test-level types outside the class.
        //
        private class PlayerBuilder
        {
            public string Name { get; }
            public Dictionary<string, int> Stats { get; } = new Dictionary<string, int>();
            public Dictionary<string, IZone> Zones { get; } = new Dictionary<string, IZone>();
            public object? Controller { get; } = null;

            public PlayerBuilder(string name) => Name = name;
        }

        /// <summary>
        /// Verifies that GetFaceDown throws an InvalidOperationException when the banished zone has no content.
        /// Input conditions: Banished.Content is empty.
        /// Expected result: InvalidOperationException with message "The banished zone is empty." is thrown.
        /// </summary>
        [Fact]
        public void GetFaceDown_EmptyContent_ThrowsInvalidOperationException()
        {
            // Arrange
            var banished = new Banished(); // Content is empty by default

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => banished.GetFaceDown());
            Assert.Equal("The banished zone is empty.", ex.Message);
        }

        /// <summary>
        /// Verifies that GetFaceDown returns exactly the cards that are face-down from the banished zone.
        /// Input conditions: various Content lists containing only face-up, only face-down and mixed cards.
        /// Expected result: Returned list contains only cards with isFaceUp == false and its Count equals expectedCount.
        /// Also verifies returned items are the same instances present in Content.
        /// </summary>
        [Theory]
        [MemberData(nameof(GetFaceDownTestData))]
        public void GetFaceDown_VariousContent_ReturnsOnlyFaceDownCards(List<Card> initialContent, int expectedCount)
        {
            // Arrange
            var banished = new Banished();
            // Populate Content with provided test data (preserve references)
            foreach (var c in initialContent)
            {
                banished.Content.Add(c);
            }

            // Act
            var result = banished.GetFaceDown();

            // Assert
            Assert.Equal(expectedCount, result.Count);
            // Every returned card must be face-down
            Assert.All(result, c => Assert.False(c.isFaceUp));
            // Ensure each returned card comes from the original Content collection (reference equality)
            foreach (var card in result)
            {
                Assert.Contains(card, banished.Content);
            }
        }

        public static IEnumerable<object[]> GetFaceDownTestData()
        {
            // Case 1: All face-up -> expect 0 results
            var allFaceUp = new List<Card>
            {
                new Card { Name = "Up1", isFaceUp = true },
                new Card { Name = "Up2", isFaceUp = true },
                new Card { Name = "Up3", isFaceUp = true }
            };

            // Case 2: All face-down -> expect all returned
            var allFaceDown = new List<Card>
            {
                new Card { Name = "Down1", isFaceUp = false },
                new Card { Name = "Down2", isFaceUp = false },
                new Card { Name = "Down3", isFaceUp = false }
            };

            // Case 3: Mixed -> expect only face-down returned
            var mixed = new List<Card>
            {
                new Card { Name = "DownA", isFaceUp = false },
                new Card { Name = "UpA", isFaceUp = true },
                new Card { Name = "DownB", isFaceUp = false }
            };

            yield return new object[] { allFaceUp, 0 };
            yield return new object[] { allFaceDown, 3 };
            yield return new object[] { mixed, 2 };
        }

        /// <summary>
        /// Verifies that GetFaceUp throws InvalidOperationException when the banished zone content is empty.
        /// Input conditions: a new Banished instance with no cards in Content.
        /// Expected result: InvalidOperationException with message "The banished zone is empty." is thrown.
        /// </summary>
        [Fact]
        public void GetFaceUp_EmptyContent_ThrowsInvalidOperationException()
        {
            // Arrange
            var zone = new Banished();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => zone.GetFaceUp());
            Assert.Equal("The banished zone is empty.", ex.Message);
        }

        /// <summary>
        /// Verifies that GetFaceUp returns an empty list (not null) when there are cards but none are face-up.
        /// Input conditions: banished zone with one or more cards where isFaceUp == false.
        /// Expected result: method returns a non-null list with Count == 0 and does not throw.
        /// </summary>
        [Theory]
        [MemberData(nameof(NoFaceUpCases))]
        public void GetFaceUp_NoFaceUpCards_ReturnsEmptyList(List<Card> content)
        {
            // Arrange
            var zone = new Banished();
            zone.Content.AddRange(content);

            // Act
            var result = zone.GetFaceUp();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Count);
            // Ensure original content remains unchanged
            Assert.Equal(content.Count, zone.Content.Count);
        }

        /// <summary>
        /// Verifies that GetFaceUp returns only the cards that are face-up and that the returned list is independent from Content.
        /// Input conditions: banished zone with a mix of face-up and face-down cards.
        /// Expected result: returned list contains exactly the face-up cards, same instances as in Content, and modifying the returned list does not affect Content.
        /// </summary>
        [Theory]
        [MemberData(nameof(MixedFaceUpCases))]
        public void GetFaceUp_MixedContent_ReturnsOnlyFaceUpCardsAndIndependentList(List<Card> content, Card[] expectedFaceUp)
        {
            // Arrange
            var zone = new Banished();
            zone.Content.AddRange(content);

            // Act
            var result = zone.GetFaceUp();

            // Assert - returned collection correctness
            Assert.NotNull(result);
            Assert.Equal(expectedFaceUp.Length, result.Count);
            foreach (var expected in expectedFaceUp)
            {
                Assert.Contains(expected, result);
            }

            // Assert - returned list is a different instance than Content
            Assert.NotSame(zone.Content, result);

            // Assert - modifications to returned list do not modify original Content
            if (expectedFaceUp.Length > 0)
            {
                var first = expectedFaceUp[0];
                result.Remove(first); // remove from result
                Assert.Contains(first, zone.Content); // must still be present in the zone's content
            }
        }

        // Test data: lists with no face-up cards
        public static IEnumerable<object[]> NoFaceUpCases()
        {
            yield return new object[] { new List<Card> { new Card { Name = "C1", isFaceUp = false } } };
            yield return new object[] { new List<Card> {
                new Card { Name = "C1", isFaceUp = false },
                new Card { Name = "C2", isFaceUp = false },
                new Card { Name = "C3", isFaceUp = false }
            } };
        }

        // Test data: mixed face-up and face-down cards, specifying expected face-up instances
        public static IEnumerable<object[]> MixedFaceUpCases()
        {
            var a = new Card { Name = "A", isFaceUp = true };
            var b = new Card { Name = "B", isFaceUp = false };
            var c = new Card { Name = "C", isFaceUp = true };

            yield return new object[] { new List<Card> { a, b, c }, new Card[] { a, c } };

            // Single face-up among face-downs
            var d = new Card { Name = "D", isFaceUp = false };
            var e = new Card { Name = "E", isFaceUp = true };
            yield return new object[] { new List<Card> { d, e }, new Card[] { e } };

            // All face-up
            var f = new Card { Name = "F", isFaceUp = true };
            var g = new Card { Name = "G", isFaceUp = true };
            yield return new object[] { new List<Card> { f, g }, new Card[] { f, g } };
        }

        /// <summary>
        /// Verifies that Clone returns a new Banished instance that is distinct from the original.
        /// Input conditions: an instantiated Banished with default state (empty Owners and Content).
        /// Expected result: returned value is an IZone of concrete type Banished, not the same reference as the original,
        /// and both Owners and Content are non-null and empty.
        /// </summary>
        [Fact]
        public void Clone_DefaultInstance_ReturnsDistinctEmptyBanished()
        {
            // Arrange
            var original = new Banished();

            // Act
            IZone? cloned = original.Clone();

            // Assert
            Assert.NotNull(cloned);
            // Concrete type is Banished
            Assert.IsType<Banished>(cloned);
            // Different object instance
            Assert.NotSame(original, cloned);

            // Owners and Content must be initialized and empty on the clone
            Assert.NotNull(cloned.Owners);
            Assert.Empty(cloned.Owners);

            Assert.NotNull(cloned.Content);
            Assert.Empty(cloned.Content);

            // Ensure clone's collections are distinct instances from original's collections
            Assert.NotSame(original.Owners, cloned.Owners);
            Assert.NotSame(original.Content, cloned.Content);
        }

        /// <summary>
        /// Verifies that modifying the clone's collections does not affect the original instance.
        /// Input conditions: original Banished is empty; clone is created and modified by adding a Card.
        /// Expected result: clone.Content reflects the addition while original.Content remains empty.
        /// </summary>
        [Fact]
        public void Clone_ModifyCloneCollections_DoesNotAffectOriginal()
        {
            // Arrange
            var original = new Banished();
            IZone cloned = original.Clone();

            var card = new Card
            {
                Name = "TestCard",
                Owner = null,
                Controller = null,
                isFaceUp = true,
                isTapped = false
            };

            // Pre-assert: original must be empty
            Assert.Empty(original.Content);
            Assert.Empty(cloned.Content);

            // Act
            // Add to clone's content (since Clone returns a Banished with its own Content list)
            cloned.Content.Add(card);

            // Assert
            Assert.Single(cloned.Content);
            Assert.Equal("TestCard", cloned.Content[0].Name);

            // Original remains unaffected
            Assert.Empty(original.Content);
        }

        /// <summary>
        /// Verifies that the parameterless constructor Banished() initializes Owners and Content
        /// as non-null, empty lists of the appropriate element types.
        /// Input conditions: default construction with no parameters.
        /// Expected result: Owners and Content are non-null List&lt;Player&gt; and List&lt;Card&gt; respectively and have Count == 0.
        /// </summary>
        [Fact]
        public void Banished_Ctor_InitializesOwnersAndContentToEmptyLists()
        {
            // Arrange
            // (no setup required)

            // Act
            var zone = new Banished();

            // Assert
            Assert.NotNull(zone.Owners);
            Assert.IsType<List<Player>>(zone.Owners);
            Assert.Equal(0, zone.Owners.Count);

            Assert.NotNull(zone.Content);
            Assert.IsType<List<Card>>(zone.Content);
            Assert.Equal(0, zone.Content.Count);
        }

        /// <summary>
        /// Verifies that the Owners and Content properties are settable after construction and accept empty lists.
        /// Input conditions: create empty List&lt;Player&gt; and List&lt;Card&gt; and assign to the properties.
        /// Expected result: the properties reference the assigned list instances and remain empty (Count == 0).
        /// </summary>
        [Fact]
        public void Banished_Ctor_AllowsSettingOwnersAndContent_ToEmptyLists()
        {
            // Arrange
            var zone = new Banished();
            var newOwners = new List<Player>();
            var newContent = new List<Card>();

            // Act
            zone.Owners = newOwners;
            zone.Content = newContent;

            // Assert
            Assert.Same(newOwners, zone.Owners);
            Assert.Empty(zone.Owners);

            Assert.Same(newContent, zone.Content);
            Assert.Empty(zone.Content);
        }
    }
}