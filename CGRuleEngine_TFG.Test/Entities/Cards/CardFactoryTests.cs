using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Moq;
using Newtonsoft.Json;
using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Cards.StandardCards;
using CGRuleEngine_TFG.Entities.Cards.DataBase;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Zones;
using CGRuleEngine_TFG.Execution.Effects;

/*
 * ANSWER TO YOUR QUESTIONS:
 * 1. Yes, it is completely possible to test this correctly.
 * 2. The solution is to avoid using Moq for CardDatabase. Moq cannot mock classes without a parameterless
 * constructor, and it cannot intercept non-virtual methods (like GetTemplate).
 * 3. Instead of changing your CardDatabase or CardFactory code to use an interface (which was not requested),
 * we will use a REAL CardDatabase instance in these tests. We achieve this by creating a temporary folder 
 * with JSON files during the test run, exactly like we did in CardDatabaseTests. 
 * * Below is the corrected CardFactoryTests class.
 */

namespace CGRuleEngine_TFG.Tests.Entities.Cards
{
    public class CardFactoryTests : IDisposable
    {
        private readonly string _tempDirectory;

        public CardFactoryTests()
        {
            // Arrange - Create a temporary directory for the real CardDatabase to read from
            _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);
        }

        public void Dispose()
        {
            // Clean up the temporary directory after tests finish
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }

        // Helper to generate a real JSON file so the real CardDatabase can load it
        private void CreateTemplateJsonFile(string templateId, string typeCard, string name)
        {
            var template = new
            {
                Id = templateId,
                Type = typeCard,
                Name = name,
                SideA = "Front",
                SideB = "Back",
                Tags = new List<string>(),
                Properties = new Dictionary<string, object>()
            };

            string filePath = Path.Combine(_tempDirectory, $"{templateId}.json");
            File.WriteAllText(filePath, JsonConvert.SerializeObject(template));
        }

        [Fact]
        public void CreateCard_DefaultNormalCardRegistered_ReturnsNormalCardInstance()
        {
            // Arrange
            CreateTemplateJsonFile("Tpl_001", "NormalCard", "Basic Goblin");
            var database = new CardDatabase(_tempDirectory);
            var factory = new CardFactory(database, null);
            var owner = new PlayerBuilder("Test Owner").Build();

            // Act
            var result = factory.CreateCard("Tpl_001", owner, null);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NormalCard>(result);
            Assert.Same(owner, result.Owner);
            Assert.Equal("Basic Goblin", result.Name);
        }

