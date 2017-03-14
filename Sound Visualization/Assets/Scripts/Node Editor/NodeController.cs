using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeController : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {
    

        if (Input.GetKeyDown("1"))
        {
            // Create game object that represents node. - It will have a rectangle background.
         
            GameObject newNode = GameObject.CreatePrimitive(PrimitiveType.Quad);
            newNode.AddComponent<BaseNode>();
        }

        if (Input.GetKeyDown("2"))
        {
            // Create game object that represents node. - It will have a rectangle background.

            GameObject newNode = GameObject.CreatePrimitive(PrimitiveType.Quad);
            newNode.AddComponent<AudioNode>();
        }

        if (Input.GetKeyDown("3"))
        {
            // Create game object that represents node. - It will have a rectangle background.

            GameObject newNode = GameObject.CreatePrimitive(PrimitiveType.Quad);
            newNode.AddComponent<VisualNode>();
        }
    }
}
