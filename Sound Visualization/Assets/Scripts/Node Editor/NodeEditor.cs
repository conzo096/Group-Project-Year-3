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

    
    
    // Constructors.
    public Node()
    {

    }
    public Node(Rect r)
    {
        rectangle = r;
    }
    public Node(int inId)
    {
        id = inId;
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

   // public Node node = new Node();

    [MenuItem("Window/Node Editor")]
    static void ShowEditor()
    {
        NodeEditor editor = GetWindow<NodeEditor>();
    }

    void OnGUI()
    {

        // If windowsToAttach is full, add to connected nodes and reset.
        if (windowsToAttach.Count >=2)
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
            windows.Add(new Node(new Rect(10, 10, 100, 100)));
            windows[windows.Count-1].id = newID;
            newID++;    
        }

        // For each window, draw window.
        for (int i = 0; i < windows.Count; i++)
        {
           
            windows[i].rectangle = GUI.Window(i, windows[i].rectangle, DrawNodeWindow, "Window " + i);
   
        }

        EndWindows();
    }


    void DrawNodeWindow(int id)
    {
       

        if (GUILayout.Button("Attach"))
        {
            if (windowsToAttach.Count < 2)
            {
                windowsToAttach.Add(id);
     
            }
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
