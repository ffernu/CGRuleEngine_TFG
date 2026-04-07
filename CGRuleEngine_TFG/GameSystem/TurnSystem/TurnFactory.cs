using CGRuleEngine_TFG.Entities.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.GameSystem.TurnSystem
{
    public class TurnFactory
    {
        public List<IPhase> Phases {  get; set; } = new List<IPhase>();

        // Constructor that initializes the properties of the TurnFactory class with default values
        public TurnFactory(List<IPhase> phases)
        {
            this.Phases = FlattenPhases(phases);
        }

        // Method to create a new turn for the active player, this method takes the active player as a parameter and returns a new Turn object initialized with the active player and the list of phases from the TurnFactory
        public Turn CreateTurn(Player activePlayer)
        {
            Turn turn = new Turn(activePlayer, this.Phases);
            return turn;
        }

        // This method flattens the nested phases into a single list of phases for easier processing
        private List<IPhase> FlattenPhases(List<IPhase> nestedPhases)
        {
            List<IPhase> flatList = new List<IPhase>();

            foreach (var phase in nestedPhases)
            {
                flatList.Add(phase);

                if (phase.SubPhases != null && phase.SubPhases.Count > 0)
                {
                    flatList.AddRange(FlattenPhases(phase.SubPhases));
                }
            }

            return flatList;
        }
    }
}
