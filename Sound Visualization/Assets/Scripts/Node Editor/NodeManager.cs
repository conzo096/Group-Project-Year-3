using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Assets.Scripts.Node_Editor
{
    // purpose of this class to hold hold all the data for it to be serialized.
    [DataContract]
    public class NodeManager
    {
        // Object field for controller node
        [DataMember]
        object fromObjectField = new object();
        [DataMember]
        Operators display = Operators.Multiply;
        [DataMember]
        public int uniqueNodeId = 0;
        // List of rectangle nodes.
        [DataMember]
        public List<Node> windows = new List<Node>();
        // Temp List which holds which two nodes are to be connected.
        [DataMember]
        public List<int> windowsToAttach = new List<int>();
        // IDS of connected nodes.
        [DataMember]
        public List<int> attachedWindows = new List<int>();

    }
}
