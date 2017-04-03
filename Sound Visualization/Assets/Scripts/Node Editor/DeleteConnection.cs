using System;
using UnityEngine;

namespace NodeEditor
{
    // This class is used to store the mouse position and the name of the callback request.
    public class DeleteConnection
    {
        public Node firstNode;
        public Node secondNode;

        public DeleteConnection() { }

        public DeleteConnection(Node first, Node second)
        {
            firstNode = first;
            secondNode = second;
        }
    }
}
