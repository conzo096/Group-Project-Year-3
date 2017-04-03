using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NodeEditor
{
    // purpose of this class to hold hold all the data for it to be serialized.
    [Serializable]
    public class NodeManager
    {
        // Holds id to use next.
        public int uniqueNodeId = 0;   
        // List of nodes - split up to be serialized.
        public List<VisualNode> viNodes = new List<VisualNode>();
        public List<AudioNode> auNodes = new List<AudioNode>();
        public List<OperatorNode> oNodes = new List<OperatorNode>();
        public List<ControllerNode> cNodes = new List<ControllerNode>();
        public List<MaterialNode> matNodes = new List<MaterialNode>();
        public List<MaxNode> mNodes = new List<MaxNode>();
        public List<RandomGeneratorNode> rNodes = new List<RandomGeneratorNode>();
        // Temp List which holds which two nodes are to be connected.
        public List<int> windowsToAttach = new List<int>();
        // IDS of connected nodes.
        public List<int> attachedWindows = new List<int>();

        public NodeManager(){ }
        // Take node editor and take information to serialize.
        public NodeManager(NodeEditor ne)
        {
            attachedWindows = ne.attachedWindows;
            uniqueNodeId = ne.uniqueNodeId;
            windowsToAttach = ne.windowsToAttach;
            // Save Nodes to correct list.
            for (int i = 0; i < ne.windows.Count; i++)
            {
                if (ne.windows[i] is AudioNode)
                    auNodes.Add((AudioNode)ne.windows[i]);
                if (ne.windows[i] is VisualNode)
                    viNodes.Add((VisualNode)ne.windows[i]);
                if (ne.windows[i] is OperatorNode)
                    oNodes.Add((OperatorNode)ne.windows[i]);
                if (ne.windows[i] is MaterialNode)
                    matNodes.Add((MaterialNode)ne.windows[i]);
                if (ne.windows[i] is ControllerNode)
                    cNodes.Add((ControllerNode)ne.windows[i]);
                if (ne.windows[i] is MaxNode)
                    mNodes.Add((MaxNode)ne.windows[i]);
                if (ne.windows[i] is RandomGeneratorNode)
                    rNodes.Add((RandomGeneratorNode)ne.windows[i]);
            }
        }
    }
}
