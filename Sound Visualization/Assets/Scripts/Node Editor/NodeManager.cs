using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NodeEditor
{
    // purpose of this class to hold hold all the data for it to be serialized.
    public class NodeManager
    {
        // Object field for controller node
        object fromObjectField = new object();
        Operators display = Operators.Multiply;
        public int uniqueNodeId = 0;
        // List of rectangle nodes.
        public List<Node> windows = new List<Node>();
        // Temp List which holds which two nodes are to be connected.
        public List<int> windowsToAttach = new List<int>();
        // IDS of connected nodes.
        public List<int> attachedWindows = new List<int>();
        // Types of each node
        public List<string> listTypes = new List<string>();

    }
}
