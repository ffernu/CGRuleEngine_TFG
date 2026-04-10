using CGRuleEngine_TFG.Entities.Cards.DataBase;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Zones;
using CGRuleEngine_TFG.Execution.Effects;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Entities.Cards
{
    public abstract class CardBase : ICard
    {
        public CardTemplate BaseTemplate { get; set; }
        public String Name { get; set; }
        public int InstanceId { get; set; }

        public String SideA { get; set; }
        public String SideB { get; set; }
        public bool IsFaceUp { get; set; } = true;

        public bool IsTapped { get; set; } = false;

        public Player Owner { get; set; }
        public Player? Controller { get; set; }
        public IZone? CurrentZone { get; set; }

        public List<String> Tags { get; set; }
        public Dictionary<String, Object> Properties { get; set; }
        public List<IEffect> Effects { get; set; }

        // Constructor that initializes the card's properties based on the provided template, owner, and instance ID
        protected CardBase(CardTemplate template, Player owner, int instanceId)
        {
            this.BaseTemplate = template;
            this.Owner = owner;
            this.InstanceId = instanceId;

            this.Name = template.Name;
            this.SideA = template.SideA;
            this.SideB = template.SideB ?? "";

            this.Tags = new List<string>(template.Tags ?? new List<string>());
            this.Properties = new Dictionary<string, object>(template.Properties ?? new Dictionary<string, object>());
            this.Effects = new List<IEffect>(); // TODO
        }

        // Method to try to get a property value by its name and type, it checks if the property exists and is of the correct type, returning true if successful and false otherwise
        public bool TryGetProperty<T>(string propertyName, out T value)
        {
            // Check if the property exists in the Properties dictionary and if it is of the expected type T
            if (Properties.TryGetValue(propertyName, out object objValue) && objValue is T typedValue)
            {
                value = typedValue;
                return true;
            }

            // If the property does not exist or is not of the expected type, set the output value to the default for type T and return false
            value = default;
            return false;
        }

        // Method to set a property value by its name, it updates the Properties dictionary with the provided value for the specified property name
        public void SetProperty(string propertyName, object value)
        {
            Properties[propertyName] = value;
        }

        // Method to check if the card has a specific tag, it checks if the Tags list contains the specified tag and returns true if it does, false otherwise
        public bool HasTag(string tag)
        {
            return Tags.Contains(tag);
        }

        // Method to add a tag to the card, it adds the specified tag to the Tags list if it is not already present
        public void AddTag(string tag)
        {
            if (!Tags.Contains(tag))
            {
                Tags.Add(tag);
            }
        }

        // Method to remove a tag from the card, it removes the specified tag from the Tags list if it is present
        public void RemoveTag(string tag)
        {
            Tags.Remove(tag);
        }

        // Method to reset the card's state, it resets the card's tags and properties to their initial values from the template
        public virtual void Reset()
        {
            // Reset the card's tags and properties to their initial values from the template
            this.Tags = new List<string>(this.BaseTemplate.Tags ?? new List<string>());
            this.Properties = new Dictionary<string, object>(this.BaseTemplate.Properties ?? new Dictionary<string, object>());

            this.Effects = new List<IEffect>();// TODO

            this.IsFaceUp = true;
            this.IsTapped = false;
        }

        // Method to flip the card, changing its face-up state by toggling the IsFaceUp property
        public void Flip()
        {
            IsFaceUp = !IsFaceUp;
        }

        // Method to tap the card, changing its tapped state by setting the IsTapped property to true
        public void Tap()
        {
            IsTapped = true;
        }

        public void Untap()
        {
            IsTapped = false;
        }
    }
}
