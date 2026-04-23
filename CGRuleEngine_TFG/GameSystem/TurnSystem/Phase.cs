using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.GameSystem.TurnSystem
{
    public abstract class Phase
    {
        public string Name { get; set; }
        public List<Phase> SubPhases { get; set; } = new List<Phase>();

        protected Phase(string name)
        {
            this.Name = name;
        }

        // Method to be called when the phase is entered
        public virtual void OnEnter(Game game)
        {

        }

        // Method to be called when the phase is exited
        public virtual void OnExit(Game game)
        {

        }
    }
}
