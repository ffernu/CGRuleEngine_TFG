using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Execution.Effects;
using CGRuleEngine_TFG.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Buffers;
using CGRuleEngine_TFG.Entities.Cards.DataBase;

namespace CGRuleEngine_TFG.Entities.Cards
{
    public class BasicCard : ICard
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

        public List<String> Tags { get; set; }
        public Dictionary<String, Object> Properties { get; set; }
        public List<IEffect> Effects { get; set; }

        // Constructor that initializes the BasicCard instance based on a CardTemplate, the owner, and a unique instance ID. It copies the relevant properties from the template to the card instance and initializes the effects list.
        public BasicCard(CardTemplate template, Player owner, int id)// TODO
        {
            this.BaseTemplate = template;
            this.Owner = owner;
            this.InstanceId = id;

            this.Name = template.Name;
            this.SideA = template.SideA;
            this.SideB = template.SideB ?? "";

            this.Tags = new List<string>(template.Tags);
            this.Properties = new Dictionary<String, Object>(template.Properties);
            this.Effects = new List<IEffect>(); // TODO: Add logic to convert effect IDs from the template into actual IEffect instances, possibly using an EffectFactory or similar mechanism

        }

        // Method to flip the card, changing its face-up state
        public void Flip()
        {
            IsFaceUp = !IsFaceUp;
        }

        // Method to tap the card, changing its tapped state
        public void Tap()
        {
            IsTapped = true;
        }

        // Method to untap the card, changing its tapped state
        public void Untap()
        {
            IsTapped = false;
        } 
    }
}
