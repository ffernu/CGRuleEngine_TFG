using CGRuleEngine_TFG.Entities.Players.Controllers;
using CGRuleEngine_TFG.Entities.Zones;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Entities.Players
{
    public class PlayerBuilder
    {
        public String Name { get; set; }
        public Dictionary<String, Object> Properties { get; set; } = new Dictionary<String, Object>();
        public List<string> Tags { get; set; } = new List<string>();
        public Dictionary<String, IZone> Zones { get; set; } = new Dictionary<String, IZone>();
        public IPlayerController Controller { get; set; }

        // Constructor that takes a name as a parameter and initializes the Name property of the builder with the given name
        public PlayerBuilder(String name)
        {
            this.Name = name;
        }

        // Default constructor that initializes the Name property of the builder with an empty string
        public PlayerBuilder() { }

        // Method to set the properties of the player, it takes a dictionary of properties as a parameter and assigns it to the properties property of the builder, it returns the builder itself to allow for method chaining
        public Player Build()
        {
            return new Player(this);
        }

        // Method to set the name of the player, it takes a string as a parameter and assigns it to the Name property of the builder
        public PlayerBuilder SetName(String name)
        {
            this.Name = name;
            return this;
        }

        // Method to set the controller of the player, it takes an IPlayerController as a parameter and assigns it to the Controller property of the builder
        public PlayerBuilder SetController(IPlayerController controller)
        {
            this.Controller = controller;
            return this;
        }

        // Method to set the properties of the player, it takes a name and a value as parameters and adds them to the properties dictionary of the builder
        public PlayerBuilder WithStat(String name, int value)
        {
            Properties[name] = value;
            return this;
        }

        // Method to set a tag for the player, it takes a string as a parameter and adds it to the Tags list of the builder
        public PlayerBuilder WithTag(String tag)
        {
            Tags.Add(tag);
            return this;
        }

        // Method to set a zone for the player, it takes a name and an IZone as parameters and adds them to the Zones dictionary of the builder
        public PlayerBuilder WithZone(String name, IZone zone)
        {
            Zones[name] = zone;
            return this;
        }
    }
}
