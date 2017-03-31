using System;
using UnityEngine;

namespace Assets.Scripts.Node_Editor
{
    [Serializable]
    public class AudioNode : Node
    {
        public AudioNode()
        {
            nodeName = "Audio";
            value = 0;
        }

        public AudioNode(string name)
        {
            nodeName = name;
            value = 0;
        }

        public AudioNode(Rect rec)
        {
            rectangle = rec;
        }
        public AudioNode(Rect rec, string name, int index) : this(rec)
        {
            nodeName = name;
            id = index;

        }

        // Update value from osc message.
        public override void UpdateValues()
        {
            //value = (float)value+1;
            // Temporarily create a random num (for testing purposes)
            //value = Random.Range(0f, 1f);
        }
    }
}
