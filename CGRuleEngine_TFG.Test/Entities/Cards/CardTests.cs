using System;
using System.Collections;
using System.Collections.Generic;

using CGRuleEngine_TFG;
using CGRuleEngine_TFG.Entities;
using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Execution;
using Moq;
using Xunit;

namespace CGRuleEngine_TFG.Entities.Cards.UnitTests
{
    public class CardTests
    {
        /// <summary>
        /// Verifies that calling Flip() toggles the isFaceUp property.
        /// Input conditions: initial isFaceUp set to provided boolean values (true and false).
        /// Expected result: After calling Flip once, isFaceUp equals the logical negation of the initial value.
        /// </summary>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Flip_WhenCalled_TogglesIsFaceUp(bool initialIsFaceUp)
        {
            // Arrange
            var card = new Card
            {
                isFaceUp = initialIsFaceUp
            };

            // Act
            card.Flip();

            // Assert
            Assert.Equal(!initialIsFaceUp, card.isFaceUp);
        }

        /// <summary>
        /// Verifies that calling Flip() twice returns isFaceUp to its original state.
        /// Input conditions: initial isFaceUp set to provided boolean values (true and false).
        /// Expected result: After calling Flip twice, isFaceUp equals the original initial value.
        /// </summary>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Flip_WhenCalledTwice_ReturnsToOriginalState(bool initialIsFaceUp)
        {
            // Arrange
            var card = new Card
            {
                isFaceUp = initialIsFaceUp
            };

            // Act
            card.Flip();
            card.Flip();

            // Assert
            Assert.Equal(initialIsFaceUp, card.isFaceUp);
        }

        /// <summary>
        /// Verifies that calling Untap sets isTapped to false regardless of the initial tapped state.
        /// Input conditions: initial tapped state (true or false).
        /// Expected result: After calling Untap, isTapped is false and no exception is thrown.
        /// </summary>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Untap_InitialTappedState_ResultIsNotTapped(bool initialTapped)
        {
            // Arrange
            var card = new Card
            {
                // set initial state according to the test case
                isTapped = initialTapped
            };

            // Act
            card.Untap();

            // Assert
            Assert.False(card.isTapped);
        }

        /// <summary>
        /// Verifies that calling Untap multiple times is idempotent (remains not tapped).
        /// Input conditions: card initially tapped (true), call Untap twice.
        /// Expected result: After repeated calls, isTapped remains false and no exception is thrown.
        /// </summary>
        [Fact]
        public void Untap_CalledMultipleTimes_RemainsNotTapped()
        {
            // Arrange
            var card = new Card
            {
                isTapped = true
            };

            // Act & Assert
            // first call should untap
            card.Untap();
            Assert.False(card.isTapped);

            // second call should keep it untapped and not throw
            card.Untap();
            Assert.False(card.isTapped);
        }

        /// <summary>
        /// Verifies that calling Tap() sets isTapped to true regardless of the initial isTapped state.
        /// Input conditions: initial isTapped set to provided boolean values (true and false).
        /// Expected result: After calling Tap once, isTapped is true and no exception is thrown.
        /// </summary>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Tap_WhenCalled_SetsIsTappedTrue(bool initialTapped)
        {
            // Arrange
            var card = new Card
            {
                isTapped = initialTapped
            };

            // Act
            card.Tap();

            // Assert
            Assert.True(card.isTapped);
        }

        /// <summary>
        /// Verifies that calling Tap() multiple times remains in the tapped state (idempotent).
        /// Input conditions: card initially set to tapped (true) and call Tap twice.
        /// Expected result: After repeated calls, isTapped remains true and no exception is thrown.
        /// </summary>
        [Fact]
        public void Tap_CalledMultipleTimes_RemainsTapped()
        {
            // Arrange
            var card = new Card
            {
                isTapped = true
            };

            // Act
            card.Tap();
            card.Tap();

            // Assert
            Assert.True(card.isTapped);
        }
    }
}