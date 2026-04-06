using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using CGRuleEngine_TFG.Entities;
using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Zones;
using Moq;
using Xunit;

namespace CGRuleEngine_TFG.Entities.Zones.UnitTests
{
    /// <summary>
    /// Tests for CGRuleEngine_TFG.Entities.Zones.Deck.Clone method.
    /// </summary>
    public class DeckTests
    {
        /// <summary>
        /// Verifies that Clone creates a new Deck instance with the same maxSize and minSize,
        /// that the returned instance is independent (distinct reference) and that its
        /// Content and Owners collections are new empty lists (modifications in either instance do not affect the other).
        /// Input conditions: various combinations of maxSize/minSize including extremes and negative values.
        /// Expected result: cloned deck has identical size properties, empty collections, and independent references.
        /// </summary>
        [Theory]
        [MemberData(nameof(SizePairs))]
        public void Clone_WithVariousSizes_CreatesIndependentDecks(int maxSize, int minSize)
        {
            // Arrange
            var original = new Deck(maxSize, minSize);

            // Populate original so we can assert clone is empty and independent
            original.Content.Add(new Card { Name = "OriginalCard1" });
            original.Owners.Add(new Player(new Players.PlayerBuilder("Owner1")));

            // Act
            IZone clonedZone = original.Clone();
            Deck clone = Assert.IsType<Deck>(clonedZone);

            // Assert - size properties are copied
            Assert.Equal(original.maxSize, clone.maxSize);
            Assert.Equal(original.minSize, clone.minSize);

            // Assert - instances are different
            Assert.NotSame(original, clone);

            // Assert - collections are initialized and empty on the clone
            Assert.NotNull(clone.Content);
            Assert.Equal(0, clone.Content.Count);

            Assert.NotNull(clone.Owners);
            Assert.Equal(0, clone.Owners.Count);

            // Assert - collections are different objects (no shared references)
            Assert.NotSame(original.Content, clone.Content);
            Assert.NotSame(original.Owners, clone.Owners);

            // Mutate original after cloning and verify clone unaffected
            original.Content.Add(new Card { Name = "OriginalCard2" });
            original.Owners.Add(new Player(new Players.PlayerBuilder("Owner2")));
            Assert.Equal(2, original.Content.Count); // sanity on original
            Assert.Equal(0, clone.Content.Count);    // clone remains empty

            // Mutate clone and verify original unaffected
            clone.Content.Add(new Card { Name = "CloneCard1" });
            clone.Owners.Add(new Player(new Players.PlayerBuilder("CloneOwner1")));
            Assert.Equal(2, original.Owners.Count); // original owners count remains as before (2)
            Assert.Equal(2, original.Content.Count); // original still has its items
            Assert.Equal(1, clone.Content.Count);    // clone has its own item
        }

        /// <summary>
        /// Provides a set of (maxSize, minSize) pairs to exercise normal, boundary and extreme values.
        /// Includes int.MinValue and int.MaxValue to ensure Clone copies values exactly.
        /// </summary>
        public static IEnumerable<object[]> SizePairs()
        {
            yield return new object[] { 0, 0 };
            yield return new object[] { int.MinValue, int.MinValue };
            yield return new object[] { int.MaxValue, int.MaxValue };
            yield return new object[] { -1, 1 };
            yield return new object[] { 1, -1 };
            yield return new object[] { 10, -5 };
        }

        /// <summary>
        /// Verifies that MoveCard throws InvalidOperationException when the provided card is not present in the deck.
        /// Input: a Deck with no cards in Content, a Card instance, and a mocked IZone.
        /// Expected: InvalidOperationException is thrown with message "The card is not in the deck." and targetZone.AddCard is not called.
        /// </summary>
        [Fact]
        public void MoveCard_CardNotInDeck_ThrowsInvalidOperationException()
        {
            // Arrange
            var deck = new Deck(maxSize: 60, minSize: 0);
            var card = new Card { Name = "MissingCard", id = 1 };
            var targetZoneMock = new Mock<IZone>();

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => deck.MoveCard(card, targetZoneMock.Object));

            // Assert
            Assert.Equal("The card is not in the deck.", ex.Message);
            targetZoneMock.Verify(t => t.AddCard(It.IsAny<Card>()), Times.Never);
        }

