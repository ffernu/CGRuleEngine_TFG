using System;
using System.Collections.Generic;
using Xunit;
using Newtonsoft.Json;
using CGRuleEngine_TFG.Entities.Cards.DataBase;

namespace CGRuleEngine_TFG.Tests.Entities.Cards.DataBase
{
    public class CardTemplateTests
    {
        [Fact]
        public void CardTemplate_SetProperties_StoresValuesCorrectly()
        {
            // Arrange
            var template = new CardTemplate();

            // Act
            template.Id = "Test01";
            template.Type = "NormalCard";
            template.Name = "Test Name";
            template.SideA = "Front";
            template.SideB = "Back";
            template.TransformInto = "Test02";
            template.Tags = new List<string> { "Magic", "Fire" };
            template.Properties = new Dictionary<string, object> { { "Damage", 5 } };
            template.Effects = new List<EffectData>
            {
                new EffectData { Type = "DamageEffect", Parameters = new Dictionary<string, object>() }
            };

            // Assert
            Assert.Equal("Test01", template.Id);
            Assert.Equal("NormalCard", template.Type);
            Assert.Equal("Test Name", template.Name);
            Assert.Equal("Front", template.SideA);
            Assert.Equal("Back", template.SideB);
            Assert.Equal("Test02", template.TransformInto);
            Assert.Equal(2, template.Tags.Count);
            Assert.Contains("Magic", template.Tags);
            Assert.Single(template.Properties);
            Assert.Equal(5, template.Properties["Damage"]);
            Assert.Single(template.Effects);
            Assert.Equal("DamageEffect", template.Effects[0].Type);
        }

        [Fact]
        public void EffectData_SetProperties_StoresValuesCorrectly()
        {
            // Arrange
            var effectData = new EffectData();
            var parameters = new Dictionary<string, object>
            {
                { "Amount", 10 }
            };

            // Act
            effectData.Type = "HealEffect";
            effectData.Parameters = parameters;

            // Assert
            Assert.Equal("HealEffect", effectData.Type);
            Assert.NotNull(effectData.Parameters);
            Assert.Equal(10, effectData.Parameters["Amount"]);
        }

        [Fact]
        public void Deserialize_ValidJson_CreatesCardTemplateCorrectly()
        {
            // Arrange
            string json = @"
            {
                ""Type"": ""NormalCard"",
                ""Id"": ""TestCard"",
                ""Name"": ""The best card"",
                ""SideA"": ""ArtA"",
                ""SideB"": """",
                ""Tags"": [""Sorcery""],
                ""Properties"": { ""Cost"": 10 },
                ""Effects"": [
                    {
                        ""Type"": ""DrawEffect"",
                        ""Parameters"": { ""Amount"": 2 }
                    }
                ]
            }";

            // Act
            var template = JsonConvert.DeserializeObject<CardTemplate>(json);

            // Assert
            Assert.NotNull(template);
            Assert.Equal("NormalCard", template.Type);
            Assert.Equal("TestCard", template.Id);
            Assert.Equal("The best card", template.Name);
            Assert.Equal("ArtA", template.SideA);
            Assert.Equal("", template.SideB);
            Assert.Null(template.TransformInto);
            Assert.Single(template.Tags);
            Assert.Contains("Sorcery", template.Tags);
            Assert.Equal(10L, template.Properties["Cost"]); // Newtonsoft deserializes small integers to Int64 (long) by default in object graphs
            Assert.Single(template.Effects);
            Assert.Equal("DrawEffect", template.Effects[0].Type);
            Assert.Equal(2L, template.Effects[0].Parameters["Amount"]);
        }
    }
}