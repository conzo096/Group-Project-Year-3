using UnityEngine;
using System.Collections;

public class LineConnection : MonoBehaviour
{
    // Is it currently connected to anything?
    bool isConnected = true;
    string meshConnection;
    string audioConnection;


	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Get line renderer.
        LineRenderer lr = this.GetComponent<LineRenderer>();

        // If lr is not connected two both parts, isConnected is false.
        if (lr)
            isConnected = false;

        // Check if it is not connected, if it is not then delete object.
        if (!isConnected)
        {
            Destroy(this, 1.5f);
        }
	}
}
