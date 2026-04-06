using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable
using CGRuleEngine_TFG.Entities;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Players.Controllers;
using CGRuleEngine_TFG.Entities.Zones;
using Moq;
using Xunit;

namespace CGRuleEngine_TFG.Entities.Players.UnitTests
{
    public class PlayerBuilderTests
    {
        /// <summary>
        /// Verifies that the constructor PlayerBuilder(string name) assigns the provided name
        /// and that the object-level collections and controller default fields are initialized as expected.
        /// Input conditions: various name values (normal, empty, whitespace-only, long, special characters).
        /// Expected result: Name property equals the provided input; Stats and Zones are non-null and empty; Controller is null.
        /// </summary>
        [Theory]
        [MemberData(nameof(NameTestData))]
        public void PlayerBuilder_Ctor_WithName_AssignsNameAndInitializesDefaults(string name)
        {
            // Arrange
            // (no additional setup required)

            // Act
            PlayerBuilder builder = new PlayerBuilder(name);

            // Assert
            // Name must be exactly the provided string
            Assert.Equal(name, builder.Name);

            // Stats and Zones are initialized to non-null dictionaries and are empty by default
            Assert.NotNull(builder.Stats);
            Assert.Equal(0, builder.Stats.Count);

            Assert.NotNull(builder.Zones);
            Assert.Equal(0, builder.Zones.Count);

            // Controller should be null by default (not set by this ctor)
            Assert.Null(builder.Controller);
        }

        /// <summary>
        /// Provides a variety of name inputs to exercise edge cases:
        /// - typical name
        /// - empty string
        /// - whitespace-only string
        /// - very long string (10k chars)
        /// - special/control characters (newline, tab, unicode)
        /// </summary>
        public static IEnumerable<object[]> NameTestData()
        {
            yield return new object[] { "Fran" };
            yield return new object[] { string.Empty };
            yield return new object[] { "   " };
            yield return new object[] { new string('a', 10_000) };
            yield return new object[] { "Name\n\t\u2603" };
        }

        /// <summary>
        /// The test verifies that SetController assigns the provided controller instance to the Builder.Controller
        /// property and returns the same builder instance to allow method chaining.
        /// Input conditions: a non-null mock IPlayerController instance (multiple instances tested via MemberData).
        /// Expected result: Builder.Controller references the provided controller and the return value is the same builder.
        /// </summary>
        [Theory]
        [MemberData(nameof(ControllerInstances))]
        public void SetController_WithVariousControllers_SetsControllerReferenceAndReturnsBuilder(IPlayerController controller)
        {
            // Arrange
            var builder = new PlayerBuilder("TestPlayer");

            // Act
            var returned = builder.SetController(controller);

            // Assert
            Assert.Same(controller, builder.Controller);
            Assert.Same(builder, returned);
        }

        /// <summary>
        /// The test verifies that calling SetController a second time overwrites the previous controller reference.
        /// Input conditions: two distinct mock IPlayerController instances.
        /// Expected result: Builder.Controller references the second controller after the second call and returned builder is same instance.
        /// </summary>
        [Fact]
        public void SetController_OverwritesExistingController_WithNewInstance()
        {
            // Arrange
            var builder = new PlayerBuilder();
            var firstMock = new Mock<IPlayerController>();
            var secondMock = new Mock<IPlayerController>();

            // Act 1
            var firstReturn = builder.SetController(firstMock.Object);

            // Assert - after first set
            Assert.Same(firstMock.Object, builder.Controller);
            Assert.Same(builder, firstReturn);

            // Act 2
            var secondReturn = builder.SetController(secondMock.Object);

            // Assert - after overwrite
            Assert.Same(secondMock.Object, builder.Controller);
            Assert.Same(builder, secondReturn);
        }

        /// <summary>
        /// Provides different controller instances for parameterized tests.
        /// Uses Moq to create distinct IPlayerController instances.
        /// </summary>
        public static IEnumerable<object[]> ControllerInstances()
        {
            yield return new object[] { new Mock<IPlayerController>().Object };
            yield return new object[] { new Mock<IPlayerController>().Object };
        }

