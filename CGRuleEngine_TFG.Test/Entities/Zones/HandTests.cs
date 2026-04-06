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
    public class HandTests
    {
        /// <summary>
        /// Verifies that Clone creates a new Hand instance with the same MaxSize but independent collections.
        /// Input conditions: various MaxSize integer values (including extremes: int.MinValue and int.MaxValue, negatives, zero, positive).
        /// Expected result: returned IZone is a Hand, not the same reference as the source; MaxSize is copied; Content and Owners are empty and are different collection instances than the original.
        /// </summary>
        [Theory]
        [MemberData(nameof(MaxSizeValues))]
        public void Clone_WithVariousMaxSize_ReturnsDistinctHandWithSameMaxSizeAndEmptyCollections(int maxSize)
        {
            // Arrange
            Hand original = new Hand(maxSize);

            // Act
            var clonedZone = original.Clone();

            // Assert - type and non-null
            Assert.NotNull(clonedZone);
            Hand clone = Assert.IsType<Hand>(clonedZone);

            // Assert - MaxSize copied by value
            Assert.Equal(original.MaxSize, clone.MaxSize);
            Assert.Equal(maxSize, clone.MaxSize);

            // Assert - distinct instances
            Assert.NotSame(original, clone);

            // Assert - collections are initialized empty and are independent instances
            Assert.NotNull(original.Content);
            Assert.NotNull(clone.Content);
            Assert.NotSame(original.Content, clone.Content);
            Assert.Empty(clone.Content);

            Assert.NotNull(original.Owners);
            Assert.NotNull(clone.Owners);
            Assert.NotSame(original.Owners, clone.Owners);
            Assert.Empty(clone.Owners);
        }

        /// <summary>
        /// Provides a set of MaxSize values to validate Clone behavior across numeric edge cases.
        /// Contains: int.MinValue, -1, 0, 1, int.MaxValue.
        /// </summary>
        public static IEnumerable<object[]> MaxSizeValues()
        {
            yield return new object[] { int.MinValue };
            yield return new object[] { -1 };
            yield return new object[] { 0 };
            yield return new object[] { 1 };
            yield return new object[] { int.MaxValue };
        }

        /// <summary>
        /// Verifies that AddCard successfully adds the provided card when the card's Controller is listed as an owner
        /// and the hand has available capacity.
        /// Input conditions: Hand.MaxSize is one of the provided values (1 and int.MaxValue), Owners contains the card.Controller, Content is empty.
        /// Expected result: The card is added to Content and Content.Count equals 1.
        /// </summary>
        [Theory]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        public void AddCard_OwnerIsControllerAndSpace_AddsCardToContent(int maxSize)
        {
            // Arrange
            var hand = new Hand(maxSize);
            var controller = new Players.Player(new Players.PlayerBuilder("Controller")) { }; // use PlayerBuilder to create Player (exists in project)
            var card = new Card { Name = "TestCard", Controller = controller };

            hand.Owners.Add(controller);

            // Act
            hand.AddCard(card);

            // Assert
            Assert.Contains(card, hand.Content);
            Assert.Equal(1, hand.Content.Count);
        }

        /// <summary>
        /// Verifies that AddCard throws InvalidOperationException when the card.Controller is not an owner of the hand.
        /// Input conditions: Hand.MaxSize is 1, Owners does NOT contain the card.Controller.
        /// Expected result: InvalidOperationException is thrown with message "The player is not an owner of the hand." and Content remains empty.
        /// </summary>
        [Fact]
        public void AddCard_ControllerNotOwner_ThrowsInvalidOperationException()
        {
            // Arrange
            var hand = new Hand(1);
            var controller = new Players.Player(new Players.PlayerBuilder("NonOwner")) { };
            var card = new Card { Name = "OrphanCard", Controller = controller };

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => hand.AddCard(card));

            // Assert
            Assert.Equal("The player is not an owner of the hand.", ex.Message);
            Assert.Empty(hand.Content);
        }

        /// <summary>
        /// Verifies that AddCard throws InvalidOperationException when the hand has already reached its capacity.
        /// Input conditions: Owners contains card.Controller; MaxSize values tested: 0 and int.MinValue (negative limits).
        /// Expected result: InvalidOperationException is thrown with message "The hand is full." and Content remains empty.
        /// </summary>
        [Theory]
        [InlineData(0)]
        [InlineData(int.MinValue)]
        public void AddCard_OwnerIsControllerButHandFull_ThrowsInvalidOperationException(int maxSize)
        {
            // Arrange
            var hand = new Hand(maxSize);
            var controller = new Players.Player(new Players.PlayerBuilder("Owner")) { };
            var card = new Card { Name = "FullCard", Controller = controller };

            hand.Owners.Add(controller);

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => hand.AddCard(card));

            // Assert
            Assert.Equal("The hand is full.", ex.Message);
            Assert.Empty(hand.Content);
        }

        /// <summary>
        /// Verifies HasReachedLimit returns expected results for a variety of MaxSize and content-count combinations.
        /// Input conditions: combinations include extreme integer boundaries (int.MinValue, int.MaxValue), zero, negative, equal, less-than and greater-than relationships between Content.Count and MaxSize.
        /// Expected result: HasReachedLimit returns true when Content.Count >= MaxSize, otherwise false.
        /// </summary>
        [Theory]
        [MemberData(nameof(HasReachedLimitTestData))]
        public void HasReachedLimit_ContentCountAndMaxSize_ReturnsExpected(int maxSize, int contentCount, bool expected)
        {
            // Arrange
            var hand = new Hand(maxSize);

            // Populate Content with the requested number of Card instances
            for (int i = 0; i < contentCount; i++)
            {
                // Card has a parameterless constructor and public setters; populate minimal data
                hand.Content.Add(new Card { Name = $"Card{i}" });
            }

            // Act
            var result = hand.HasReachedLimit();

            // Assert
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> HasReachedLimitTestData()
        {
            // Format per case: maxSize, contentCount, expectedResult

            // int.MaxValue with empty content -> should not be reached
            yield return new object[] { int.MaxValue, 0, false };

            // int.MinValue with empty content -> always reached because any non-negative Count >= int.MinValue
            yield return new object[] { int.MinValue, 0, true };

            // zero max size; empty content (Count == 0) -> reached
            yield return new object[] { 0, 0, true };

            // typical less-than case
            yield return new object[] { 1, 0, false };

            // exact equality case
            yield return new object[] { 1, 1, true };

            // greater-than case
            yield return new object[] { 1, 2, true };

            // negative max size (e.g., -1) with empty content -> reached (0 >= -1)
            yield return new object[] { -1, 0, true };

            // moderate equal case: contentCount equals maxSize
            yield return new object[] { 5, 5, true };

            // moderate less-than case
            yield return new object[] { 5, 4, false };
        }

        /// <summary>
        /// Verifies that Search returns the first card whose Name exactly matches the provided search string.
        /// Input conditions: various card name layouts (regular, empty, whitespace, very long, special characters) where at least one card matches.
        /// Expected result: The first matching Card instance from Hand.Content is returned.
        /// </summary>
        [Theory]
        [MemberData(nameof(MatchingCases))]
        public void Search_WithMatchingNames_ReturnsFirstMatch(string searchName, List<string> cardNames, int expectedIndex)
        {
            // Arrange
            var hand = new Hand(maxSize: 10);

            foreach (var n in cardNames)
            {
                hand.Content.Add(new Card { Name = n });
            }

            // Act
            var result = hand.Search(searchName);

            // Assert
            Assert.Same(hand.Content[expectedIndex], result);
        }

        /// <summary>
        /// Verifies that Search returns null when no card name exactly matches the provided search string.
        /// Input conditions: content empty, single non-matching, multiple non-matching, and case-mismatch scenarios.
        /// Expected result: null is returned.
        /// </summary>
        [Theory]
        [MemberData(nameof(NonMatchingCases))]
        public void Search_WithNonMatchingNames_ReturnsNull(string searchName, List<string> cardNames)
        {
            // Arrange
            var hand = new Hand(maxSize: 5);

            foreach (var n in cardNames)
            {
                hand.Content.Add(new Card { Name = n });
            }

            // Act
            var result = hand.Search(searchName);

            // Assert
            Assert.Null(result);
        }

        // Test data for matching scenarios
        public static IEnumerable<object[]> MatchingCases()
        {
            // Single exact match at first position
            yield return new object[]
            {
                "Alpha",
                new List<string> { "Alpha", "Beta", "Gamma" },
                0
            };

            // Match not at first position -> ensure first matching is returned
            yield return new object[]
            {
                "Beta",
                new List<string> { "Alpha", "Beta", "Beta", "Gamma" },
                1
            };

            // Empty string match
            yield return new object[]
            {
                "",
                new List<string> { "", "NonEmpty" },
                0
            };

            // Whitespace-only name match
            yield return new object[]
            {
                "   ",
                new List<string> { "   ", " " },
                0
            };

            // Very long string match
            yield return new object[]
            {
                new string('x', 1000),
                new List<string> { new string('x', 1000), "other" },
                0
            };

            // Special/control characters
            yield return new object[]
            {
                "Line\nBreak",
                new List<string> { "Line\nBreak", "Line\\Break" },
                0
            };
        }

        // Test data for non-matching scenarios
        public static IEnumerable<object[]> NonMatchingCases()
        {
            // Empty content
            yield return new object[]
            {
                "Anything",
                new List<string>()
            };

            // Single non-matching entry
            yield return new object[]
            {
                "Alpha",
                new List<string> { "Beta" }
            };

            // Multiple non-matching entries
            yield return new object[]
            {
                "Gamma",
                new List<string> { "Alpha", "Beta", "" }
            };

            // Case-sensitivity: different casing should not match
            yield return new object[]
            {
                "alpha", // lower-case search
                new List<string> { "Alpha", "alphaX" } // only "Alpha" (different case) and "alphaX" (different content) exist
            };

            // Special characters that differ slightly
            yield return new object[]
            {
                "Line\rBreak",
                new List<string> { "Line\nBreak", "Line\\Break" }
            };
        }

        // Minimal PlayerBuilder helper used only to construct Player instances in tests.
        // This is an inner helper to avoid creating test-only external types.
        // It mirrors the minimal contract used by Player's constructor in production code.
        private class PlayerBuilder
        {
            public string Name { get; }
            public Dictionary<string, int> Stats { get; } = new Dictionary<string, int>();
            public Dictionary<string, IZone> Zones { get; } = new Dictionary<string, IZone>();
            public object Controller { get; } = null;

            public PlayerBuilder(string name)
            {
                Name = name;
            }
        }

        /// <summary>
        /// Verifies that the constructor Hand(int maxSize) assigns the provided MaxSize value
        /// and initializes the Owners and Content collections to non-null, empty lists.
        /// Input conditions: various maxSize values (int.MinValue, -1, 0, 1, int.MaxValue).
        /// Expected result: Hand.MaxSize equals the provided value; Owners and Content are non-null and empty.
        /// </summary>
        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        public void Hand_Ctor_WithVariousMaxSizes_SetsMaxSizeAndInitializesCollections(int maxSize)
        {
            // Arrange
            // (no additional setup required)

            // Act
            Hand hand = new Hand(maxSize);

            // Assert
            // MaxSize must be exactly the provided value
            Assert.Equal(maxSize, hand.MaxSize);

            // Owners and Content must be initialized (non-null) and empty
            Assert.NotNull(hand.Owners);
            Assert.Equal(0, hand.Owners.Count);

            Assert.NotNull(hand.Content);
            Assert.Equal(0, hand.Content.Count);
        }

        /// <summary>
        /// Verifies that multiple Hand instances each get their own distinct Owners and Content lists.
        /// Input conditions: two Hand instances constructed with different maxSize values.
        /// Expected result: Owners and Content references are not the same between instances (no shared static lists).
        /// </summary>
        [Fact]
        public void Hand_Ctor_MultipleInstances_HaveIndependentCollections()
        {
            // Arrange
            int firstSize = 3;
            int secondSize = 5;

            // Act
            Hand first = new Hand(firstSize);
            Hand second = new Hand(secondSize);

            // Assert
            // Ensure MaxSize values are set independently
            Assert.Equal(firstSize, first.MaxSize);
            Assert.Equal(secondSize, second.MaxSize);

            // Ensure Owners and Content collections are separate instances (not shared)
            Assert.NotSame(first.Owners, second.Owners);
            Assert.NotSame(first.Content, second.Content);

            // Both should be empty initially
            Assert.Equal(0, first.Owners.Count);
            Assert.Equal(0, second.Owners.Count);
            Assert.Equal(0, first.Content.Count);
            Assert.Equal(0, second.Content.Count);
        }

        /// <summary>
        /// Verifies that calling DiscardAll when the hand Content is empty does nothing.
        /// Input conditions: Hand with no cards.
        /// Expected result: No calls to the discard zone's AddCard and the hand remains empty.
        /// </summary>
        [Fact]
        public void DiscardAll_EmptyContent_NoAddCardCallsAndContentRemainsEmpty()
        {
            // Arrange
            var hand = new Hand(maxSize: 5);
            var discardMock = new Mock<IZone>();

            // Act
            hand.DiscardAll(discardMock.Object);

            // Assert
            discardMock.Verify(d => d.AddCard(It.IsAny<Card>()), Times.Never);
            Assert.Equal(0, hand.Content.Count);
        }

        /// <summary>
        /// Verifies that DiscardAll moves all cards to the discard zone when all cards have valid owners.
        /// Input conditions: Hand with two cards, both controllers are present in Owners.
        /// Expected result: AddCard is invoked once per card and the hand Content is empty afterwards.
        /// </summary>
        [Fact]
        public void DiscardAll_AllCardsHaveOwners_AllMovedToDiscardAndContentEmpty()
        {
            // Arrange
            var builder = new CGRuleEngine_TFG.Entities.Players.PlayerBuilder("PlayerA");
            var player = builder.Build();

            var hand = new Hand(maxSize: 10);
            hand.Owners.Add(player);

            var card1 = new Card { Name = "Card1", Controller = player };
            var card2 = new Card { Name = "Card2", Controller = player };

            hand.Content.Add(card1);
            hand.Content.Add(card2);

            var discardMock = new Mock<IZone>();

            // Act
            hand.DiscardAll(discardMock.Object);

            // Assert
            discardMock.Verify(d => d.AddCard(It.Is<Card>(c => object.ReferenceEquals(c, card1))), Times.Once);
            discardMock.Verify(d => d.AddCard(It.Is<Card>(c => object.ReferenceEquals(c, card2))), Times.Once);
            Assert.Equal(0, hand.Content.Count);
        }

        /// <summary>
        /// Verifies behavior when one card cannot be moved because its controller is not an owner.
        /// Input conditions: Hand with two cards where the first has a valid owner and the second does not.
        /// Expected result: DiscardAll throws InvalidOperationException when processing the invalid card,
        /// earlier valid cards are moved (AddCard called), and remaining invalid cards stay in the hand.
        /// </summary>
        [Fact]
        public void DiscardAll_PartialFailure_ThrowsAndStopsProcessingAfterError()
        {
            // Arrange
            var builderA = new CGRuleEngine_TFG.Entities.Players.PlayerBuilder("Owner");
            var owner = builderA.Build();

            var builderB = new CGRuleEngine_TFG.Entities.Players.PlayerBuilder("NotOwner");
            var notOwner = builderB.Build();

            var hand = new Hand(maxSize: 10);
            // only owner is added to Owners
            hand.Owners.Add(owner);

            var validCard = new Card { Name = "ValidCard", Controller = owner };
            var invalidCard = new Card { Name = "InvalidCard", Controller = notOwner };

            // Add cards in order: valid then invalid
            hand.Content.Add(validCard);
            hand.Content.Add(invalidCard);

            var discardMock = new Mock<IZone>();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => hand.DiscardAll(discardMock.Object));
            Assert.Equal("The player is not an owner of the hand.", ex.Message);

            // The valid card should have been moved before the exception (AddCard called once)
            discardMock.Verify(d => d.AddCard(It.Is<Card>(c => object.ReferenceEquals(c, validCard))), Times.Once);

            // The invalid card must still be present in Content and valid card removed
            Assert.DoesNotContain(validCard, hand.Content);
            Assert.Contains(invalidCard, hand.Content);

            // Ensure AddCard was not called for the invalid card
            discardMock.Verify(d => d.AddCard(It.Is<Card>(c => object.ReferenceEquals(c, invalidCard))), Times.Never);
        }

        /// <summary>
        /// Verifies that RemoveCard throws InvalidOperationException when the card's controller is not an owner of the hand.
        /// Input conditions: Hand with no owners; Card whose Controller is a Player instance not present in Owners.
        /// Expected result: InvalidOperationException with message "The player is not an owner of the hand." is thrown.
        /// </summary>
        [Fact]
        public void RemoveCard_ControllerNotOwner_ThrowsInvalidOperationException()
        {
            // Arrange
            var hand = new Hand(maxSize: 5);
            // Build a player who will be the controller of the card but NOT added to hand.Owners
            var controllerBuilder = new Players.PlayerBuilder("Controller");
            var controller = controllerBuilder.Build();

            var card = new Card
            {
                Name = "TestCard",
                Controller = controller,
                Owner = controller
            };

            // Ensure preconditions: Owners does not contain the controller
            Assert.DoesNotContain(controller, hand.Owners);

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => hand.RemoveCard(card));

            // Assert
            Assert.Equal("The player is not an owner of the hand.", ex.Message);
        }

        /// <summary>
        /// Verifies that RemoveCard throws InvalidOperationException when the card is not present in the hand's Content,
        /// even if the card's controller is an owner.
        /// Input conditions: Hand whose Owners contains the card.Controller, but Content does NOT contain the card.
        /// Expected result: InvalidOperationException with message "The card is not in the hand." is thrown.
        /// </summary>
        [Fact]
        public void RemoveCard_CardNotInHand_ThrowsInvalidOperationException()
        {
            // Arrange
            var hand = new Hand(maxSize: 5);
            var controllerBuilder = new Players.PlayerBuilder("Owner");
            var owner = controllerBuilder.Build();

            // Add owner to hand
            hand.Owners.Add(owner);

            var card = new Card
            {
                Name = "AbsentCard",
                Controller = owner,
                Owner = owner
            };

            // Precondition checks
            Assert.Contains(owner, hand.Owners);
            Assert.DoesNotContain(card, hand.Content);

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => hand.RemoveCard(card));

            // Assert
            Assert.Equal("The card is not in the hand.", ex.Message);
        }

        /// <summary>
        /// Verifies that RemoveCard successfully removes a card from Content when the controller is an owner and the card exists in the hand.
        /// Input conditions: Hand whose Owners contains the card.Controller and Content contains the card.
        /// Expected result: Content no longer contains the card after RemoveCard; no exception thrown.
        /// </summary>
        [Fact]
        public void RemoveCard_Valid_RemovesCardFromContent()
        {
            // Arrange
            var hand = new Hand(maxSize: 5);
            var controllerBuilder = new Players.PlayerBuilder("Owner2");
            var owner = controllerBuilder.Build();

            // Add owner and card to hand
            hand.Owners.Add(owner);

            var card = new Card
            {
                Name = "PresentCard",
                Controller = owner,
                Owner = owner
            };

            hand.Content.Add(card);

            // Precondition checks
            Assert.Contains(owner, hand.Owners);
            Assert.Contains(card, hand.Content);

            // Act
            hand.RemoveCard(card);

            // Assert
            Assert.DoesNotContain(card, hand.Content);
        }
    }
}