using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class linecreation : MonoBehaviour
{
    private bool mouseDown = false;
    Vector3 startPoint;
    Vector3 endPoint;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) == true && mouseDown == false)
        {
            startPoint = Input.mousePosition;
            mouseDown = true;
        }
        if (Input.GetMouseButtonUp(0) == true && mouseDown == true)
        {
            endPoint = Input.mousePosition;
            mouseDown = false;
            DrawLine(startPoint, endPoint, Color.cyan);
        }


        //Debug.Log(Input.mousePosition);

    }

    void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        // Line needs to be drawn infront of camera. 

        // Calculate difference between points.
     //   Vector3 vstart = Camera.main.WorldToScreenPoint(start);
      //  Vector3 vend = Camera.main.WorldToScreenPoint(end);
        float dist = Vector3.Distance(start, end);
        Debug.Log("Length: " + dist);
        GameObject myLine = new GameObject();
        myLine.name = "THISISALINE";
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 1;
        lr.endWidth = 1;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        //lr.useWorldSpace = false;
        myLine.transform.SetParent(this.transform);
        myLine.layer = 5;
    }
}