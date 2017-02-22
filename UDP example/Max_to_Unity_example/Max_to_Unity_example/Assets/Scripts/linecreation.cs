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
            //  startPoint.x = Screen.width - startPoint.x;
            startPoint.y = Screen.height - startPoint.y;
            endPoint.z = 0;
            mouseDown = true;
        }
        // If left button has been released.
        if (Input.GetMouseButtonUp(0) == true && mouseDown == true)
        {
            mouseDown = false;
            endPoint = Input.mousePosition;
            //  endPoint.x = Screen.width - endPoint.x;
            endPoint.y = Screen.height - endPoint.y;
            endPoint.z = 10;
            DrawLine(startPoint, endPoint, Color.cyan);

        }

    }

//    void OnGUI()
//    {
//    //// If left button has been pressed down.
//    if (Input.GetMouseButtonDown(0) == true && mouseDown == false)
//    {
//        startPoint = Input.mousePosition;
//        mouseDown = true;
//    }
//    // If left button has been released.
//    if (Input.GetMouseButtonUp(0) == true && mouseDown == true)
//    {
//        mouseDown = false;
//        endPoint = Input.mousePosition;
//        // DrawLine(startPoint, endPoint, Color.cyan);
//        DrawGuiLine((Vector2)startPoint, (Vector2)endPoint);
//    }
//}

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
        Texture2D lineTex = new Texture2D(1,1);
      


    //    myLine.AddComponent<Collider2D>();
    ////    myLine.AddComponent<LineRenderer>();
    //    // Populate the line renderer with correct information.
        
        
    //    //LineRenderer lr = myLine.GetComponent<LineRenderer>();
    //    // Allow user to change this material?
    //    lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
    //    lr.SetColors(color, color);
    //    lr.SetWidth(1, 1);
    //    // Add start and end vertices.
    //    lr.SetPosition(0, start);
    //    lr.SetPosition(1, end);
    //    // Increment the line counter.
        lineCounter++;
    }
    Vector2 setPoint(Vector2 point)
    {
        point.x = (int)point.x;
        point.y = Screen.height - (int)point.y;
        return point;
    }


    void DrawGuiLine(Vector2 pointA, Vector2 pointB)
    {
        pointA = setPoint(pointA);
        pointB = setPoint(pointB);
        float length = (pointB - pointA).magnitude;
        Texture2D lineTex = new Texture2D(1, 1);
        Matrix4x4 matrixBackup = GUI.matrix;
        float width = 8.0f;
        GUI.color = Color.red;
        float angle = Mathf.Atan2(pointB.y - pointA.y, pointB.x - pointA.x) * 180f / Mathf.PI;

        GUIUtility.RotateAroundPivot(angle, pointA);
        GUI.DrawTexture(new Rect(pointA.x, pointA.y, length, width), lineTex);
        GUI.matrix = matrixBackup;
        Debug.Log(pointA);
        Debug.Log(pointB);
    }
}