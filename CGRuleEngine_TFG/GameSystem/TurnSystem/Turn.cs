using CGRuleEngine_TFG.Entities.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.GameSystem.TurnSystem
{
    public class Turn
    {
        public Player ActivePlayer { get; set; }
        public List<Phase> Phases { get; set; } = new List<Phase>();
        public Phase CurrentPhase { get; set; }
        public int CurrentPhaseIndex { get; set; }

        // Constructor that initializes the properties of the Turn class with default values
        public Turn(Player player, List<Phase> phases)
        {
            this.ActivePlayer = player;
            this.Phases = phases;
            this.CurrentPhase = null; // The current phase will be initialized when the turn starts, it is set to null for now
        }

        // Method to start the turn, this method checks if the turn has at least one phase before starting the turn. If the turn has at least one phase, it sets the current phase index to 0 and initializes the current phase with the first phase in the list of phases
        public void Start(Game game)
        {
            if (Phases.Count == 0)
            {
                throw new InvalidOperationException("The turn must have at least one phase to start");
            }
            this.CurrentPhaseIndex = 0;
            this.CurrentPhase = Phases[CurrentPhaseIndex];
            this.CurrentPhase.OnEnter(game);
        }

        // Method to move to the next phase of the turn, this method checks if the current phase index is less than the total number of phases in the turn. If it is not, it returns false indicating that there are no more phases left in the turn
        public bool NextPhase(Game game)
        {
            this.CurrentPhase.OnExit(game);

            if (CurrentPhaseIndex >= Phases.Count - 1)
            {
                return false;
            }
            this.CurrentPhaseIndex++;
            this.CurrentPhase = Phases[CurrentPhaseIndex];
            this.CurrentPhase.OnEnter(game);

            return true;
        }

        // Method to get the current phase of the turn, this method returns the current phase of the turn
        public Phase GetCurrentPhase()
        {
            return this.CurrentPhase;
        }
    }
}