        /// <summary>
        /// Verifies the parameterless PlayerBuilder constructor leaves reference properties in their expected runtime defaults.
        /// Input conditions: creating a new PlayerBuilder with the default constructor.
        /// Expected result: Name is null (no initialization in constructor), Stats and Zones are non-null and empty, Controller is null.
        /// This test exposes a mismatch between the XML comment in source (which claims Name is set to empty string) and actual implementation.
        /// </summary>
        [Fact]
        public void Ctor_DefaultConstructor_InitializesCollectionsAndLeavesNameAndControllerNull()
        {
            // Arrange & Act
            PlayerBuilder builder = new PlayerBuilder();

            // Assert
            // Name: source's parameterless ctor does not assign Name, so runtime default is null.
            Assert.Null(builder.Name);
            // Stats and Zones use property initializers and must be non-null and empty.
            Assert.NotNull(builder.Stats);
            Assert.Empty(builder.Stats);
            Assert.NotNull(builder.Zones);
            Assert.Empty(builder.Zones);
            // Controller is not initialized by the default ctor and should be null.
            Assert.Null(builder.Controller);
        }

        /// <summary>
        /// Provides test cases combining representative string edge cases and numeric extremes for WithStat.
        /// Each entry: (name, value)
        /// </summary>
        public static IEnumerable<object[]> StatTestData()
        {
            // Empty string with zero
            yield return new object[] { string.Empty, 0 };

            // Whitespace-only string with a negative value
            yield return new object[] { "   ", -1 };

            // Typical stat name with positive small value
            yield return new object[] { "Health", 1 };

            // Special/control characters with int.MinValue
            yield return new object[] { "\n\t!@#", int.MinValue };

            // Very long string key with int.MaxValue
            yield return new object[] { new string('a', 1000), int.MaxValue };
        }

        /// <summary>
        /// Verifies that WithStat sets or updates the Stats dictionary entry for a variety of keys and values,
        /// and that it returns the same builder instance to allow chaining.
        /// Inputs tested:
        /// - name: empty, whitespace, normal, special chars, very long string (non-nullable)
        /// - value: 0, negative, small positive, int.MinValue, int.MaxValue
        /// Expected:
        /// - The Stats dictionary contains the provided key and exact value after calling WithStat.
        /// - The returned PlayerBuilder reference is the same instance used to call the method.
        /// </summary>
        [Theory]
        [MemberData(nameof(StatTestData))]
        public void WithStat_VariousKeysAndValues_AddsOrUpdatesAndReturnsBuilder(string name, int value)
        {
            // Arrange
            var builder = new PlayerBuilder();

            // Act
            var returned = builder.WithStat(name, value);

            // Assert
            // Returned instance should be the same (for chaining)
            Assert.Same(builder, returned);

            // Stats dictionary should contain the key with the exact value
            Assert.True(builder.Stats.ContainsKey(name), "Stats dictionary should contain the provided key.");
            Assert.Equal(value, builder.Stats[name]);
        }

        /// <summary>
        /// Verifies that calling WithStat twice with the same key overwrites the previous value.
        /// Inputs:
        /// - name: "Health"
        /// - firstValue: 10
        /// - secondValue: 20
        /// Expected:
        /// - After two calls, the Stats entry for "Health" equals the second value.
        /// - Dictionary contains only the key once with the updated value.
        /// </summary>
        [Fact]
        public void WithStat_SameKeyTwice_OverwritesPreviousValue()
        {
            // Arrange
            var builder = new PlayerBuilder();

            // Act
            builder.WithStat("Health", 10);
            builder.WithStat("Health", 20);

            // Assert
            Assert.True(builder.Stats.ContainsKey("Health"));
            Assert.Equal(20, builder.Stats["Health"]);
        }

