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
        public Dictionary<String, int> Stats { get; set; }
        public Dictionary<String, IZone> Zones { get; set; }
        private IPlayerController Controller { get; set; }

        // Constructor that takes a PlayerBuilder object as a parameter and initializes the properties of the Player class with the values from the builder
        public Player(PlayerBuilder builder)
        {
            Name = builder.Name;
            Id = NextId;
            Stats = builder.Stats;
            Zones = builder.Zones;
            Controller = builder.Controller;
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

    }
}
