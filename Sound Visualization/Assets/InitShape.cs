/* InitShape.cs (Needs a more suitable name) by Conner Weatherston (40167111). Last edited: 24/01/17
 * This class creates the inital sphere for the dynamic object. This is done by creating a new sphere primitive then attaching it the the gameobject.
 * After the new sphere GameObject is deleted. Here, an attempt at scaling the mesh has also been done. Either move this to another script or rename this script.
 */

using UnityEngine;
using System.Collections;

public class InitShape : MonoBehaviour {
    [Range(0, 127)]
    public int currentScale;
    public int previousScale;

    
	// Use this for initialization
	void Start ()
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        gameObject.GetComponent<MeshFilter>().mesh =  sphere.GetComponent<MeshFilter>().mesh;
        Destroy(sphere);
    }
	
	// Update is called once per frame
	void Update ()
    {
        Renderer rend = GetComponent<Renderer>();
        if (currentScale == 0)
            rend.enabled = false;
        else
            rend.enabled = true;
        // Need set and gets for scale?
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (currentScale < 127)
            {
                previousScale = currentScale;
                currentScale++;
            }
                ScaleMesh(currentScale);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (currentScale > 1)
            {
                previousScale = currentScale;
                currentScale--;
            }
                ScaleMesh(currentScale);
        }
    }


    // This method will scale the gameObject mesh by the new scaling provided. This should only be in the range of 1-127

    public bool ScaleMesh(int newScaling)
    {
        // If it not a valid scaling exit the method.
        if (newScaling < 1)
            return false;

        Vector3 csc;
        csc.x = currentScale;
        csc.y = currentScale;
        csc.z = currentScale;
        Vector3 sc;
        sc.x = newScaling;
        sc.y = newScaling;
        sc.z = newScaling;
        // remove current scaling.
        //gameObject.transform.localScale -= csc;
        // Add new scaling.
        gameObject.transform.localScale = sc * 0.1f;
        //Vector3[] newVert = new Vector3[sphere.GetComponent<MeshFilter>().mesh.vertexCount];
        //Vector3[] newVert = gameObject.GetComponent<MeshFilter>().mesh.vertices;
        //for (int i = 0; i < gameObject.GetComponent<MeshFilter>().mesh.vertexCount; i++)
        //{
        //    newVert[i].x *= 1.05f;
        //    newVert[i].y *= 1.05f;
        //    newVert[i].z *= 1.05f;
        //}
        //gameObject.GetComponent<MeshFilter>().mesh.vertices = newVert;


        return true;
    }

}
