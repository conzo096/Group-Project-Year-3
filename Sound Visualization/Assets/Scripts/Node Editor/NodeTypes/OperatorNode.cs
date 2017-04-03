/* OperatorNode, used to modify a value circulating in the system by applying math operations to it. */
using UnityEngine;
using System;
namespace NodeEditor
{
    // All possible math operations
    public enum Operators
    {
        Add = 0,
        Subtract = 1,
        Divide = 2,
        Multiply = 3,
        Power = 4
    }
    // Operator node, receives a value, modifies it and outputs it to other nodes.
    [Serializable]
    public class OperatorNode : Node
    {
        // The user specified value to modify by
        public float modifier;
        // The resulting output
        public float output;
        // Current math operation to be used
        public Operators currentOperator;
        public OperatorNode(Rect r, string name, int index)
        {
            rectangle = r;
            nodeName = name;
            id = index;
        }

        // Default constructor
        public OperatorNode()
        {
        }

        // Calculates the output
        public void CalculateOutput()
        {
            switch (currentOperator)
            {
                // Add
                case Operators.Add:
                    output = value + modifier;
                    break;
                // Divide
                case Operators.Divide:
                    // Avoid dividing by 0
                    if (modifier != 0)
                        output = value / modifier;
                    break;
                // Multiply
                case Operators.Multiply:
                    output = value * modifier;
                    break;
                // Subtract
                case Operators.Subtract:
                    output = value - modifier;
                    break;
                // Power
                case Operators.Power:
                    output = Mathf.Pow(value, modifier);
                    break;
                // Default will just pass on the value
                default:
                    output = value;
                    break;
            }
        }
        // Finds out which operator to use
        public void UpdateState(Operators currentOperator)
        {
            this.currentOperator = currentOperator;
        }
    }
}
