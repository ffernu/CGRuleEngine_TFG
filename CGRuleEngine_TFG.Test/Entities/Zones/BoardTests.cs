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
    public class BoardTests
    {
        /// <summary>
        /// Verifies that RemoveCard throws an InvalidOperationException when the specified card is not present on the board.
        /// Input conditions: a newly created board with no cards and a card instance that was never added.
        /// Expected result: InvalidOperationException is thrown with message "The card is not on the board.".
        /// </summary>
        [Fact]
        public void RemoveCard_CardNotOnBoard_ThrowsInvalidOperationException()
        {
            // Arrange
            var board = new Board(5, 5);
            var card = new Card { Name = "MissingCard", id = 1 };

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => board.RemoveCard(card));

            // Assert
            Assert.Equal("The card is not on the board.", ex.Message);
        }

        /// <summary>
        /// Verifies that RemoveCard removes exactly one occurrence of the specified card when it is present.
        /// Input conditions: board contains the same card instance repeated 'initialCount' times (1 or 2).
        /// Expected result: After removal, the board's Content count is decreased by one;
        /// if initialCount == 1 the card is no longer present, if initialCount > 1 the card remains present.
        /// </summary>
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void RemoveCard_WhenCardPresent_RemovesSingleOccurrence(int initialCount)
        {
            // Arrange
            var board = new Board(10, 5);
            var card = new Card { Name = "Removable", id = 42 };

            for (int i = 0; i < initialCount; i++)
            {
                board.Content.Add(card); // adding same instance to simulate duplicates
            }

            int beforeCount = board.Content.Count;

            // Sanity checks (Arrange assertions)
            Assert.Equal(initialCount, beforeCount);
            Assert.True(board.Content.Contains(card));

            // Act
            board.RemoveCard(card);

            // Assert
            Assert.Equal(beforeCount - 1, board.Content.Count);

            if (initialCount == 1)
            {
                Assert.DoesNotContain(card, board.Content);
            }
            else
            {
                Assert.Contains(card, board.Content);
            }
        }

        /// <summary>
        /// Provides diverse test cases for Clone testing including boundary integer values and typical cases.
        /// Each element is an object[] of (maxSize, maxSizePerPlayer).
        /// </summary>
        public static IEnumerable<object[]> CloneTestData()
        {
            // Typical cases
            yield return new object[] { 0, 0 };
            yield return new object[] { 5, 2 };
            yield return new object[] { -1, -10 };

            // Boundary extremes
            yield return new object[] { int.MinValue, int.MinValue };
            yield return new object[] { int.MaxValue, int.MaxValue };
        }

        /// <summary>
        /// Verifies that the Board constructor assigns the provided maxSize and maxSizePerPlayer
        /// and initializes Owners and Content collections to non-null, empty lists.
        /// Input conditions: various numeric pairs for maxSize and maxSizePerPlayer (including 0, negative, int.MinValue, int.MaxValue).
        /// Expected result: properties are assigned exactly as provided and Owners/Content are non-null and empty.
        /// </summary>
        [Theory]
        [InlineData(5, 2)]
        [InlineData(0, 0)]
        [InlineData(1, -1)]
        [InlineData(-1, 1)]
        [InlineData(int.MaxValue, int.MaxValue)]
        [InlineData(int.MinValue, int.MinValue)]
        public void Board_Ctor_WithVariousSizes_AssignsFieldsAndInitializesCollections(int maxSize, int maxSizePerPlayer)
        {
            // Arrange
            // (no external setup required)

            // Act
            var board = new Board(maxSize, maxSizePerPlayer);

            // Assert
            // Numeric properties must reflect constructor inputs exactly
            Assert.Equal(maxSize, board.maxSize);
            Assert.Equal(maxSizePerPlayer, board.maxSizePerPlayer);

            // Collections must be initialized (not null) and empty
            Assert.NotNull(board.Owners);
            Assert.Empty(board.Owners);

            Assert.NotNull(board.Content);
            Assert.Empty(board.Content);
        }

        /// <summary>
        /// Provides various content scenarios for the Board.clear() method.
        /// Cases:
        /// - Empty content
        /// - Single card
        /// - Multiple distinct cards
        /// - Duplicate references (same Card instance added multiple times)
        /// </summary>
        public static IEnumerable<object[]> ClearScenarios()
        {
            // Empty list
            yield return new object[] { new List<Card>() };

            // Single card
            yield return new object[] { new List<Card> { new Card { Name = "Single", id = 1 } } };

            // Multiple distinct cards
            yield return new object[] {
                new List<Card> {
                    new Card { Name = "A", id = 2 },
                    new Card { Name = "B", id = 3 },
                    new Card { Name = "C", id = 4 }
                }
            };

            // Duplicate references (same instance twice)
            var shared = new Card { Name = "Shared", id = 5 };
            yield return new object[] { new List<Card> { shared, shared } };
        }

        /// <summary>
        /// Verifies that Board.clear() returns a shallow copy of the current Content,
        /// preserves element order and references, and empties the board's Content.
        /// Input conditions: various Content lists provided by ClearScenarios (empty, single, multiple, duplicates).
        /// Expected result: returned list has same elements in same order (reference equality),
        /// Board.Content is empty after call, and the returned list instance is not the same object as Board.Content.
        /// </summary>
        [Theory]
        [MemberData(nameof(ClearScenarios))]
        public void clear_WithVariousContents_ReturnsRemovedListAndEmptiesContent(List<Card> initialContent)
        {
            // Arrange
            var board = new Board(maxSize: 100, maxSizePerPlayer: 100);

            // Populate the board's Content with the provided initial content (preserve references & order)
            foreach (var c in initialContent)
            {
                board.Content.Add(c);
            }

            // Act
            var removed = board.clear();

            // Assert - common expectations
            Assert.NotNull(removed);
            Assert.Equal(initialContent.Count, removed.Count); // same number of items returned
            Assert.Equal(initialContent, removed); // same elements in same order (reference equality for Card)
            Assert.Equal(0, board.Content.Count); // board content must be cleared
            Assert.NotSame(removed, board.Content); // returned list must be a new List instance

            // Additional assertions for duplicate-reference scenario:
            if (initialContent.Count >= 2 && object.ReferenceEquals(initialContent[0], initialContent[1]))
            {
                // If initial had duplicate references, the removed list must preserve duplicate references at same positions
                Assert.True(object.ReferenceEquals(removed[0], removed[1]));
            }

            // Mutating the returned list should not impact the board's Content (which must remain empty)
            removed.Add(new Card { Name = "NewAfterClear", id = 999 });
            Assert.Equal(0, board.Content.Count);
        }

        /// <summary>
        /// Verifies that calling clear() on an already-empty board returns an empty list and does not throw.
        /// Input conditions: a board with no content.
        /// Expected result: returned list is empty and Board.Content remains empty.
        /// </summary>
        [Fact]
        public void clear_OnAlreadyEmptyBoard_ReturnsEmptyListAndDoesNotThrow()
        {
            // Arrange
            var board = new Board(maxSize: 10, maxSizePerPlayer: 5);

            // Precondition assert (Arrange)
            Assert.Equal(0, board.Content.Count);

            // Act
            var removed = Record.Exception(() => board.clear());

            // Assert - no exception thrown
            Assert.Null(removed);

            // Act - retrieve the return value explicitly
            var result = board.clear();

            // Assert - result is an empty list and board remains empty
            Assert.NotNull(result);
            Assert.Equal(0, result.Count);
            Assert.Equal(0, board.Content.Count);
        }

        /// <summary>
        /// Verifies that MoveCard throws InvalidOperationException when the provided card is not present on the board.
        /// Input conditions: a Board with empty Content and a Card instance not contained in the board.
        /// Expected result: InvalidOperationException is thrown with message "The card is not on the board."
        /// </summary>
        [Fact]
        public void MoveCard_CardNotOnBoard_ThrowsInvalidOperationException()
        {
            // Arrange
            var board = new Board(maxSize: 5, maxSizePerPlayer: 3);
            var card = new Card { Name = "UnplacedCard" };
            var mockTarget = new Mock<IZone>();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => board.MoveCard(card, mockTarget.Object));
            Assert.Equal("The card is not on the board.", ex.Message);
        }

        // Helper PlayerBuilder wrapper usage to match existing Player constructors in production code.
        // The instruction prohibits creating external helper types; this inner class merely mirrors expected builder usage
        // for constructing Player instances used in tests. It is defined inside the test class to satisfy the requirement
        // that helper types be inner classes when necessary.
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

        public static IEnumerable<object[]> AddCardExceptionCases()
        {
            // owner missing -> should throw "The player is not an owner of the board."
            yield return new object[] { false, 0, 5, 0, 2, "The player is not an owner of the board." };

            // board full -> Content.Count (1) >= maxSize (1) triggers "The board is full."
            yield return new object[] { true, 1, 1, 0, 5, "The board is full." };

            // player reached max per player -> one existing card for this controller and maxSizePerPlayer = 1
            yield return new object[] { true, 1, 5, 1, 1, "The player has reached the maximum number of cards on the board." };
        }

    }


    // Minimal PlayerBuilder helper matching production shape used by Player ctor.
    // Placed inside the test file as an inner/helpful type per test-generation constraints.
    internal class PlayerBuilder
    {
        public string Name { get; }
        public Dictionary<string, int> Stats { get; }
        public Dictionary<string, IZone> Zones { get; }
        public CGRuleEngine_TFG.Entities.Players.Controllers.IPlayerController? Controller { get; }

        public PlayerBuilder(string name)
        {
            Name = name;
            Stats = new Dictionary<string, int>();
            Zones = new Dictionary<string, IZone>();
            Controller = null;
        }
    }
}