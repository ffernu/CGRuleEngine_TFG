using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Execution;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Entities.Zones
{
    public interface IZone : ITargetable
    {
        public List<Player> Owners { get; set; }
        public List<ICard> Content { get; set; }

        // Prototype pattern: allows to create a copy of the zone
        public IZone Clone();

        // Method to add a card to the zone, it should also update the card's controller and owner properties
        public void AddCard(ICard card);

        // Method to remove a card from the zone, it should also update the card's controller and owner properties
        public void RemoveCard(ICard card);

        // Method to move a card from this zone to another zone, it should also update the card's controller and owner properties
        public void MoveCard(ICard card, IZone targetZone);

        // Method to search for a card in the zone by its name, it should return the first card that matches the name or null if no card is found
        public ICard Search(string name);

    }
}
