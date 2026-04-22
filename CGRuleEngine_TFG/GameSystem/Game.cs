using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Zones;
using CGRuleEngine_TFG.Execution;
using CGRuleEngine_TFG.GameSystem.TurnSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.GameSystem
{
    public class Game : IGameState
    {
        public List<Player> Players { get; set; } = new List<Player>();

        // Explicit implementation of the Players property from the IGameState interface, this allows the Players property to be accessed as an IReadOnlyList<Player> when accessed through an IGameState reference, while still allowing it to be modified as a List<Player> when accessed through a Game reference
        IReadOnlyList<Player> IGameState.Players => this.Players;
        public Player? ActivePlayer { get; set; }
        public CardFactory CardFactory { get; set; }
        public TurnFactory TemplateTurn { get; set; }
        public Turn? CurrentTurn { get; set; }
        public int TurnNumber { get; set; }
        public ActionStack Stack { get; set; }
        public bool IsGameOver { get; set; }
        public Dictionary<Player, int> results { get; set; }

        // Constructor that takes a GameBuilder object as a parameter and initializes the properties of the Game class with the values from the builder
        public Game(GameBuilder builder)
        {
            this.Players = builder.Players;
            this.ActivePlayer = null; // The active player will be initialized when the game starts, it is set to null for now
            this.TemplateTurn = builder.TemplateTurn;
            this.CardFactory = builder.CardFactory;
            this.CurrentTurn = null; // The current turn will be initialized when the game starts, it is set to null for now
            this.TurnNumber = 0;
            this.Stack = new ActionStack(this);
            this.IsGameOver = false;
            this.results = new Dictionary<Player, int>();
        }

        // Method to initialize the game, this method can be used to set up the initial game state, such as shuffling decks, drawing starting hands, etc. It is marked as virtual to allow for customization in derived classes
        public virtual void initializeGame()
        {
            foreach (Player player in Players)
            {
                // Initialize player zones, shuffle decks, draw starting hands, etc.
                foreach (IZone zone in player.Zones.Values)
                {
                    zone.Initialize();
                }
            }
        }

        // Method to start the game, this method checks if the game is already over or if the stack is not empty before starting the game. If the game is not over and the stack is empty, it calls the nextTurn method to start the first turn of the game
        public void startGame()
        {
            if (IsGameOver)
            {
                throw new InvalidOperationException("The game is already over");
            }
            if (!this.Stack.IsEmpty())
            {
                throw new InvalidOperationException("The stack is not empty, cannot start the game");
            }
            this.nextTurn(Players[0]);
        }

        // Method to start the next turn of the game, this method checks if the game is already over before starting the next turn. If the game is not over, it sets the active player to the next player in the list of players, creates a new turn using the template turn and starts it
        public void nextTurn()
        {
            if (IsGameOver)
            {
                throw new InvalidOperationException("The game is already over");
            }
            int lastPlayer = Players.IndexOf(ActivePlayer);
            this.ActivePlayer = Players[lastPlayer + 1];
            this.TurnNumber += 1;
            this.CurrentTurn = this.TemplateTurn.CreateTurn(ActivePlayer);
            this.CurrentTurn.Start();
        }

        // Method to start the next turn of the game, this method checks if the game is already over before starting the next turn. If the game is not over, it sets the active player to the given player, creates a new turn using the template turn and starts it
        public void nextTurn(Player player)
        {
            if (IsGameOver)
            {
                throw new InvalidOperationException("The game is already over");
            }
            this.ActivePlayer = player;
            this.TurnNumber += 1;
            this.CurrentTurn = this.TemplateTurn.CreateTurn(ActivePlayer);
            this.CurrentTurn.Start();
        }

        // Method to check the win condition of the game, this method is marked as virtual to allow for customization in derived classes. It can be used to check if any player has met the win condition and set the IsGameOver property to true if the game is over
        public virtual void checkWinCondition()
        {
            // TODO: Implement an standart logic to check the win condition of the game
        }

        // Method to get the rank of a player, this method checks if the player is in the results dictionary and returns their rank, otherwise it throws an exception
        public int getRank(Player player)
        {
            return results[player];
        }

    }
}