        /// <summary>
        /// Verifies that Build() constructs a Player whose Name, Stats and Zones come from the builder instance,
        /// and that successive builds produce incrementing Id values.
        /// Input conditions:
        /// - Builder may have Name set to various values (including null).
        /// - Builder has explicit Stats and Zones dictionaries and a mocked controller.
        /// Expected:
        /// - The resulting Player's Name equals the builder's Name.
        /// - The Player.Stats and Player.Zones are the exact same instances provided on the builder (reference equality).
        /// - Building two players in sequence yields ids where second == first + 1.
        /// </summary>
        [Theory]
        [MemberData(nameof(NameVariants))]
        public void Build_AssignsBuilderPropertiesAndIds_PropertiesCopiedAndIdsIncrement(string? name)
        {
            // Arrange
            var builder = new PlayerBuilder();
            if (name is not null)
            {
                builder.Name = name;
            }
            // Prepare explicit dictionaries to ensure reference-copy behavior
            var stats = new Dictionary<string, int> { { "Health", 20 }, { "Level", 1 } };
            var zoneMock = new Mock<IZone>(MockBehavior.Strict);
            var zones = new Dictionary<string, IZone> { { "Hand", zoneMock.Object } };

            builder.Stats = stats;
            builder.Zones = zones;

            var controllerMock = new Mock<IPlayerController>(MockBehavior.Strict);
            builder.Controller = controllerMock.Object;

            var otherBuilder = new PlayerBuilder("other")
            {
                Stats = new Dictionary<string, int>(),
                Zones = new Dictionary<string, IZone>()
            };

            // Act
            Player playerA = builder.Build();
            Player playerB = otherBuilder.Build();

            // Assert
            // Name equality (covers null, empty, whitespace, long, special chars)
            Assert.Equal(name, playerA.Name);

            // Stats and Zones should be the same instances provided to the builder
            Assert.Same(stats, playerA.Stats);
            Assert.Same(zones, playerA.Zones);

            // Ids increment by one between two consecutive builds performed here
            Assert.Equal(playerA.Id + 1, playerB.Id);
        }

        /// <summary>
        /// Ensures that when a builder is created without modifying Stats/Zones, Build()
        /// results in a Player that has non-null, empty Stats and Zones dictionaries and that
        /// the Player uses the same instances as the builder.
        /// Input conditions:
        /// - PlayerBuilder created with default constructor (no modifications).
        /// Expected:
        /// - Player.Stats and Player.Zones are non-null and empty.
        /// - Player.Stats and Player.Zones are the exact same instances as builder's properties.
        /// </summary>
        [Fact]
        public void Build_WithoutSettingOptionalCollections_UsesDefaultInstances()
        {
            // Arrange
            var builder = new PlayerBuilder("defaultName");

            // Act
            Player player = builder.Build();

            // Assert
            Assert.Equal("defaultName", player.Name);

            // The default dictionaries should be present and empty
            Assert.NotNull(player.Stats);
            Assert.NotNull(player.Zones);
            Assert.Empty(player.Stats);
            Assert.Empty(player.Zones);

            // They should be the exact same instances as on the builder
            Assert.Same(builder.Stats, player.Stats);
            Assert.Same(builder.Zones, player.Zones);
        }

        // MemberData for name variants to exercise edge cases for string parameter handling.
        public static IEnumerable<object?[]> NameVariants()
        {
            // null (builder left without explicit Name)
            yield return new object?[] { null };

            // empty string
            yield return new object?[] { string.Empty };

            // whitespace-only
            yield return new object?[] { "   " };

            // normal name
            yield return new object?[] { "Fran" };

            // special characters and control characters
            yield return new object?[] { "NameWith\nTab\tAnd\u0000NullChar" };

            // long string (reasonable length to detect issues)
            yield return new object?[] { new string('x', 1024) };
        }

        /// <summary>
        /// Provides a variety of string inputs and constructor usage flags to exercise SetName behavior.
        /// Each item is: (nameToSet, useCtorWithInitialName)
        /// </summary>
        public static IEnumerable<object[]> SetNameTestCases()
        {
            // Empty string
            yield return new object[] { string.Empty, false };
            yield return new object[] { string.Empty, true };

            // Whitespace-only
            yield return new object[] { "   ", false };
            yield return new object[] { "\t\n", true };

            // Normal typical name
            yield return new object[] { "PlayerOne", false };
            yield return new object[] { "PlayerTwo", true };

            // Very long string (boundary stress)
            yield return new object[] { new string('a', 10000), false };
            yield return new object[] { new string('b', 10000), true };

            // Special/control and unicode characters
            yield return new object[] { "NameWith\nNewLine\u0001\u0000", false };
            yield return new object[] { "Emoji🚀Name", true };
        }

