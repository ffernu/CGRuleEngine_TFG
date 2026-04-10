using CGRuleEngine_TFG.Entities.Cards.DataBase;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Zones;
using CGRuleEngine_TFG.Execution;
using CGRuleEngine_TFG.Execution.Effects;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Entities.Cards
{
    public interface ICard : ITargetable
    {
        public CardTemplate BaseTemplate { get; set; }
        public String Name { get; set; }
        public int InstanceId { get; set; }

        public String SideA { get; set; }
        public String SideB { get; set; }
        public bool IsFaceUp { get; set; }

        public bool IsTapped { get; set; }

        public Player Owner { get; set; }
        public Player? Controller { get; set; }
        public IZone? CurrentZone { get; set; }

        public List<String> Tags { get; set; }
        public Dictionary<String, Object> Properties { get; set; }
        public List<IEffect> Effects { get; set; }

        // Method to flip the card, changing its face-up state
        public void Flip();

        // Method to tap the card, changing its tapped state
        public void Tap();

        // Method to untap the card, changing its tapped state
        public void Untap();

        // Method to reset the card's state, untapping it and flipping it face up
        public void Reset();
    }
}
