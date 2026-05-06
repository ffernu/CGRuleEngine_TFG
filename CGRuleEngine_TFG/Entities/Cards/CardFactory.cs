using CGRuleEngine_TFG.Entities.Cards.StandardCards;
using CGRuleEngine_TFG.Entities.Cards.DataBase;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Zones;
using CGRuleEngine_TFG.Execution.Effects;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CGRuleEngine_TFG.Entities.Cards
{
    public class CardFactory
    {
        private readonly CardDatabase _database;
        private int _idCounter = 0; // Counter to generate unique instance IDs
        private readonly EffectFactory _effectFactory; // EffectFactory instance to create effects for cards

        private readonly Dictionary<string, Func<CardTemplate, Player, int, EffectFactory, ICard>> _builders = new Dictionary<string, Func<CardTemplate, Player, int, EffectFactory, ICard>>(); // Dictionary to hold custom builders for different card types

        // Method to create a new card instance based on a template ID and registered builders for standard card types
        public CardFactory(CardDatabase database, EffectFactory effectFactory)
        {
            this._database = database;
            this._effectFactory = effectFactory;
            RegisterBuilder("NormalCard", (t, o, id, ef) => new NormalCard(t, o, id, ef));
        }

        // Method to register a custom builder function for a specific card type
        public void RegisterBuilder(string cardType, Func<CardTemplate, Player, int, EffectFactory, ICard> builderFunction)
        {
            _builders[cardType] = builderFunction;
        }

        // Method to create a card instance based on the template ID and the owner, using the registered builders
        public ICard CreateCard(string templateId, Player owner, IZone? zone)
        {
            CardTemplate template = _database.GetTemplate(templateId);
            string typeCard = template.Type;

            if (_builders.TryGetValue(typeCard, out var constructor))
            {
                _idCounter++;

                // Use the registered builder function to create the card instance
                ICard card = constructor(template, owner, _idCounter, this._effectFactory);
                if (zone != null)
                {
                    zone.AddCard(card);
                }
                return card;
            }
            throw new Exception($"No builder registered for card type: {typeCard}");
        }

        // Method to create multiple card instances based on a list of template IDs and the owner, using the registered builders
        public List<ICard> CreateCards(List<string> templateIds, Player owner, IZone? zone)
        {
            List<ICard> createdCards = new List<ICard>();
            foreach (string id in templateIds)
            {
                    ICard card = CreateCard(id, owner, zone);
                    createdCards.Add(card);
            }
            return createdCards;
        }
    }
}