        /// <summary>
        /// Verifies that SetName sets the Name property to the provided value and returns the same builder instance.
        /// Test inputs include empty, whitespace, long, special/control, and unicode-containing strings.
        /// Also verifies behavior when the builder was constructed with or without an initial name.
        /// Expected: Name property equals the provided input and the returned object is the same instance.
        /// </summary>
        /// <param name="name">The name to set; must be non-null per source annotations.</param>
        /// <param name="useCtorWithInitialName">If true, constructs the builder with an initial name to ensure replacement works.</param>
        [Theory]
        [MemberData(nameof(SetNameTestCases))]
        public void SetName_VariousInputs_SetsNameAndReturnsSameBuilder(string name, bool useCtorWithInitialName)
        {
            // Arrange
            PlayerBuilder builder = useCtorWithInitialName
                ? new PlayerBuilder("InitialName")
                : new PlayerBuilder();

            // Capture reference for identity check
            PlayerBuilder beforeReference = builder;

            // Act
            PlayerBuilder result = builder.SetName(name);

            // Assert
            // Returned object must be the same instance (allows chaining)
            Assert.Same(beforeReference, result);

            // Name property must equal the provided input
            Assert.Equal(name, builder.Name);
        }

        /// <summary>
        /// Ensures that calling SetName consecutively changes the Name each time and still returns the same instance.
        /// Input: two different non-null names.
        /// Expected: Name equals last set value and the same instance is returned each call.
        /// </summary>
        [Fact]
        public void SetName_ConsecutiveCalls_UpdatesNameEachTimeAndReturnsSameInstance()
        {
            // Arrange
            var builder = new PlayerBuilder("Start");
            string first = "FirstName";
            string second = "SecondName";

            // Act
            var firstResult = builder.SetName(first);
            var secondResult = builder.SetName(second);

            // Assert
            Assert.Same(builder, firstResult);
            Assert.Same(builder, secondResult);
            Assert.Equal(second, builder.Name);
        }

        /// <summary>
        /// Verifies that WithZone sets the provided zone instance into the Zones dictionary for various key forms
        /// and that the method returns the same builder instance to allow chaining.
        /// Inputs tested: normal name, empty string, whitespace-only string, very long string, and name with special characters.
        /// Expected: Zones contains the provided key and the exact zone instance; return value is the original builder.
        /// </summary>
        [Theory]
        [InlineData("Hand")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("VeryLongName_XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX")]
        [InlineData("Name-With-!@#$%^&*()")]
        public void WithZone_VariousNames_SetsZoneAndReturnsBuilder(string key)
        {
            // Arrange
            var builder = new PlayerBuilder();
            var zoneMock = new Mock<IZone>(MockBehavior.Strict);
            IZone zone = zoneMock.Object;

            // Act
            var returned = builder.WithZone(key, zone);

            // Assert
            Assert.Same(builder, returned);
            Assert.True(builder.Zones.ContainsKey(key));
            Assert.Same(zone, builder.Zones[key]);
        }

        /// <summary>
        /// Verifies that calling WithZone twice with the same key overwrites the previous entry.
        /// Input: same key with two different IZone instances.
        /// Expected: Zones contains only one entry for the key and the stored value is the last provided instance.
        /// </summary>
        [Fact]
        public void WithZone_SameKeyTwice_OverwritesPreviousZone()
        {
            // Arrange
            var builder = new PlayerBuilder();
            var firstMock = new Mock<IZone>(MockBehavior.Strict);
            var secondMock = new Mock<IZone>(MockBehavior.Strict);

            IZone first = firstMock.Object;
            IZone second = secondMock.Object;
            string key = "Deck";

            // Act
            builder.WithZone(key, first);
            builder.WithZone(key, second);

            // Assert
            Assert.True(builder.Zones.ContainsKey(key));
            Assert.Equal(1, builder.Zones.Count);
            Assert.Same(second, builder.Zones[key]);
        }
    }
}