using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class linecreation : MonoBehaviour
{
    // Mouse being pressed down?
    private bool mouseDown = false;
    // Start point of the line.
    Vector3 startPoint;
    // End point of the line.
    Vector3 endPoint;
    // How many lines have been drawn.
    int lineCounter = 0;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // If left button has been pressed down.
        if (Input.GetMouseButtonDown(0) == true && mouseDown == false)
        {
            startPoint = Input.mousePosition;
            mouseDown = true;
        }
        // If left button has been released.
        if (mouseDown == true)
        {
            endPoint = Input.mousePosition;
            DrawLine(startPoint, endPoint, Color.cyan);

        }
        if (Input.GetMouseButtonUp(0) == true)
            mouseDown = false;
    }

    // This method will draw a new line object on the user interface.
    void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject myLine = new GameObject();
        // Change the name of this later.
        myLine.name = "line (" + lineCounter + ")";
        myLine.transform.SetParent(this.transform);
        // Set layer of line to the UI layer.
        myLine.layer = 5;
        // Add neccessary components to the line.
        myLine.AddComponent<LineConnection>();
        myLine.AddComponent<Collider2D>();
        myLine.AddComponent<LineRenderer>();

        // Populate the line renderer with correct information.
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        // Allow user to change this material?
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 1;
        lr.endWidth = 1;
        // Add start and end vertices.
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        // Increment the line counter.
        lineCounter++;
    }
}