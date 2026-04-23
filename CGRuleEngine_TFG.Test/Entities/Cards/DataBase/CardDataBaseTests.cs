using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Newtonsoft.Json;
using CGRuleEngine_TFG.Entities.Cards.DataBase;

namespace CGRuleEngine_TFG.Tests.Entities.Cards.DataBase
{
    public class CardDatabaseTests : IDisposable
    {
        private readonly string _tempDirectory;

        // Setup: Create a temporary directory for each test to provide a real file system for CardDatabase
        public CardDatabaseTests()
        {
            _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);
        }

        // Teardown: Clean up the temporary directory after each test
        public void Dispose()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }

        // Helper to create a JSON file in the temp directory
        private void CreateCardJsonFile(string fileName, object content)
        {
            string filePath = Path.Combine(_tempDirectory, fileName);
            string json = JsonConvert.SerializeObject(content);
            File.WriteAllText(filePath, json);
        }

        [Fact]
        public void Constructor_InvalidDirectory_ThrowsDirectoryNotFoundException()
        {
            // Arrange
            string invalidPath = Path.Combine(_tempDirectory, "NonExistentFolder");

            // Act & Assert
            var exception = Assert.Throws<DirectoryNotFoundException>(() => new CardDatabase(invalidPath));
            Assert.Equal("The path provided doesnt exist.", exception.Message);
        }

        [Fact]
        public void Constructor_ValidDirectoryWithFiles_LoadsTemplates()
        {
            // Arrange
            CreateCardJsonFile("card1.json", new { Id = "Card01", Name = "Goblin" });
            CreateCardJsonFile("card2.json", new { Id = "Card02", Name = "Dragon" });

            // Act
            var database = new CardDatabase(_tempDirectory);

            // Assert
            Assert.Equal(2, database.Count);
            Assert.True(database.Contains("Card01"));
            Assert.True(database.Contains("Card02"));
        }

        [Fact]
        public void Constructor_FileWithDuplicateId_IgnoresDuplicateAndContinues()
        {
            // Arrange
            CreateCardJsonFile("card1.json", new { Id = "DupCard", Name = "First" });
            CreateCardJsonFile("card2.json", new { Id = "DupCard", Name = "Second" }); // Duplicate ID

            // Act
            var database = new CardDatabase(_tempDirectory);

            // Assert
            Assert.Equal(1, database.Count); // Only one should be loaded
            var loadedTemplate = database.GetTemplate("DupCard");
            // Due to unpredictable file reading order, we just assert it loaded one of them
            Assert.True(loadedTemplate.Name == "First" || loadedTemplate.Name == "Second");
        }

        [Fact]
        public void Constructor_InvalidJsonFormat_IgnoresFileAndContinues()
        {
            // Arrange
            CreateCardJsonFile("valid.json", new { Id = "ValidCard" });
            File.WriteAllText(Path.Combine(_tempDirectory, "invalid.json"), "This is not a JSON {");

            // Act
            var database = new CardDatabase(_tempDirectory);

            // Assert
            Assert.Equal(1, database.Count);
            Assert.True(database.Contains("ValidCard"));
        }

        [Fact]
        public void Constructor_MissingId_IgnoresFile()
        {
            // Arrange
            CreateCardJsonFile("missingId.json", new { Name = "No Id Card" });
            CreateCardJsonFile("validId.json", new { Id = "Valid1" });

            // Act
            var database = new CardDatabase(_tempDirectory);

            // Assert
            Assert.Equal(1, database.Count);
            Assert.True(database.Contains("Valid1"));
        }

        [Fact]
        public void Constructor_LongProperties_ConvertsLongToInt()
        {
            // Arrange
            // Explicitly using long to simulate Newtonsoft default behavior for numeric deserialization
            long costValue = 10L;
            var cardWithLongProp = new
            {
                Id = "PropCard",
                Properties = new Dictionary<string, object>
                {
                    { "Cost", costValue }
                }
            };
            CreateCardJsonFile("prop.json", cardWithLongProp);

            // Act
            var database = new CardDatabase(_tempDirectory);
            var template = database.GetTemplate("PropCard");

            // Assert
            Assert.NotNull(template.Properties);
            Assert.True(template.Properties.ContainsKey("Cost"));
            Assert.IsType<int>(template.Properties["Cost"]); // Verify conversion to int
            Assert.Equal(10, template.Properties["Cost"]);
        }

        [Fact]
        public void GetTemplate_ExistingId_ReturnsTemplate()
        {
            // Arrange
            CreateCardJsonFile("card.json", new { Id = "Hero", Name = "Knight" });
            var database = new CardDatabase(_tempDirectory);

            // Act
            var template = database.GetTemplate("Hero");

            // Assert
            Assert.NotNull(template);
            Assert.Equal("Hero", template.Id);
            Assert.Equal("Knight", template.Name);
        }

        [Fact]
        public void GetTemplate_NonExistingId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var database = new CardDatabase(_tempDirectory);

            // Act & Assert
            var exception = Assert.Throws<KeyNotFoundException>(() => database.GetTemplate("Ghost"));
            Assert.Equal("No existe ninguna carta con el ID: Ghost", exception.Message);
        }

        [Fact]
        public void Contains_ExistingId_ReturnsTrue()
        {
            // Arrange
            CreateCardJsonFile("card.json", new { Id = "Hero" });
            var database = new CardDatabase(_tempDirectory);

            // Act
            bool result = database.Contains("Hero");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_NonExistingId_ReturnsFalse()
        {
            // Arrange
            var database = new CardDatabase(_tempDirectory);

            // Act
            bool result = database.Contains("Unknown");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Reload_FilesChanged_UpdatesDatabase()
        {
            // Arrange
            CreateCardJsonFile("initial.json", new { Id = "Card1" });
            var database = new CardDatabase(_tempDirectory);

            // Validate initial state
            Assert.Equal(1, database.Count);
            Assert.True(database.Contains("Card1"));

            // Change filesystem
            CreateCardJsonFile("newFile.json", new { Id = "Card2" });
            File.Delete(Path.Combine(_tempDirectory, "initial.json"));

            // Act
            database.Reload();

            // Assert
            Assert.Equal(1, database.Count);
            Assert.False(database.Contains("Card1")); // Was deleted
            Assert.True(database.Contains("Card2"));  // Was added
        }

        [Fact]
        public void GetAllTemplates_HasTemplates_ReturnsAllTemplates()
        {
            // Arrange
            CreateCardJsonFile("card1.json", new { Id = "CardA" });
            CreateCardJsonFile("card2.json", new { Id = "CardB" });
            var database = new CardDatabase(_tempDirectory);

            // Act
            var templates = database.GetAllTemplates().ToList();

            // Assert
            Assert.Equal(2, templates.Count);
            Assert.Contains(templates, t => t.Id == "CardA");
            Assert.Contains(templates, t => t.Id == "CardB");
        }

        [Fact]
        public void FilterBy_ValidPredicate_ReturnsMatchingTemplates()
        {
            // Arrange
            CreateCardJsonFile("card1.json", new { Id = "Spell1", Type = "SpellCard" });
            CreateCardJsonFile("card2.json", new { Id = "Creature1", Type = "NormalCard" });
            CreateCardJsonFile("card3.json", new { Id = "Spell2", Type = "SpellCard" });

            var database = new CardDatabase(_tempDirectory);

            // Act
            var spellCards = database.FilterBy(t => t.Type == "SpellCard");

            // Assert
            Assert.Equal(2, spellCards.Count);
            Assert.Contains(spellCards, t => t.Id == "Spell1");
            Assert.Contains(spellCards, t => t.Id == "Spell2");
            Assert.DoesNotContain(spellCards, t => t.Id == "Creature1");
        }

        [Fact]
        public void FilterBy_NoMatch_ReturnsEmptyList()
        {
            // Arrange
            CreateCardJsonFile("card1.json", new { Id = "Creature1", Type = "NormalCard" });
            var database = new CardDatabase(_tempDirectory);

            // Act
            var matches = database.FilterBy(t => t.Type == "TrapCard");

            // Assert
            Assert.Empty(matches);
        }
    }
}