        [Fact]
        public void CreateCard_UnregisteredType_ThrowsException()
        {
            // Arrange
            CreateTemplateJsonFile("Tpl_Unknown", "UnknownCardType", "Glitch Card");
            var database = new CardDatabase(_tempDirectory);
            var factory = new CardFactory(database, null);
            var owner = new PlayerBuilder("Test Owner").Build();

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => factory.CreateCard("Tpl_Unknown", owner, null));
            Assert.Equal("No builder registered for card type: UnknownCardType", exception.Message);
        }

        [Fact]
        public void RegisterBuilder_CustomType_SuccessfullyBuildsCustomType()
        {
            // Arrange
            CreateTemplateJsonFile("Tpl_Custom", "CustomCard", "Test Card");
            var database = new CardDatabase(_tempDirectory);
            var factory = new CardFactory(database, null);
            var owner = new PlayerBuilder("Test Owner").Build();

            var expectedCardMock = new Mock<ICard>();

            // Act
            factory.RegisterBuilder("CustomCard", (t, o, id, ef) => expectedCardMock.Object);
            var result = factory.CreateCard("Tpl_Custom", owner, null);

            // Assert
            Assert.Same(expectedCardMock.Object, result);
        }

        [Fact]
        public void RegisterBuilder_OverwriteExistingBuilder_UsesNewBuilder()
        {
            // Arrange
            CreateTemplateJsonFile("Tpl_001", "NormalCard", "Test Card");
            var database = new CardDatabase(_tempDirectory);
            var factory = new CardFactory(database, null);
            var owner = new PlayerBuilder("Test Owner").Build();

            var customMockCard = new Mock<ICard>();

            // Act
            factory.RegisterBuilder("NormalCard", (t, o, id, ef) => customMockCard.Object);
            var result = factory.CreateCard("Tpl_001", owner, null);

            // Assert
            Assert.Same(customMockCard.Object, result);
        }

        [Fact]
        public void CreateCard_WithValidZone_AddsCardToZone()
        {
            // Arrange
            CreateTemplateJsonFile("Tpl_001", "CustomCard", "Test Card");
            var database = new CardDatabase(_tempDirectory);
            var factory = new CardFactory(database, null);
            var owner = new PlayerBuilder("Test Owner").Build();

            var generatedCardMock = new Mock<ICard>();
            factory.RegisterBuilder("CustomCard", (t, o, id, ef) => generatedCardMock.Object);

            var targetZoneMock = new Mock<IZone>();

            // Act
            var result = factory.CreateCard("Tpl_001", owner, targetZoneMock.Object);

            // Assert
            Assert.Same(generatedCardMock.Object, result);
            targetZoneMock.Verify(z => z.AddCard(generatedCardMock.Object), Times.Once);
        }

        [Fact]
        public void CreateCard_MultipleCalls_IncrementsInstanceIdCorrectly()
        {
            // Arrange
            CreateTemplateJsonFile("Tpl_001", "CustomCard", "Test Card");
            var database = new CardDatabase(_tempDirectory);
            var factory = new CardFactory(database, null);
            var owner = new PlayerBuilder("Test Owner").Build();

            var capturedIds = new List<int>();

            factory.RegisterBuilder("CustomCard", (t, o, id, ef) =>
            {
                capturedIds.Add(id);
                return new Mock<ICard>().Object;
            });

            // Act
            factory.CreateCard("Tpl_001", owner, null);
            factory.CreateCard("Tpl_001", owner, null);
            factory.CreateCard("Tpl_001", owner, null);

            // Assert
            Assert.Equal(3, capturedIds.Count);
            Assert.Equal(1, capturedIds[0]);
            Assert.Equal(2, capturedIds[1]);
            Assert.Equal(3, capturedIds[2]);
        }

        [Fact]
        public void CreateCards_MultipleValidIds_ReturnsListOfCards()
        {
            // Arrange
            CreateTemplateJsonFile("Id1", "NormalCard", "Card 1");
            CreateTemplateJsonFile("Id2", "NormalCard", "Card 2");
            var database = new CardDatabase(_tempDirectory);
            var factory = new CardFactory(database, null);
            var owner = new PlayerBuilder("Test Owner").Build();

            var templateIds = new List<string> { "Id1", "Id2" };

            // Act
            var result = factory.CreateCards(templateIds, owner, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.IsType<NormalCard>(result[0]);
            Assert.IsType<NormalCard>(result[1]);
        }

        [Fact]
        public void CreateCards_EmptyList_ReturnsEmptyList()
        {
            // Arrange
            var database = new CardDatabase(_tempDirectory);
            var factory = new CardFactory(database, null);
            var owner = new PlayerBuilder("Test Owner").Build();
            var emptyList = new List<string>();

            // Act
            var result = factory.CreateCards(emptyList, owner, null);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void CreateCards_WithTargetZone_AddsAllCardsToZone()
        {
            // Arrange
            CreateTemplateJsonFile("Id1", "CustomCard", "Card 1");
            CreateTemplateJsonFile("Id2", "CustomCard", "Card 2");
            var database = new CardDatabase(_tempDirectory);
            var factory = new CardFactory(database, null);
            var owner = new PlayerBuilder("Test Owner").Build();

            var templateIds = new List<string> { "Id1", "Id2" };
            var targetZoneMock = new Mock<IZone>();

            var mockCards = new List<ICard> { new Mock<ICard>().Object, new Mock<ICard>().Object };
            int callCount = 0;

            factory.RegisterBuilder("CustomCard", (t, o, id, ef) =>
            {
                var card = mockCards[callCount];
                callCount++;
                return card;
            });

            // Act
            var result = factory.CreateCards(templateIds, owner, targetZoneMock.Object);

            // Assert
            Assert.Equal(2, result.Count);
            targetZoneMock.Verify(z => z.AddCard(mockCards[0]), Times.Once);
            targetZoneMock.Verify(z => z.AddCard(mockCards[1]), Times.Once);
        }
    }
}