using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NodeEditor
{

    // Base node class.
    [Serializable]
    public class Node
    {
        // Identifies this node.
        public int id;
        // Background rectangle, visually holds nodes info.
        public Rect rectangle = new Rect();
        // what type of node is this, scale, pitch, plus operator etc.
        public string nodeName;
        // Value that is being passed through system.
        public object value;
        
        // Constructors. FIX CHAINING
        public Node()
        {
        }
        public Node(Rect r)
        {
            rectangle = r;
        }
        public Node(string name)
        {
            nodeName = name;
        }
        public Node(Rect r, string name, int index) : this(name)
        {
            rectangle = r;
            id = index;
        }

        // This method should be called every frame to get the most recent value.
        public virtual void UpdateValues() { Debug.Log("Error, this should be over-written!"); }

    }
}
