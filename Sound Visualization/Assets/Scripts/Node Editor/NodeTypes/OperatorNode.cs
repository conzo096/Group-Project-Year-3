using UnityEngine;
using System;
namespace NodeEditor
{
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

        public float modifier;
        public float output;
        public Operators currentOperator;
        public OperatorNode(Rect r, string name, int index)
        {
            this.rectangle = r;
            this.nodeName = name;
            id = index;
        }

        public OperatorNode()
        {
        }

        public void CalculateOutput()
        {
            switch (currentOperator)
            {
                case Operators.Add:
                    this.output = (float)this.value + this.modifier;
                    break;
                case Operators.Divide:
                    if (modifier != 0)
                        this.output = (float)this.value / this.modifier;
                    break;
                case Operators.Multiply:
                    this.output = (float)this.value * this.modifier;
                    break;
                case Operators.Subtract:
                    this.output = (float)this.value - this.modifier;
                    break;
                case Operators.Power:
                    this.output = Mathf.Pow((float)this.value, this.modifier);
                    break;
                default:
                    this.output = (float)this.value * this.modifier;
                    break;

                //Debug.Log(this.output);
            }
        }

        public void UpdateState(Operators currentOperator)
        {
            this.currentOperator = currentOperator;
        }
    }
}
