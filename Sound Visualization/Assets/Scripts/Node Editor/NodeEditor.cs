using UnityEngine;
using UnityEditor;
using System.Collections.Generic;



public class Node
{
    // Background rectangle, visually holds nodes info.
    public Rect rectangle = new Rect();
    // ID of this node.
    public int id;
    // Value that is being passed through system.
    public float value;

    public string name;

    // Constructors.
    public Node()
    {

    }
    public Node(Rect r)
    {
        rectangle = r;
    }

    public Node(Rect r, string name)
    {
        rectangle = r;
        this.name = name;
    }
    public Node(int inId)
    {
        id = inId;
    }

}

public class vsNode : Node
{
    private GameObject visual;

    private NoiseRingController controller;

    

    public vsNode(Rect r, string tag, string name)
    {
        rectangle = r;
        visual = GameObject.FindGameObjectWithTag(tag);
        controller = visual.GetComponent<NoiseRingController>();
        this.name = name;
    }

    public void UpdateValue()
    {
        switch (this.name)
        { 
            case "yPosition":
                controller.yPositionModulator = this.value;
                break;
            case "xRotation":
                controller.xRotationModulator = this.value;
                break;
            case "yScale":
                controller.yScaleModulator = this.value;
                break;
        }
        controller.yPositionModulator = this.value;   
    }

}

public class NodeEditor : EditorWindow
{
    // List of rectangle nodes.
    List<Node> windows = new List<Node>();

    // Temp List which holds which two nodes are to be connected.
    List<int> windowsToAttach = new List<int>();

    // IDS of connected nodes.
    List<int> attachedWindows = new List<int>();

    int newID = 0;

    int tempCount = 0;
    // public Node node = new Node();

    [MenuItem("Window/Node Editor")]
    static void ShowEditor()
    {
        NodeEditor editor = GetWindow<NodeEditor>();
    }

    void Update()
    {
        // Make sure all visual nodes are updating their values
        for (int i = 0; i < windows.Count; i++)
        {
            // Give each node a random value to test things out
            windows[i].value = Random.Range(0f, 1f);
            // Random.seed++;
            // Check if types are of vsNode
            if (windows[i].GetType() == typeof(vsNode))
            {
                // Cast and call update method
                vsNode temp = (vsNode)windows[i];
                temp.UpdateValue();
            }
        }
    }

    void OnGUI()
    {
        // If windowsToAttach is full, add to connected nodes and reset.
        if (windowsToAttach.Count >= 2)
        {
            attachedWindows.Add(windowsToAttach[0]);
            attachedWindows.Add(windowsToAttach[1]);
            // Reset windowsToAttach.
            windowsToAttach = new List<int>();

        }

        if (attachedWindows.Count >= 2)
        {

            for (int i = 0; i < attachedWindows.Count; i += 2)
            {

                DrawNodeCurve(windows[attachedWindows[i]].rectangle, windows[attachedWindows[i + 1]].rectangle);

            }
        }

        BeginWindows();

        // If button is pressed, create new node.
        if (GUILayout.Button("Create Node"))
        {
            windows.Add(new Node(new Rect(10, 10, 100, 100), "Audio"));
            windows[windows.Count - 1].id = newID;
            newID++;
        }

        if (GUILayout.Button("Create Visual Node"))
        {
            switch (tempCount)
            { 
                case 0:
                    windows.Add(new vsNode(new Rect(10, 10, 100, 100), "NoiseRing", "yPosition"));
                    tempCount++;
                    break;
                case 1:
                    windows.Add(new vsNode(new Rect(10, 10, 100, 100), "NoiseRing", "xRotation"));
                    tempCount++;
                    break;
                case 2:
                    windows.Add(new vsNode(new Rect(10, 10, 100, 100), "NoiseRing", "yScale"));
                    tempCount++;
                    break;
            }
            //windows.Add(new vsNode(new Rect(10, 10, 100, 100), "NoiseRing", "Visual"));
            windows[windows.Count - 1].id = newID;
            newID++;
        }

        // For each window, draw window.
        for (int i = 0; i < windows.Count; i++)
        {

            windows[i].rectangle = GUI.Window(i, windows[i].rectangle, DrawNodeWindow, windows[i].name + i );

        }

        EndWindows();
    }


    void DrawNodeWindow(int id)
    {


        if (GUILayout.Button("Attach"))
        {
            if (windowsToAttach.Count < 2)
            {
                // Avoid duplicates
                if (windowsToAttach.Contains(id))
                {
                    // Do nothing
                }
                else
                {
                    windowsToAttach.Add(id);
                }
            }

            Debug.Log(windowsToAttach.Count);

        }
        GUILayout.TextArea(windows[id].id.ToString());
        GUI.DragWindow();
    }


    void DrawNodeCurve(Rect start, Rect end)
    {
        Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
        Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;
        Color shadowCol = new Color(0, 0, 0, 0.06f);

        for (int i = 0; i < 3; i++)
        {
            // Draw a shadow
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
        }

        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
    }
}