        /// <summary>
        /// Verifies that MoveCard removes the card from the deck's Content and calls AddCard on the target zone when the card is present.
        /// Input: a Deck containing the Card in Content and a mocked IZone.
        /// Expected: the card is removed from deck.Content and targetZone.AddCard is invoked exactly once with the same card.
        /// </summary>
        [Fact]
        public void MoveCard_CardInDeck_RemovesFromContentAndAddsToTargetZone()
        {
            // Arrange
            var deck = new Deck(maxSize: 60, minSize: 0);
            var card = new Card { Name = "PresentCard", id = 2 };
            // Bypass owner checks by directly manipulating Content
            deck.Content.Add(card);

            var targetZoneMock = new Mock<IZone>();

            // Act
            deck.MoveCard(card, targetZoneMock.Object);

            // Assert
            Assert.DoesNotContain(card, deck.Content);
            targetZoneMock.Verify(t => t.AddCard(card), Times.Once);
        }

        /// <summary>
        /// Verifies that Search returns null when the deck content is empty.
        /// Input conditions: empty Content list and arbitrary search name.
        /// Expected result: null is returned.
        /// </summary>
        [Fact]
        public void Search_ContentEmpty_ReturnsNull()
        {
            // Arrange
            var deck = new Deck(maxSize: 60, minSize: 40);
            // ensure content is empty
            deck.Content.Clear();

            // Act
            Card? result = deck.Search("AnyName");

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// Parameterized tests for Search with various content and search-name inputs.
        /// Input conditions: different card name arrays and search name values including:
        ///  - exact match
        ///  - case-different name (should not match)
        ///  - empty string
        ///  - whitespace-only string
        ///  - very long string
        ///  - special/control characters
        ///  - multiple occurrences (should return the first match)
        ///  - not found (should return null)
        /// Expected result: when a matching card exists, the first matching Card instance from Content is returned; otherwise null.
        /// </summary>
        [Theory]
        [MemberData(nameof(SearchTestCases))]
        public void Search_VariousInputs_ReturnsExpectedCard(string[] cardNames, string searchName, int? expectedIndex)
        {
            // Arrange
            var deck = new Deck(maxSize: 60, minSize: 40);
            deck.Content.Clear();
            foreach (var n in cardNames)
            {
                deck.Content.Add(new Card { Name = n });
            }

            // Act
            Card? found = deck.Search(searchName);

            // Assert
            if (expectedIndex.HasValue)
            {
                Assert.NotNull(found);
                // Validate that the returned instance is exactly the one at the expected index
                Assert.Same(deck.Content[expectedIndex.Value], found);
            }
            else
            {
                Assert.Null(found);
            }
        }

        public static IEnumerable<object[]> SearchTestCases()
        {
            // exact match
            yield return new object[] { new string[] { "Alpha" }, "Alpha", 0 };

            // case sensitivity: should not match
            yield return new object[] { new string[] { "Alpha" }, "alpha", (int?)null };

            // empty string name
            yield return new object[] { new string[] { "" }, "", 0 };

            // whitespace-only name
            yield return new object[] { new string[] { "   " }, "   ", 0 };

            // very long string
            string longName = new string('a', 10000);
            yield return new object[] { new string[] { longName }, longName, 0 };

            // special and control characters
            string special = "Line1\nLine2\u2603\0End";
            yield return new object[] { new string[] { special }, special, 0 };

            // multiple occurrences -> first match returned
            yield return new object[] { new string[] { "X", "Y", "X" }, "X", 0 };

            // not found returns null
            yield return new object[] { new string[] { "A", "B" }, "C", (int?)null };

            // multiple different names, target in middle
            yield return new object[] { new string[] { "One", "Target", "Three" }, "Target", 1 };
        }

        /// <summary>
        /// Verifies that calling Suffle on an empty deck or a deck with a single card does not change the content sequence.
        /// Input conditions: deck with 0 and 1 card(s).
        /// Expected result: Content.Count remains equal to the original count and the sequence is unchanged.
        /// </summary>
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void Suffle_EmptyOrSingle_ContentUnchanged(int initialCount)
        {
            // Arrange
            var deck = new Deck(maxSize: 10, minSize: 0);
            for (int i = 0; i < initialCount; i++)
            {
                deck.Content.Add(new Card { id = i, Name = $"Card{i}" });
            }

            var original = new List<Card>(deck.Content);

            // Act
            deck.Suffle();

            // Assert
            Assert.Equal(original.Count, deck.Content.Count); // count unchanged
            // For empty and single-element lists, sequence must remain the same (reference equality)
            Assert.Equal(original, deck.Content);
        }

        /// <summary>
        /// Verifies that Suffle preserves the multiset of cards and the count for a deck with multiple cards.
        /// Input conditions: deck with multiple distinct Card instances.
        /// Expected result: After shuffling, no cards are lost or duplicated; counts and identities are preserved.
        /// </summary>
        [Fact]
        public void Suffle_Multiple_PreservesElementsAndCount()
        {
            // Arrange
            var deck = new Deck(maxSize: 100, minSize: 0);
            var originalCards = Enumerable.Range(0, 10)
                                          .Select(i => new Card { id = i, Name = $"Card{i}" })
                                          .ToList();
            foreach (var c in originalCards)
            {
                deck.Content.Add(c);
            }

            var originalOrder = new List<Card>(deck.Content);
            var originalIdsSorted = originalOrder.Select(c => c.id).OrderBy(i => i).ToList();

            // Act
            deck.Suffle();

            // Assert
            // Count must remain the same
            Assert.Equal(originalOrder.Count, deck.Content.Count);

            // All original card instances must still be present (no loss and no duplication)
            foreach (var c in originalOrder)
            {
                Assert.Contains(c, deck.Content);
            }

            // The multiset of ids must match exactly (order-independent check)
            var afterIdsSorted = deck.Content.Select(c => c.id).OrderBy(i => i).ToList();
            Assert.Equal(originalIdsSorted, afterIdsSorted);
        }

        /// <summary>
        /// Verifies that Draw throws InvalidOperationException when the deck is empty.
        /// Input conditions: newly created Deck with no cards in Content.
        /// Expected result: InvalidOperationException is thrown with message "The deck is empty.".
        /// </summary>
        [Fact]
        public void Draw_EmptyDeck_ThrowsInvalidOperationExceptionWithMessage()
        {
            // Arrange
            var deck = new Deck(maxSize: 10, minSize: 0);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => deck.Draw());
            Assert.Equal("The deck is empty.", ex.Message);
        }

