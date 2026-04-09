using System;
using System.Collections.Generic;
using System.Text;

namespace CGRuleEngine_TFG.Execution
{
    public interface ITargetable
    {
        // Method to try to get a property value by its name, it checks if the property exists and if it can be cast to the specified type, returning true if successful and false otherwise
        bool TryGetProperty<T>(string propertyName, out T value);

        // Method to set a property value by its name, it allows setting a property value without needing to know the specific type of the property, as it accepts an object as the value
        void SetProperty(string propertyName, object value);

        // Method to check if the targetable object has a specific tag, it checks if the Tags collection contains the specified tag and returns true if it does, false otherwise
        bool HasTag(string tag);

        // Method to add a tag to the targetable object, it adds the specified tag to the Tags collection if it is not already present
        void AddTag(string tag);

        // Method to remove a tag from the targetable object, it removes the specified tag from the Tags collection if it is present
        void RemoveTag(string tag);
    }
}
