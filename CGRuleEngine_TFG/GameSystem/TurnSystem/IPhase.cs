using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.GameSystem.TurnSystem
{
    public interface IPhase
    {
        public String name { get; set; }
        public List<IPhase> SubPhases { get; set; }

        public void OnEnter()
        {
            // This method is called when the phase is entered, it can be used to perform any setup or initialization for the phase
        }

        public void OnExit()
        {
            // This method is called when the phase is exited, it can be used to perform any cleanup or finalization for the phase
        }

        // TODO: rest of the logic + pensar si esto es una interfaz o no :(

    }
}