        /// <summary>
        /// Verifies that Draw returns the top card (last element) and removes it from Content.
        /// Input conditions: Deck prepopulated with N cards (tested for multiple sizes).
        /// Expected result: Returned card is the last added card; Content count is decremented by one;
        /// remaining cards keep their original relative order.
        /// </summary>
        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        public void Draw_WithCards_ReturnsTopCardAndRemovesIt(int initialCount)
        {
            // Arrange
            var deck = new Deck(maxSize: 10, minSize: 0);
            var initialCards = new List<Card>();
            for (int i = 0; i < initialCount; i++)
            {
                var c = new Card { Name = $"Card{i}", id = i };
                initialCards.Add(c);
                deck.Content.Add(c);
            }

            int beforeCount = deck.Content.Count;
            Card expectedTop = initialCards[initialCount - 1];

            // Act
            Card returned = deck.Draw();

            // Assert
            // Returned card must be the top (last) card
            Assert.Same(expectedTop, returned);

            // Content count should decrease by one
            Assert.Equal(beforeCount - 1, deck.Content.Count);

            // Returned card should no longer be present in Content
            Assert.DoesNotContain(returned, deck.Content);

            // Remaining cards should preserve their original order (bottom..top-1)
            for (int i = 0; i < deck.Content.Count; i++)
            {
                Assert.Same(initialCards[i], deck.Content[i]);
            }
        }

