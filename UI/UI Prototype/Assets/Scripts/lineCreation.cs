using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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
    }

    void OnGUI()
    {   

        if (Input.GetMouseButtonDown(0) == true && mouseDown == false)
        {
            startPoint = GUIUtility.ScreenToGUIPoint(Input.mousePosition);

            Debug.Log("Start point:" + startPoint);
            mouseDown = true;
        }
        else if (Input.GetMouseButtonUp(0) == true && mouseDown == true)
        {
            endPoint = GUIUtility.ScreenToGUIPoint(Input.mousePosition);
            Debug.Log("end point:" + endPoint);
            mouseDown = false;
            DrawGuiLine(startPoint, endPoint);
        }
    }



    // This is a temp method to change parameters with a button, will be replaced with line drawing.
    public void ChangePar()
    {

        string[] newLine = new string[2];

        System.Random rnd = new System.Random();
        int x = rnd.Next(2);
        int y = rnd.Next(2);
        string musicPar = "Null";
        string meshPar = "Null";

        // Set musicPar.
        switch (x)
        {
            case 0:
                musicPar = "/Pitch";
            break;
            case 1:
                musicPar = "/Volume";
            break;
        }

        // Set musicPar.
        switch (y)
        {
            case 0:
                meshPar = "/Scale";
                break;
            case 1:
                meshPar = "/Rotate";
                break;
        }

        newLine[0] = musicPar;
        newLine[1] = meshPar;


        GameObject.Find("LineManager").GetComponent<LineController>().AddNewLine(newLine);
     //   Debug.Log(newLine[0]+ ", "+ newLine[1]);
    }


    // Draws the line, just in the incorrect place.
    public void DrawGuiLine(Vector3 start, Vector3 end)
    {

        GameObject lineObj = new GameObject();
        lineObj.name = "LineConnection(" +lineCounter+ ")";
        lineObj.transform.parent = GameObject.Find("Panel").transform;
        lineObj.AddComponent<LineRenderer>();
        lineObj.AddComponent<RectTransform>();
        lineObj.transform.position = new Vector3(0, 0, 0);
        LineRenderer lineObjRend = lineObj.GetComponent<LineRenderer>();
        lineObjRend.SetPosition(0, start);
        lineObjRend.SetPosition(1, end);
        lineCounter++;
    }
}