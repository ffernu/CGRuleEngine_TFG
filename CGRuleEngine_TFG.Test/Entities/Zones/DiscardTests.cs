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
    /// Tests for CGRuleEngine_TFG.Entities.Zones.Discard
    /// </summary>
    public class DiscardTests
    {
        /// <summary>
        /// Verifies that Clone returns a new Discard instance which is not the same reference as the original.
        /// Input conditions: original is a freshly constructed Discard with default-initialized collections.
        /// Expected result: Clone returns a non-null Discard, the returned instance is a distinct object,
        /// its Content and Owners are non-null, empty, and are different instances than the original's lists.
        /// </summary>
        [Fact]
        public void Clone_DefaultDiscard_ReturnsDistinctDiscardWithEmptyCollections()
        {
            // Arrange
            var original = new Discard();

            // Act
            var zone = original.Clone();

            // Assert
            Assert.NotNull(zone);
            var clone = Assert.IsType<Discard>(zone);

            // Instance-level assertions
            Assert.NotSame(original, clone);

            // Collections should be initialized and empty
            Assert.NotNull(clone.Content);
            Assert.NotNull(clone.Owners);
            Assert.Equal(0, clone.Content.Count);
            Assert.Equal(0, clone.Owners.Count);

            // Lists must be distinct instances so changes on one won't affect the other
            Assert.NotSame(original.Content, clone.Content);
            Assert.NotSame(original.Owners, clone.Owners);
        }

        /// <summary>
        /// Verifies that mutating the original Discard's collection properties after cloning does not affect the clone.
        /// Input conditions: clone obtained from a freshly constructed Discard, then original's Content and Owners are reassigned.
        /// Expected result: clone retains its original independent collections (different references and unchanged counts).
        /// </summary>
        [Fact]
        public void Clone_AfterOriginalMutated_CloneRemainsIndependent()
        {
            // Arrange
            var original = new Discard();
            var clone = (Discard)original.Clone();

            // Act - mutate original by reassigning the collection properties to new list instances
            original.Content = new List<Card>();
            original.Owners = new List<Player>();

            // Assert - collections on clone remain independent and unchanged
            Assert.NotSame(original.Content, clone.Content);
            Assert.NotSame(original.Owners, clone.Owners);

            Assert.NotNull(clone.Content);
            Assert.NotNull(clone.Owners);
            Assert.Equal(0, clone.Content.Count);
            Assert.Equal(0, clone.Owners.Count);
        }

        /// <summary>
        /// Verifies that MoveCard throws InvalidOperationException when the provided card is not present in the discard's Content.
        /// Input conditions: a Discard with empty Content and a Card instance that is not added to Content.
        /// Expected result: InvalidOperationException is thrown with message "The card is not in the discard.".
        /// </summary>
        [Fact]
        public void MoveCard_CardNotInDiscard_ThrowsInvalidOperationException()
        {
            // Arrange
            var discard = new Discard();
            var card = new Card { Name = "NotPresentCard" };
            var targetMock = new Mock<IZone>();

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => discard.MoveCard(card, targetMock.Object));

            // Assert
            Assert.Equal("The card is not in the discard.", ex.Message);
            // Ensure target zone was not called
            targetMock.Verify(t => t.AddCard(It.IsAny<Card>()), Times.Never);
        }

        /// <summary>
        /// Verifies that MoveCard removes the card from Content and calls AddCard on the target zone when the card is present.
        /// Input conditions: a Discard whose Content contains a single Card instance and a mocked target IZone.
        /// Expected result: the card is removed from discard.Content and targetZone.AddCard is invoked exactly once with the same card.
        /// </summary>
        [Fact]
        public void MoveCard_CardInContent_RemovesCardAndCallsTargetAddCard()
        {
            // Arrange
            var discard = new Discard();
            var card = new Card { Name = "PresentCard" };
            discard.Content.Add(card);

            var targetMock = new Mock<IZone>();
            // No setup required for AddCard (void), verification will assert invocation.

            // Act
            discard.MoveCard(card, targetMock.Object);

            // Assert
            Assert.DoesNotContain(card, discard.Content);
            targetMock.Verify(t => t.AddCard(card), Times.Once);
        }

        /// <summary>
        /// Verifies that MoveAll calls AddCard on the target zone once per card in the discard's Content
        /// and that Content is empty after a successful move.
        /// Input conditions: discard.Content populated with a variable number of Card instances (0,1,3).
        /// Expected result: target.AddCard invoked exactly for each card and discard.Content is empty.
        /// </summary>
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(3)]
        public void MoveAll_WithVariousCounts_CallsAddCardForEachAndEmptiesContent(int initialCount)
        {
            // Arrange
            var discard = new Discard();
            var cards = new List<Card>();
            for (int i = 0; i < initialCount; i++)
            {
                var c = new Card { Name = $"Card{i + 1}" };
                cards.Add(c);
                discard.Content.Add(c);
            }

            var targetMock = new Mock<IZone>(MockBehavior.Strict);

            // Setup: expect AddCard calls for each card if any; Strict mock requires setups for expected calls.
            if (initialCount == 0)
            {
                // No setup required; verify no calls are made.
            }
            else
            {
                foreach (var c in cards)
                {
                    // For each expected card call, allow a call that does nothing.
                    targetMock.Setup(t => t.AddCard(It.Is<Card>(x => ReferenceEquals(x, c))));
                }
            }

            // Act
            discard.MoveAll(targetMock.Object);

            // Assert
            if (initialCount == 0)
            {
                targetMock.Verify(t => t.AddCard(It.IsAny<Card>()), Times.Never);
            }
            else
            {
                targetMock.Verify(t => t.AddCard(It.IsAny<Card>()), Times.Exactly(initialCount));
                // Also verify each specific card was passed
                foreach (var c in cards)
                {
                    targetMock.Verify(t => t.AddCard(It.Is<Card>(x => ReferenceEquals(x, c))), Times.Once);
                }
            }

            Assert.NotNull(discard.Content);
            Assert.Equal(0, discard.Content.Count);
        }

        /// <summary>
        /// Verifies behavior when the target zone throws during AddCard for one of the cards:
        /// Input conditions: discard.Content has three cards; target.AddCard throws for the second card.
        /// Expected result: an exception is thrown; cards processed before the exception are removed from Content;
        /// the card that caused the exception is also removed (MoveCard removes before calling AddCard);
        /// remaining unprocessed cards remain in Content.
        /// </summary>
        [Fact]
        public void MoveAll_WhenTargetAddThrows_ExceptionIsPropagatedAndProcessedCardsAreRemoved()
        {
            // Arrange
            var discard = new Discard();
            var card1 = new Card { Name = "Card1" };
            var card2 = new Card { Name = "Card2" };
            var card3 = new Card { Name = "Card3" };

            discard.Content.Add(card1);
            discard.Content.Add(card2);
            discard.Content.Add(card3);

            var targetMock = new Mock<IZone>();

            // Setup: AddCard succeeds for card1, throws for card2, (card3 should not be reached)
            targetMock.Setup(t => t.AddCard(It.Is<Card>(c => ReferenceEquals(c, card1))));
            targetMock.Setup(t => t.AddCard(It.Is<Card>(c => ReferenceEquals(c, card2))))
                      .Throws(new InvalidOperationException("simulated failure for card2"));

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => discard.MoveAll(targetMock.Object));
            Assert.Equal("simulated failure for card2", ex.Message);

            // card1 was successfully moved (removed from Content)
            Assert.DoesNotContain(card1, discard.Content);

            // card2 was removed before AddCard threw, so it is not in Content
            Assert.DoesNotContain(card2, discard.Content);

            // card3 was not processed due to exception, so it remains in Content
            Assert.Contains(card3, discard.Content);
            Assert.Equal(1, discard.Content.Count);

            // Verify AddCard was called for card1 and card2 (card2 call threw)
            targetMock.Verify(t => t.AddCard(It.Is<Card>(c => ReferenceEquals(c, card1))), Times.Once);
            targetMock.Verify(t => t.AddCard(It.Is<Card>(c => ReferenceEquals(c, card2))), Times.Once);

            // Ensure AddCard was not called for card3
            targetMock.Verify(t => t.AddCard(It.Is<Card>(c => ReferenceEquals(c, card3))), Times.Never);
        }

        /// <summary>
        /// Verifies that GetTopCard returns the last card in Content when the discard contains one or more cards.
        /// Input conditions: discard.Content populated with N cards (N provided via InlineData).
        /// Expected result: returned Card is the card at index Content.Count - 1 and Content count remains unchanged.
        /// </summary>
        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        public void GetTopCard_WithItems_ReturnsLastCard(int count)
        {
            // Arrange
            var discard = new Discard();
            var cards = new List<Card>(count);
            for (int i = 0; i < count; i++)
            {
                var card = new Card
                {
                    // Name set to unique value to help identification
                    Name = $"Card_{i}",
                    id = i,
                    isFaceUp = false,
                    isTapped = false,
                    Owner = null,
                    Controller = null
                };
                cards.Add(card);
                discard.Content.Add(card); // directly populate Content to avoid owner checks in AddCard
            }

            int beforeCount = discard.Content.Count;
            var expectedTop = discard.Content[discard.Content.Count - 1];

            // Act
            var top = discard.GetTopCard();

            // Assert
            Assert.Same(expectedTop, top);
            Assert.Equal(beforeCount, discard.Content.Count); // ensure GetTopCard does not remove the card
        }

        /// <summary>
        /// Verifies that GetTopCard throws InvalidOperationException when the discard pile is empty.
        /// Input conditions: discard.Content is empty (no cards).
        /// Expected result: InvalidOperationException is thrown with message "The discard pile is empty.".
        /// </summary>
        [Fact]
        public void GetTopCard_Empty_ThrowsInvalidOperationException()
        {
            // Arrange
            var discard = new Discard();
            // Ensure content is empty
            discard.Content.Clear();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => discard.GetTopCard());
            Assert.Equal("The discard pile is empty.", ex.Message);
        }

        /// <summary>
        /// Verifies that Search returns null when the discard is empty or when no card matches the searched name.
        /// Input conditions: various search name values (empty, whitespace, very long, special/control characters, non-existing).
        /// Expected result: Search returns null for each provided search name because the discard only contains non-matching card names.
        /// </summary>
        [Theory]
        [MemberData(nameof(NameTestData))]
        public void Search_NoMatchingName_ReturnsNull(string searchName)
        {
            // Arrange
            var discard = new Discard();
            // Populate with known non-matching cards
            discard.Content.Add(new Card { Name = "Alpha" });
            discard.Content.Add(new Card { Name = "Beta" });

            // Act
            var result = discard.Search(searchName);

            // Assert
            Assert.Null(result);
        }

        public static IEnumerable<object[]> NameTestData()
        {
            yield return new object[] { "" }; // empty string
            yield return new object[] { "   " }; // whitespace-only
            yield return new object[] { new string('x', 1000) }; // very long string
            yield return new object[] { "§ç©\u0001" }; // special + control character
            yield return new object[] { "NonExistingName_12345" }; // normal non-existing
        }

        /// <summary>
        /// Verifies that Search returns the first matching Card when multiple cards share the same Name.
        /// Input conditions: two Card instances with identical Name inserted in order (first, then second).
        /// Expected result: Search returns the first (earliest added) Card instance reference.
        /// </summary>
        [Fact]
        public void Search_MultipleCardsWithSameName_ReturnsFirstOccurrence()
        {
            // Arrange
            var discard = new Discard();
            var firstCard = new Card { Name = "DuplicateName" };
            var secondCard = new Card { Name = "DuplicateName" };

            discard.Content.Add(firstCard);   // index 0
            discard.Content.Add(secondCard);  // index 1

            // Act
            var result = discard.Search("DuplicateName");

            // Assert
            Assert.Same(firstCard, result);
        }

        /// <summary>
        /// Verifies that Search is case-sensitive: searching for a name with different casing does not match.
        /// Input conditions: one Card with Name \"CaseSensitive\". Search performed with different-cased string.
        /// Expected result: Search returns null because the comparison is case-sensitive.
        /// </summary>
        [Fact]
        public void Search_CaseSensitivity_DifferentCasingDoesNotMatch()
        {
            // Arrange
            var discard = new Discard();
            discard.Content.Add(new Card { Name = "CaseSensitive" });

            // Act
            var result = discard.Search("casesensitive"); // different casing

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// Verifies that Search does not modify the discard Content list.
        /// Input conditions: one Card present with Name \"ImmutableTest\".
        /// Expected result: Search returns the matching Card and Content count remains unchanged.
        /// </summary>
        [Fact]
        public void Search_DoesNotModifyContent_ReturnsMatchAndKeepsCount()
        {
            // Arrange
            var discard = new Discard();
            var card = new Card { Name = "ImmutableTest" };
            discard.Content.Add(card);
            int beforeCount = discard.Content.Count;

            // Act
            var result = discard.Search("ImmutableTest");

            // Assert
            Assert.Same(card, result);
            Assert.Equal(beforeCount, discard.Content.Count);
        }

        /// <summary>
        /// Verifies that the default constructor initializes Owners and Content as non-null, empty lists of the correct types.
        /// Input conditions: call the parameterless constructor.
        /// Expected result: Owners and Content are non-null, have Count == 0, and are of type List&lt;Player&gt; and List&lt;Card&gt; respectively.
        /// </summary>
        [Xunit.Fact]
        public void Discard_Ctor_InitializesOwnersAndContentAsEmptyLists()
        {
            // Arrange
            // (no setup required)

            // Act
            var discard = new Discard();

            // Assert
            // Owners initialization
            Assert.NotNull(discard.Owners);
            Assert.IsType<List<Player>>(discard.Owners);
            Assert.Equal(0, discard.Owners.Count);

            // Content initialization
            Assert.NotNull(discard.Content);
            Assert.IsType<List<Card>>(discard.Content);
            Assert.Equal(0, discard.Content.Count);

            // Ensure Owners and Content are distinct instances
            Assert.NotSame(discard.Owners, discard.Content);
        }

        /// <summary>
        /// Verifies that newly constructed Discard has writable collection properties (can assign new lists).
        /// Input conditions: create a Discard and assign new List instances to Owners and Content.
        /// Expected result: properties accept new List instances and reflect those assignments.
        /// </summary>
        [Xunit.Fact]
        public void Discard_Ctor_AllowsAssignmentOfNewCollections()
        {
            // Arrange
            var discard = new Discard();

            // Act
            var newOwners = new List<Player> { };
            var newContent = new List<Card> { };
            discard.Owners = newOwners;
            discard.Content = newContent;

            // Assert
            Assert.Same(newOwners, discard.Owners);
            Assert.Same(newContent, discard.Content);
        }

    }
}