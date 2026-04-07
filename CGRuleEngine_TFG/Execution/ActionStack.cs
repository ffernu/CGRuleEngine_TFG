using CGRuleEngine_TFG.GameSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Execution
{
    public class ActionStack
    {
        public Stack<StackEntry> Entries = new Stack<StackEntry>();

        // Push a new entry onto the stack
        public void Push(StackEntry entry)
        {
            this.Entries.Push(entry);
        }

        // Pop the top entry from the stack and resolve it
        public void Pop()
        {
            StackEntry entry = this.Entries.Pop();
            Game gameState = null; // TODO: Get the current game state from somewhere
            entry.Resolve(gameState);
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
