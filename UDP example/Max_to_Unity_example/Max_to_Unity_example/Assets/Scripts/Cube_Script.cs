using UnityEngine;
using System.Collections;


public class Cube_Script : MonoBehaviour
{

    //OSC variables
    public string RemoteIP = /*"146.176.164.4";*/ "127.0f.0.1f"; // signifies a local host (if testing locally
    public int SendToPort = 9000; //the port you will be sending from
    public int ListenerPort = 8050; //the port you will be listening on
    public Transform controller;
    private Osc handler;

    //VARIABLES YOU WANT TO BE ANIMATED
    private float zRot = 0; //the rotation around the z axis
    private float allScale = 1; //the scale around all axis
    private GameObject go1;
    private object msgValue;            //value passed in via OSC
    private int reset;

    LineController lineController;


    public void Start()
    {
        //Initializes on start up to listen for messages
        //make sure this game object has both UDPPackIO and OSC script attached

        UDPPacketIO udp = GetComponent<UDPPacketIO>();
        // Init the user datagram protocal.
        // Can change the listen port for each different input?
        udp.init(RemoteIP, SendToPort, ListenerPort);
        handler = GetComponent<Osc>();
        handler.init(udp);
        handler.SetAllMessageHandler(AllMessageHandler);

        //assigning particle components
        //particles = GetComponent<PlaygroundParticlesC>();
        
        go1 = GameObject.Find("/Cube"); //assign the game object to the go variable
        GameObject.Find("LineManager").GetComponent<LineController>();
    }


    public void Update()
    {
        // Constantly updated. This can be improved by checking for differences, then applying new attributes if they are different.
        go1.transform.Rotate(zRot, zRot, zRot);
        go1.transform.localScale = new Vector3(allScale, allScale, allScale);

    }


    public void AllMessageHandler(OscMessage oscMessage)
    {
    //   var msgString = Osc.OscMessageToString(oscMessage);
        var msgAddress = oscMessage.Address; //the message parameters
        msgValue = oscMessage.Values[0]; //the message value
                                         //    Debug.Log(msgString); //log the message and values coming from OSC 
                                         //Debug.Log(msgAddress); //log the message coming from OSC

        //FUNCTIONS YOU WANT CALLED WHEN A SPECIFIC MESSAGE IS RECEIVED - these get sent to variables and functions before the update, i think...

        object[] newMessage = new object[2];
        // 0 is the address.
        newMessage[0] = msgAddress;
        newMessage[1] = msgValue;

        // Check line manager.
        msgAddress = lineController.FindMeshParameter(msgAddress);
        switch (msgAddress)
        {
           
            //default:
            case "/Rotate":
                zRot = (float)msgValue;
                Debug.Log("Rotate msgValue is " + zRot);
                break;
            case "/Scale":
                allScale = (float)msgValue;
                Debug.Log("Scale msgValue is " + allScale);
                break;
        }



    }

}
