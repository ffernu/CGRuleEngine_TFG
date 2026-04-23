using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using CGRuleEngine_TFG.Execution.Effects;
using CGRuleEngine_TFG.Entities.Cards.DataBase;

namespace CGRuleEngine_TFG.Tests.Execution.Effects
{
    public class EffectFactoryTests
    {
        [Fact]
        public void Constructor_RegistersDefaultBuildersSuccessfully()
        {
            // Arrange
            var factory = new EffectFactory();

            var data = new EffectData
            {
                Type = "ModifyPropertyEffect",
                Parameters = new Dictionary<string, object>()
            };

            // Act
            var effect = factory.CreateEffect(data);

            // Assert
            Assert.NotNull(effect);
            Assert.IsAssignableFrom<IEffect>(effect);
        }

        [Fact]
        public void CreateEffect_UnregisteredType_ThrowsException()
        {
            // Arrange
            var factory = new EffectFactory();
            var data = new EffectData { Type = "UnknownEffect" };

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => factory.CreateEffect(data));
            Assert.Equal("The effect 'UnknownEffect' is not registered in the EffectFactory.", exception.Message);
        }

        [Fact]
        public void RegisterBuilder_CustomType_RegistersAndCreatesSuccessfully()
        {
            // Arrange
            var factory = new EffectFactory();
            var customEffectMock = new Mock<IEffect>();

            var data = new EffectData { Type = "CustomEffect" };

            // Act
            factory.RegisterBuilder("CustomEffect", d => customEffectMock.Object);
            var result = factory.CreateEffect(data);

            // Assert
            Assert.Same(customEffectMock.Object, result);
        }

        [Fact]
        public void CreateEffect_ModifyPropertyEffectWithMissingParams_UsesDefaultValues()
        {
            // Arrange
            var factory = new EffectFactory();
            var data = new EffectData
            {
                Type = "ModifyPropertyEffect",
                Parameters = null // Edge case: Null parameters dictionary
            };

            // Act
            var effect = factory.CreateEffect(data);

            // Assert
            Assert.NotNull(effect);
            Assert.IsAssignableFrom<IEffect>(effect);
            // We cannot strongly assert the exact properties of ModifyPropertyEffect as it's not available in context,
            // but we verify the creation succeeds without throwing null reference exceptions.
        }

        [Fact]
        public void CreateEffect_WithInvalidParameterType_CatchesExceptionAndUsesDefaultValue()
        {
            // Arrange
            var factory = new EffectFactory();
            var data = new EffectData
            {
                Type = "ModifyPropertyEffect",
                Parameters = new Dictionary<string, object>
                {
                    { "Value", "NotANumber" } // Invalid type that fails Convert.ChangeType internally
                }
            };

            // Act
            var exception = Record.Exception(() => factory.CreateEffect(data));

            // Assert
            Assert.Null(exception); // Proves the try/catch inside GetParam worked properly
        }
    }
}