        /// <summary>
        /// The test verifies that calling Seek on an empty deck throws InvalidOperationException with the expected message.
        /// Input conditions: Deck with no cards and a positive amount (1).
        /// Expected result: InvalidOperationException with message "The deck is empty.".
        /// </summary>
        [Fact]
        public void Seek_EmptyDeck_ThrowsInvalidOperationException()
        {
            // Arrange
            var deck = new Deck(maxSize: 10, minSize: 0);
            // Ensure Content is empty by default
            Assert.Equal(0, deck.Content.Count);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => deck.Seek(1));
            Assert.Equal("The deck is empty.", ex.Message);
        }

        /// <summary>
        /// The test verifies that Seek returns the correct top-N cards (top = last element) when amount is within [1, Count].
        /// Input conditions: Deck with three cards in known order and amounts 1 and 3.
        /// Expected result: Returned list contains the requested top cards in top-first order and original Content remains unchanged.
        /// </summary>
        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        public void Seek_AmountLessOrEqualCount_ReturnsTopNInTopFirstOrder(int amount)
        {
            // Arrange
            var deck = new Deck(maxSize: 10, minSize: 0);
            var c1 = new Card { Name = "Card1" };
            var c2 = new Card { Name = "Card2" };
            var c3 = new Card { Name = "Card3" };
            // Add in bottom-to-top order: c1 is bottom (index 0), c3 is top (last)
            deck.Content.AddRange(new[] { c1, c2, c3 });
            var originalNames = deck.Content.Select(c => c.Name).ToList();

            // Act
            List<Card> result = deck.Seek(amount);

            // Assert - result should be top-first
            var expected = new List<string>();
            if (amount >= 1)
            {
                // Determine expected top-first sequence for requested amount (cap by count)
                int take = Math.Min(amount, deck.Content.Count);
                for (int i = 0; i < take; i++)
                {
                    expected.Add(deck.Content[deck.Content.Count - 1 - i].Name);
                }
            }

            Assert.Equal(expected, result.Select(c => c.Name).ToList());

            // Content must remain unchanged
            Assert.Equal(originalNames, deck.Content.Select(c => c.Name).ToList());
            Assert.Equal(3, deck.Content.Count);
        }

        /// <summary>
        /// The test verifies that Seek with amount greater than the number of cards returns all cards in top-first order.
        /// Input conditions: Deck with three cards and amounts larger than count (4 and int.MaxValue).
        /// Expected result: Returned list equals all deck cards in reversed order (top-first) and original Content unchanged.
        /// </summary>
        [Theory]
        [InlineData(4)]
        [InlineData(int.MaxValue)]
        public void Seek_AmountGreaterThanCount_ReturnsAllCardsReversed(int amount)
        {
            // Arrange
            var deck = new Deck(maxSize: 10, minSize: 0);
            var c1 = new Card { Name = "Bottom" };
            var c2 = new Card { Name = "Middle" };
            var c3 = new Card { Name = "Top" };
            deck.Content.AddRange(new[] { c1, c2, c3 });
            var originalNames = deck.Content.Select(c => c.Name).ToList();

            // Act
            List<Card> result = deck.Seek(amount);

            // Assert - should return all cards reversed (top-first)
            var expected = new List<string> { "Top", "Middle", "Bottom" };
            Assert.Equal(expected, result.Select(c => c.Name).ToList());

            // Content must remain unchanged
            Assert.Equal(originalNames, deck.Content.Select(c => c.Name).ToList());
            Assert.Equal(3, deck.Content.Count);
        }

        /// <summary>
        /// The test verifies that Seek with amount zero returns an empty list and does not modify the deck.
        /// Input conditions: Deck with three cards and amount = 0.
        /// Expected result: Empty returned list and deck Content unchanged.
        /// </summary>
        [Fact]
        public void Seek_ZeroAmount_ReturnsEmptyList()
        {
            // Arrange
            var deck = new Deck(maxSize: 5, minSize: 0);
            deck.Content.AddRange(new[] {
                new Card { Name = "A" },
                new Card { Name = "B" },
                new Card { Name = "C" }
            });
            var originalCount = deck.Content.Count;

            // Act
            var result = deck.Seek(0);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            Assert.Equal(originalCount, deck.Content.Count);
        }

        /// <summary>
        /// The test verifies that negative amounts (including int.MinValue) cause an ArgumentOutOfRangeException from List.GetRange.
        /// Input conditions: Deck with three cards and negative amounts (-1 and int.MinValue).
        /// Expected result: ArgumentOutOfRangeException is thrown.
        /// </summary>
        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void Seek_NegativeAmount_ThrowsArgumentOutOfRangeException(int amount)
        {
            // Arrange
            var deck = new Deck(maxSize: 5, minSize: 0);
            deck.Content.AddRange(new[] {
                new Card { Name = "X" },
                new Card { Name = "Y" },
                new Card { Name = "Z" }
            });

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => deck.Seek(amount));
        }

        /// <summary>
        /// Verifies that calling PutOnTop on an empty deck throws InvalidOperationException with the expected message.
        /// Input conditions: Deck has no cards (Content.Count == 0). A non-null Card instance is provided.
        /// Expected result: InvalidOperationException is thrown with message "The deck is empty.".
        /// </summary>
        [Fact]
        public void PutOnTop_EmptyDeck_ThrowsInvalidOperationExceptionWithMessage()
        {
            // Arrange
            var deck = new Deck(maxSize: 10, minSize: 0);
            var card = new Card { Name = "TestCard" };

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => deck.PutOnTop(card));
            Assert.Equal("The deck is empty.", ex.Message);
        }

        /// <summary>
        /// Verifies that calling PutOnTop with a card that is not present in a non-empty deck throws InvalidOperationException with expected message.
        /// Input conditions: Deck contains at least one card (different instance), the provided card is not in the Content list.
        /// Expected result: InvalidOperationException is thrown with message "The card is not in the deck.".
        /// </summary>
        [Fact]
        public void PutOnTop_CardNotInDeck_ThrowsInvalidOperationExceptionWithMessage()
        {
            // Arrange
            var deck = new Deck(maxSize: 10, minSize: 0);
            var existing = new Card { Name = "Existing" };
            deck.Content.Add(existing);

            var other = new Card { Name = "OtherNotInDeck" };

            // Sanity check - ensure other is not the same reference and not contained
            Assert.False(ReferenceEquals(existing, other));
            Assert.DoesNotContain(other, deck.Content);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => deck.PutOnTop(other));
            Assert.Equal("The card is not in the deck.", ex.Message);
        }

        /// <summary>
        /// Verifies that PutOnTop moves the specified card instance to the top (end) of the deck content.
        /// Input conditions: Deck contains three distinct card instances; tested for a card initially in various positions.
        /// Expected result: After PutOnTop, the specified card is the last element in Content, Content.Count is unchanged,
        /// and the relative order of the other cards is preserved.
        /// </summary>
        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        public void PutOnTop_CardInDeck_MovesCardToTop(int initialIndex)
        {
            // Arrange
            var deck = new Deck(maxSize: 10, minSize: 0);

            var cardA = new Card { Name = "A" };
            var cardB = new Card { Name = "B" };
            var cardC = new Card { Name = "C" };

            var initialList = new List<Card> { cardA, cardB, cardC };

            // Select the instance by the provided initial index (0..2)
            var target = initialList[initialIndex];

            // Populate deck content with the same instances in order
            foreach (var c in initialList)
            {
                deck.Content.Add(c);
            }

            var originalCount = deck.Content.Count;

            // Act
            deck.PutOnTop(target);

            // Assert - count unchanged
            Assert.Equal(originalCount, deck.Content.Count);

            // The target must now be the last element
            Assert.Same(target, deck.Content[deck.Content.Count - 1]);

            // Build expected order: original list without target, then target at end
            var expected = new List<Card>();
            foreach (var c in initialList)
            {
                if (!ReferenceEquals(c, target))
                {
                    expected.Add(c);
                }
            }
            expected.Add(target);

            // Assert that the content order matches expected by reference equality
            Assert.Equal(expected.Count, deck.Content.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Same(expected[i], deck.Content[i]);
            }
        }

        /// <summary>
        /// Verifies that calling PutOnBottom on an empty deck throws InvalidOperationException.
        /// Input conditions: Deck with no content and a non-null Card instance passed as argument.
        /// Expected result: InvalidOperationException is thrown with message "The deck is empty.".
        /// </summary>
        [Fact]
        public void PutOnBottom_EmptyDeck_ThrowsInvalidOperationException()
        {
            // Arrange
            var deck = new Deck(maxSize: 10, minSize: 0);
            var card = new Card { Name = "Any" };

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => deck.PutOnBottom(card));

            // Assert
            Assert.Equal("The deck is empty.", ex.Message);
        }

        /// <summary>
        /// Verifies that calling PutOnBottom with a card that is not present in a non-empty deck throws InvalidOperationException.
        /// Input conditions: Deck containing one card (different from the provided one).
        /// Expected result: InvalidOperationException is thrown with message "The card is not in the deck.".
        /// </summary>
        [Fact]
        public void PutOnBottom_CardNotInDeck_ThrowsInvalidOperationException()
        {
            // Arrange
            var deck = new Deck(maxSize: 10, minSize: 0);
            deck.Content.Add(new Card { Name = "Existing" });
            var missingCard = new Card { Name = "Missing" };

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => deck.PutOnBottom(missingCard));

            // Assert
            Assert.Equal("The card is not in the deck.", ex.Message);
        }

        /// <summary>
        /// Verifies that PutOnBottom moves an existing card to the bottom (index 0) of the deck while preserving count.
        /// Input conditions: Deck with three distinct cards where the target card is in the middle.
        /// Expected result: The target card becomes the element at index 0, count remains unchanged, and other cards retain their relative order aside from the moved card.
        /// </summary>
        [Fact]
        public void PutOnBottom_CardInDeck_MovesCardToBottom()
        {
            // Arrange
            var deck = new Deck(maxSize: 10, minSize: 0);
            var cardA = new Card { Name = "A" };
            var cardB = new Card { Name = "B" };
            var cardC = new Card { Name = "C" };

            deck.Content.Add(cardA); // index 0
            deck.Content.Add(cardB); // index 1 (target)
            deck.Content.Add(cardC); // index 2

            // Act
            deck.PutOnBottom(cardB);

            // Assert
            // The moved card must now be at index 0 (bottom)
            Assert.Same(cardB, deck.Content[0]);

            // The deck size must remain the same
            Assert.Equal(3, deck.Content.Count);

            // Other cards should remain present; their order relative to each other should be preserved except for the moved card
            Assert.Same(cardA, deck.Content[1]);
            Assert.Same(cardC, deck.Content[2]);
        }

        /// <summary>
        /// Verifies that the Deck constructor assigns the provided maxSize and minSize values
        /// and that the Owners and Content collections are initialized to non-null, empty lists.
        /// Input conditions: various edge integer pairs for (maxSize, minSize) including extremes and negative values.
        /// Expected result: Properties maxSize and minSize equal inputs; Owners and Content are non-null and empty.
        /// </summary>
        [Theory]
        [InlineData(int.MaxValue, int.MinValue)]
        [InlineData(int.MinValue, int.MaxValue)]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(-1, -5)]
        public void Deck_Ctor_VariousSizes_AssignsProperties(int maxSize, int minSize)
        {
            // Arrange
            // (no additional setup required)

            // Act
            var deck = new Deck(maxSize, minSize);

            // Assert
            // Verify numeric properties are assigned exactly
            Assert.Equal(maxSize, deck.maxSize);
            Assert.Equal(minSize, deck.minSize);

            // Collections should be initialized and empty by default
            Assert.NotNull(deck.Content);
            Assert.Empty(deck.Content);

            Assert.NotNull(deck.Owners);
            Assert.Empty(deck.Owners);
        }

        /// <summary>
        /// Verifies that RemoveCard removes exactly one occurrence of the provided card instance when it is present.
        /// Input conditions: deck contains the same Card instance 'duplicateCount' times (1 or 2).
        /// Expected result: after calling RemoveCard once the count decreases by one and the deck no longer contains
        /// the removed occurrence (for duplicateCount==1 the card is absent; for 2 one occurrence remains).
        /// </summary>
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void RemoveCard_CardPresent_RemovesSingleOccurrence(int duplicateCount)
        {
            // Arrange
            var deck = new Deck(maxSize: 10, minSize: 0);
            var card = new Card { Name = "TestCard" };

            for (int i = 0; i < duplicateCount; i++)
            {
                // Add same instance multiple times to simulate duplicates in the list
                deck.Content.Add(card);
            }

            int initialCount = deck.Content.Count;

            // Act
            deck.RemoveCard(card);

            // Assert
            Assert.Equal(initialCount - 1, deck.Content.Count);
            // If there was only one occurrence it should be absent; if there were duplicates at least one remains.
            if (duplicateCount == 1)
            {
                Assert.DoesNotContain(card, deck.Content);
            }
            else
            {
                Assert.Contains(card, deck.Content);
            }
        }

        /// <summary>
        /// Verifies that RemoveCard throws InvalidOperationException when the card is not present in the deck.
        /// Input conditions: deck contains other cards but not the target card instance.
        /// Expected result: InvalidOperationException is thrown with message "The card is not in the deck.".
        /// </summary>
        [Fact]
        public void RemoveCard_CardNotPresent_ThrowsInvalidOperationException()
        {
            // Arrange
            var deck = new Deck(maxSize: 5, minSize: 0);
            var existing = new Card { Name = "Existing" };
            var missing = new Card { Name = "Missing" };

            deck.Content.Add(existing);

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => deck.RemoveCard(missing));

            // Assert
            Assert.Equal("The card is not in the deck.", ex.Message);
            // Ensure original content remains unchanged
            Assert.Single(deck.Content);
            Assert.Contains(existing, deck.Content);
        }
    }
}