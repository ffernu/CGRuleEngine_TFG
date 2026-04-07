using CGRuleEngine_TFG.Entities.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.GameSystem.TurnSystem
{
    public class Turn
    {
        public Player ActivePlayer { get; set; }
        public List<IPhase> Phases { get; set; } = new List<IPhase>();
        public IPhase CurrentPhase { get; set; }
        public int CurrentPhaseIndex { get; set; }

        // Constructor that initializes the properties of the Turn class with default values
        public Turn(Player player, List<IPhase> phases)
        {
            this.ActivePlayer = player;
            this.Phases = phases;
            this.CurrentPhase = null; // The current phase will be initialized when the turn starts, it is set to null for now
        }

        // Method to start the turn, this method checks if the turn has at least one phase before starting the turn. If the turn has at least one phase, it sets the current phase index to 0 and initializes the current phase with the first phase in the list of phases
        public void Start()
        {
            if (Phases.Count == 0)
            {
                throw new InvalidOperationException("The turn must have at least one phase to start");
            }
            this.CurrentPhaseIndex = 0;
            this.CurrentPhase = Phases[CurrentPhaseIndex];
            this.CurrentPhase.OnEnter();
        }

        // Method to move to the next phase of the turn, this method checks if the current phase index is less than the last index of the list of phases before moving to the next phase. If the current phase index is less than the last index, it calls the OnExit method of the current phase, increments the current phase index, updates the current phase with the next phase in the list of phases and calls the OnEnter method of the new current phase
        public void NextPhase()
        {
            if (CurrentPhaseIndex >= Phases.Count - 1)
            {
                throw new InvalidOperationException("The turn has already reached the last phase, cannot move to the next phase");
            }
            this.CurrentPhase.OnExit();
            this.CurrentPhaseIndex++;
            this.CurrentPhase = Phases[CurrentPhaseIndex];
            this.CurrentPhase.OnEnter();
        }

        // Method to get the current phase of the turn, this method returns the current phase of the turn
        public IPhase GetCurrentPhase()
        {
            return this.CurrentPhase;
        }
    }
}
