using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Execution;
using CGRuleEngine_TFG.Execution.Effects;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Entities.Cards
{
    public interface ICard : ITargetable
    {
        public String Name { get; set; }
        public int id { get; set; }

        public String sideA { get; set; }
        public String sideB { get; set; }
        public bool isFaceUp { get; set; }

        public bool isTapped { get; set; }

        public Player Owner { get; set; }
        public Player? Controller { get; set; }

        public List<String> Tags { get; set; }
        public Dictionary<String, Object> Properties { get; set; }
        public List<IEffect> Effects { get; set; }

        // Method to flip the card, changing its face-up state
        public void Flip()
        {
            isFaceUp = !isFaceUp;
        }

        // Method to tap the card, changing its tapped state
        public void Tap()
        {
            isTapped = true;
        }

        // Method to untap the card, changing its tapped state
        public void Untap()
        {
            isTapped = false;
        }
    }
}
