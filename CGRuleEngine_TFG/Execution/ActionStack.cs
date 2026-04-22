using CGRuleEngine_TFG.GameSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Execution
{
    public class ActionStack
    {
        public Stack<StackEntry> Entries = new Stack<StackEntry>();
        public IGameState gameState { get; set; }

        // Constructor that takes an IGameState object as a parameter and initializes the gameState property of the ActionStack class with the provided game state
        public ActionStack(IGameState gameState)
        {
            this.gameState = gameState;
        }

        // Push a new entry onto the stack
        public void Push(StackEntry entry)
        {
            this.Entries.Push(entry);
        }

        // Pop the top entry from the stack and resolve it
        public void Pop()
        {
            StackEntry entry = this.Entries.Pop();
            entry.Resolve(this.gameState);
        }

        // Check if the stack is empty
        public bool IsEmpty()
        {
            return this.Entries.Count == 0;
        }

        // Peek at the top entry of the stack without popping it
        public StackEntry Peek()
        {
            return this.Entries.Peek();
        }
    }
}
