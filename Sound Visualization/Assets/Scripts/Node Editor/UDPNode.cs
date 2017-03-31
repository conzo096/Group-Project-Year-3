using System;
using UnityEngine;

namespace Assets.Scripts.Node_Editor
{
    [Serializable]
    public class MaxNode : Node
    {
        // Port to listen on
        public int inPort = 8050;
        // Ip audio is coming from?
        public string incomingIp;
        // Sending to port? - not used by us.
        public int outPort = 9000;
        // Listens for messages - need a seperate one per node
        public Osc handler;
        // Value from maxMSP
        public float maxValue;

        public MaxNode()
        {
            UDPPacketIO udp = new UDPPacketIO();
            // Init the user datagram protocal.
            // Can change the listen port for each different input?
            udp.init(incomingIp, outPort, inPort);
            handler = new Osc();
            handler.init(udp);
            handler.SetAllMessageHandler(AllMessageHandler);
        }
        public MaxNode(Rect rec, string name, int index) : this()
        {
            rectangle = rec;
            nodeName = name;
            id = index;

        }
        // msgAddress is a poor variable name. It is actually what musical parameter (e.g. pitch, frequency etc)
        public void AllMessageHandler(OscMessage oscMessage)
        {
            //Debug.Log(oscMessage.Address);
            maxValue = (float)oscMessage.Values[0];

        }
    }

}
