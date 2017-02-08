using UnityEngine;
using System.Collections;
/* RingController.cs by Emmanuel Miras. Last edited 26/01/17
 * Attach to GameObject with a particle system. 
 * Controls a ring shaped particle system
*/

public class RingController : MonoBehaviour
{
    #region 1

    //OSC variables
    public string RemoteIP = /*"146.176.164.4";*/ "127.0f.0.1f"; // signifies a local host (if testing locally
    public int SendToPort = 9000; //the port you will be sending from
    public int ListenerPort = 8050; //the port you will be listening on
    public Transform controller;
    private Osc handler;

    //VARIABLES YOU WANT TO BE ANIMATED
    private float zRot = 0; //the rotation around the z axis
    private float allScale = 0; //the scale around all axis

    private object msgValue;            //value passed in via OSC
    private int reset;

    [Range(0, 5)]
    public float positionModulator;
    [Range(10, 50)]
    public float ringRadius;
    [Range(1, 10)]
    public float ringLifeTime;
    [Range(1, 10)]
    public float scaler;

    #endregion
    [Range(1, 10)]
    // Number of rings
    public int ringInstances;
    // The ring particle system prefab
    public GameObject ring;
    // The particle system attached to the GameObject
    private ParticleSystem[] ps;
    // The shape of the particle system
    private ParticleSystem.ShapeModule[] psShape;

    //private Transform[] transforms;

    
    //private GameObject ringInstance;
	// Use this for initialization
	void Start ()
    {
        #region 2

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


        #endregion
        // Initialize
        ps = GetComponentsInChildren<ParticleSystem>();
        //transforms = GetComponentsInChildren<Transform>();
        //ringInstance = GetComponentInChildren<GameObject>();
        //getcomponents
        //ringLifeTime = 1f;
        //ringRadius = 10f;
        //ringInstances = 1;
        //CreateRingSetup();
        scaler = 1f;
        for (int i = 0; i < ringInstances; i++)
        {
            //Instantiate(ring);
            GameObject childObject = Instantiate(ring) as GameObject;
            childObject.transform.parent = this.transform;
        }

    }
	
	// Update is called once per frame
	void Update ()
    {
        // Keep getting a reference to the particle systems of the children (TODO: fix, probably costly)
        ps = GetComponentsInChildren<ParticleSystem>();

        // Loop through all particle systems and adjust ring radius
        foreach (ParticleSystem psCurrent in ps)
        {
            var sh = psCurrent.shape;

            sh.radius = ringRadius;//ringRadius;

            psCurrent.startLifetime = ringLifeTime;
        }

        // Add missing rings
        if (transform.childCount < ringInstances)
        {
            // Number of rings to be created
            int instancesToCreate = ringInstances - transform.childCount;
            // Create rings as children
            for (int i = 0; i < instancesToCreate; i++)
            {
                
                GameObject childObject = Instantiate(ring) as GameObject;
                childObject.transform.parent = this.transform;
                int displacement = 0;//5 * transform.childCount;
                childObject.transform.Translate(new Vector3(0f, displacement, 0f));
            }
        }

        // Destroy extra rings
        if (transform.childCount > ringInstances)
        {
            // Number of rings to be deleted
            int instancesToDelete = transform.childCount - ringInstances;
            
            foreach (Transform child in transform)
            {
                if (instancesToDelete == 0)
                {
                    continue;
                }
                else
                {
                    Destroy(child.gameObject);
                    instancesToDelete--;
                }

            }
        }


    }

    // Sets the particle system to emit as a ring, if not already set-up
    void CreateRingSetup()
    {
        
        //GetComponentInChildren<Transform>().
        int i = 0;
        // Loop through all particlesystems
        foreach (ParticleSystem psCurrent in ps)
        {
            //transforms[i].rotation = Quaternion.Euler(new Vector3(-90f, 0f, 0f));
            //psShape.SetValue(psCurrent.shape, i);
            var sh = psCurrent.shape;
            sh.enabled = true;
            sh.shapeType = ParticleSystemShapeType.ConeShell;
            sh.angle = 0f;
            psCurrent.startLifetime = 1f;
            var emission = psCurrent.emission;
            emission.rate = 100f;

            i++;
        }
    }

    public void AllMessageHandler(OscMessage oscMessage)
    {
        //   var msgString = Osc.OscMessageToString(oscMessage);
        var msgAddress = oscMessage.Address; //the message parameters
        msgValue = oscMessage.Values[0]; //the message value
        //    Debug.Log(msgString); //log the message and values coming from OSC 
        //Debug.Log(msgAddress); //log the message coming from OSC

        //FUNCTIONS YOU WANT CALLED WHEN A SPECIFIC MESSAGE IS RECEIVED - these get sent to variables and functions before the update, i think...
        switch (msgAddress)
        {
            //default:
            case "/Rotate":
                zRot = (float)msgValue;
                Debug.Log("Rotate msgValue is " + zRot);
                break;
            case "/Scale":
                ringRadius = Mathf.Abs((float)msgValue * scaler);
                ringLifeTime = Mathf.Abs((float)msgValue * scaler);
                Debug.Log("Scale msgValue is " + allScale);
                break;

        }



    }
}
