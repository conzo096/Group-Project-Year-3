using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Node_Editor
{
    // Node for testing
    public class RandomGeneratorNode : Node
    {
        public RandomGeneratorNode()
        {
            nodeName = "Random";
            value = 0;
        }

        public RandomGeneratorNode(string name)
        {
            nodeName = name;
            value = 0;
        }

        public RandomGeneratorNode(Rect rec)
        {
            rectangle = rec;
        }
        public RandomGeneratorNode(Rect rec, string name, int index) : this(rec)
        {
            nodeName = name;
            id = index;
        }

        // Update value randomly.
        public override void UpdateValues()
        {
            // Temporarily create a random num (for testing purposes)
            value = UnityEngine.Random.Range(0f, 1f);
        }
    }
}
