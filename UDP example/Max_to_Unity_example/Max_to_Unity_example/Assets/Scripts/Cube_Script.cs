/* Last edited: 06/02/2017
 * This is the main script of the project. It connects the values to the game object in the scene. This needs to be attached to the game object.
 */
using UnityEngine;
using System.Collections;


public class Cube_Script : MonoBehaviour
{
    // Holds the list of all the attributes that can be changed. 
    // Static as the values should be the same regardless of where it is accessed.
    static ModifiableAttributes attributeList;


    //OSC variables
    public string RemoteIP = /*"146.176.164.4";*/ "127.0f.0.1f"; // signifies a local host (if testing locally
    public int SendToPort = 9000; //the port you will be sending from
    public int ListenerPort = 8050; //the port you will be listening on
    public Transform controller;
    private Osc handler;

    //VARIABLES YOU WANT TO BE ANIMATED - now moved to modifibaleAttributes.cs
   // private float zRot = 0; //the rotation around the z axis
    //private float allScale = 0; //the scale around all axis

    private GameObject go1;
    private object msgValue;            //value passed in via OSC
    private int reset;

 

    public void Start()
    {
        //Initializes on start up to listen for messages
        //make sure this game object has both UDPPackIO and OSC script attached
        attributeList = new ModifiableAttributes();
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
       
    }


    public void Update()
    {
        // Constantly updated. This can be improved by checking for differences, then applying new attributes if they are different.
        // go1.transform.Rotate(zRot, zRot, zRot);
        // go1.transform.localScale = new Vector3(allScale, allScale, allScale);
        go1.transform.Rotate(attributeList.Rotation);
        go1.transform.localScale = attributeList.Scale;

    }


    // msgAddress is a poor variable name. It is actually what musical parameter (e.g. pitch, frequency etc)
    public void AllMessageHandler(OscMessage oscMessage)
    {
        // Send message to reroute message.
        Reroutemessage newMessage = new Reroutemessage();
        newMessage.ConvertOSCMessage(oscMessage);

        string msgAttribute = newMessage.Attribute;
        object value = newMessage.AttributeValue;

        //FUNCTIONS YOU WANT CALLED WHEN A SPECIFIC MESSAGE IS RECEIVED - these get sent to variables and functions before the update, i think...
        switch (msgAttribute)
        {
            case "/Rotate":
                attributeList.Rotation = new Vector3((float)msgValue, (float)msgValue, (float)msgValue);
                Debug.Log("Rotate msgValue is " + attributeList.Rotation);  
                break;
            case "/Scale":
                attributeList.Scale = new Vector3((float) msgValue, (float)msgValue, (float)msgValue);
                Debug.Log("Scale msgValue is " + attributeList.Scale ); 
                break;
            default:
                Debug.Log(msgAttribute + " does not exist...");
                break;
               
        }
    }

}
