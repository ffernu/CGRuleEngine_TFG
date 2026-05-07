using CGRuleEngine_TFG.Entities.Cards;
using CGRuleEngine_TFG.Entities.Players;
using CGRuleEngine_TFG.Entities.Zones;
using CGRuleEngine_TFG.Execution;
using CGRuleEngine_TFG.Execution.Actions;
using CGRuleEngine_TFG.GameSystem.TurnSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.GameSystem
{
    public class Game : IGameState, IActionHandler
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
        public Player? PriorityPlayer { get; set; }
        private int consecutivePasses = 0;

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
            foreach (Player player in this.Players)
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
            if (this.IsGameOver)
            {
                throw new InvalidOperationException("The game is already over");
            }
            if (!this.Stack.IsEmpty())
            {
                throw new InvalidOperationException("The stack is not empty, cannot start the game");
            }
            this.nextTurn(this.Players[0]);
        }

        // Method to start the next turn of the game, this method checks if the game is already over before starting the next turn. If the game is not over, it sets the active player to the next player in the list of players, creates a new turn using the template turn and starts it
        public void nextTurn()
        {
            if (this.IsGameOver)
            {
                throw new InvalidOperationException("The game is already over");
            }

            int nextIndex = this.Players.IndexOf(this.ActivePlayer);
            Player candidate;
            do
            {
                nextIndex = (nextIndex + 1) % this.Players.Count;
                candidate = this.Players[nextIndex];
            } while (IsEliminated(candidate) && candidate != this.ActivePlayer);

            this.ActivePlayer = candidate;
            this.TurnNumber += 1;
            this.CurrentTurn = this.TemplateTurn.CreateTurn(this.ActivePlayer);

            this.ResetPriority();
            this.CurrentTurn.Start(this);
            this.NotifyPriorityPlayer();
        }

        // Method to start the next turn of the game, this method checks if the game is already over before starting the next turn. If the game is not over, it sets the active player to the given player, creates a new turn using the template turn and starts it
        public void nextTurn(Player player)
        {
            if (this.IsGameOver)
            {
                throw new InvalidOperationException("The game is already over");
            }
            if(this.IsEliminated(player))
            {
                throw new InvalidOperationException("The given player is eliminated and cannot start a turn");
            }
            this.ActivePlayer = player;
            this.TurnNumber += 1;
            this.CurrentTurn = this.TemplateTurn.CreateTurn(ActivePlayer);

            this.ResetPriority();
            this.CurrentTurn.Start(this);
            this.NotifyPriorityPlayer();
        }

        // Method to check the win condition of the game, this method is marked as virtual to allow for customization in derived classes. It can be used to check if any player has met the win condition and set the IsGameOver property to true if the game is over
        public virtual void checkWinCondition()
        {
            if (this.IsGameOver) return;

            List<Player> alivePlayers = new List<Player>();
            Player? vpWinner = null;

            foreach (Player player in this.Players)
            {
                if (player.TryGetProperty<int>("Health", out int health) && health > 0)
                {
                    alivePlayers.Add(player);
                }

                if (player.TryGetProperty<int>("VictoryPoints", out int vp) && vp >= 20)
                {
                    vpWinner = player;
                    break;
                }
            }

            if (vpWinner != null)
            {
                this.EndGame(vpWinner);
            }
            else if (alivePlayers.Count == 1)
            {
                this.EndGame(alivePlayers[0]);
            }
            else if (alivePlayers.Count == 0)
            {
                this.IsGameOver = true;
            }
        }

        // Method to get the rank of a player, this method checks if the player is in the results dictionary and returns their rank, otherwise it throws an exception
        public int getRank(Player player)
        {
            return results[player];
        }

        // Method to pass priority to the next player, this method checks if the given player is the current priority player, if it is not, it returns without doing anything. If it is, it increments the consecutivePasses counter and sets the next player in the list as the new priority player. If the number of consecutive passes is equal to the number of players, it resets the counter and checks if the stack is empty. If the stack is not empty, it pops an action from the stack and sets the priority player to the active player. If the stack is empty, it calls the nextPhase method of the current turn to move to the next phase of the turn
        public void PassPriority(Player player)
        {
            if (player != this.PriorityPlayer) return;
            this.consecutivePasses++;

            int nextIndex = this.Players.IndexOf(this.PriorityPlayer);
            Player nextPriorityPlayer;
            do
            {
                nextIndex = (nextIndex + 1) % this.Players.Count;
                nextPriorityPlayer = this.Players[nextIndex];
            } while (IsEliminated(nextPriorityPlayer) && nextPriorityPlayer != this.PriorityPlayer);

            this.PriorityPlayer = nextPriorityPlayer;

            if (this.consecutivePasses >= this.Players.Count)
            {
                this.ResetPriority();
                this.checkWinCondition();
                if (this.IsGameOver) return;
                if (IsEliminated(this.ActivePlayer))
                {
                    this.nextTurn();
                    return;
                }
                if (!this.Stack.IsEmpty())
                {
                    while (!this.Stack.IsEmpty())
                    {
                        this.Stack.Pop();
                        this.checkWinCondition();
                        if (this.IsGameOver) return;
                    }
                    this.NotifyPriorityPlayer();
                }
                else
                {
                    bool turnIsStillActive = this.CurrentTurn.NextPhase(this);

                    // If the turn did end after moving to the next phase, we need to start the next turn
                    if (!turnIsStillActive)
                    {
                        this.nextTurn();
                    }
                    else
                    {
                        this.NotifyPriorityPlayer();
                    }
                }
            }
            else
            {
                this.NotifyPriorityPlayer();
            }
        }

        // Method to end the game, this method sets the IsGameOver property to true and can be used to calculate the results
        public void EndGame(Player winner)
        {
            this.IsGameOver = true;
            this.results[winner] = 1;

            int otherRank = 2;
            foreach (var player in this.Players)
            {
                if (player != winner)
                {
                    this.results[player] = otherRank++;
                }
            }
        }

        // Helper method to check if a player is eliminated, this method checks if the player has a "Health" property and if its value is less than or equal to 0, which indicates that the player is eliminated
        private bool IsEliminated(Player p)
        {
            return p.TryGetProperty<int>("Health", out int health) && health <= 0;
        }

        // Method to reset the priority, this method sets the priority player to the active player and resets the consecutivePasses counter
        public void ResetPriority()
        {
            this.PriorityPlayer = this.ActivePlayer;
            this.consecutivePasses = 0;
        }

        // If the game is not over, it calls the RequestAction method of the priority player's controller to request an action from them
        public void NotifyPriorityPlayer()
        {
            if (this.IsGameOver) return;
            this.PriorityPlayer?.Controller.RequestAction(this);
        }
    }
}
