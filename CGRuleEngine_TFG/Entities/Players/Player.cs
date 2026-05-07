using CGRuleEngine_TFG.Entities.Players.Controllers;
using CGRuleEngine_TFG.Entities.Zones;
using CGRuleEngine_TFG.Execution;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Entities.Players
{
    public class Player : ITargetable
    {
        private static int NextId = 1;

        public String Name { get; set; }
        public int Id { get; set; }
        public Dictionary<String, Object> Properties { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public Dictionary<String, IZone> Zones { get; set; }
        public IPlayerController Controller { get; set; }

        // Constructor that takes a PlayerBuilder object as a parameter and initializes the properties of the Player class with the values from the builder
        public Player(PlayerBuilder builder)
        {
            this.Name = builder.Name;
            this.Id = NextId;
            this.Properties = builder.Properties;
            this.Tags = builder.Tags;
            this.Zones = builder.Zones;
            this.Controller = builder.Controller;
            NextId += 1;
        }

        // Method to get a zone by its name, it checks if the zone exists in the player's zones and returns it, otherwise it throws an exception
        public IZone getZone(String name)
        {
            if (Zones.TryGetValue(name, out IZone zone))
            {
                return zone;
            }
            throw new InvalidOperationException("The player does not have a zone with that name");
        }

        // TODO:
        public void playTurn()
        {
            // TODO: Implement the logic for the player to play their turn, this could involve calling methods on the controller to make decisions and interact with the game state
        }

        // Implementation of the ITargetable interface methods
        public bool TryGetProperty<T>(string propertyName, out T value)
        {
            if (Properties.TryGetValue(propertyName, out object objValue) && objValue is T typedValue)
            {
                value = typedValue;
                return true;
            }
            value = default;
            return false;
        }

        // Method to set a property value, it takes the property name and the value as parameters and updates the Properties dictionary with the new value
        public void SetProperty(string propertyName, object value)
        {
            Properties[propertyName] = value;
        }

        // Method to check if the player has a specific tag, it checks if the Tags list contains the specified tag and returns true if it does, false otherwise
        public bool HasTag(string tag)
        {
            return Tags.Contains(tag);
        }

        // Method to add a tag to the player, it takes a tag as a parameter and adds it to the Tags list if it is not already present
        public void AddTag(string tag)
        {
            if (!Tags.Contains(tag))
            {
                Tags.Add(tag);
            }
        }

        // Method to remove a tag from the player, it takes a tag as a parameter and removes it from the Tags list if it is present
        public void RemoveTag(string tag)
        {
            Tags.Remove(tag);
        }

    }
}
