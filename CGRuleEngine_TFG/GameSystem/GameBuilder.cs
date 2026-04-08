using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.GameSystem.TurnSystem;
using CGRuleEngine_TFG.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using CGRuleEngine_TFG.Entities.Cards;

namespace CGRuleEngine_TFG.GameSystem
{
    public class GameBuilder
    {
        public List<Player> Players { get; set; } = new List<Player>();
        public TurnFactory TemplateTurn { get; set; }
        public ActionStack Stack { get; set; }
        public CardFactory CardFactory { get; set; }

        // Constructor that initializes the properties of the GameBuilder class with default values
        public GameBuilder() { }

        // Method to build a Game object using the properties of the GameBuilder class
        public Game Build()
        {
            return new Game(this);
        }

        // Method to add a player to the list of players in the GameBuilder class and return the GameBuilder object for method chaining
        public GameBuilder WithPlayer(Player player)
        {
            Players.Add(player);
            return this;
        }

        // Method to set the current turn in the GameBuilder class and return the GameBuilder object for method chaining
        public GameBuilder SetStack(ActionStack stack)
        {
            this.Stack = stack;
            return this;
        }

        // Method to set the current turn in the GameBuilder class and return the GameBuilder object for method chaining
        public GameBuilder SetTurnTemplate(TurnFactory template)
        {
            this.TemplateTurn = template;
            return this;
        }

        // Method to set the card factory in the GameBuilder class and return the GameBuilder object for method chaining
        public GameBuilder SetCardFactory(CardFactory cardFactory)
        {
            this.CardFactory = cardFactory;
            return this;
        }
    }
}
