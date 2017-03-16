using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/* TODO LIST.
 * Visual, Audio and operator nodes.
 * Apply osc manager onto this script.
 * Create phyiscal connections between nodes.
 */ 



// Base node class.
public abstract class Node
{
    // Background rectangle, visually holds nodes info.
    public Rect rectangle = new Rect();
    // what type of node is this, scale, pitch, plus operator etc.
    public string nodeName;
    // Value that is being passed through system.
    public float value;
    // Constructors. FIX CHAINING
    public Node()
    {
    }
    public Node(Rect r)
    {
        rectangle = r;
    }
    public Node(string name)
    {
        nodeName = name;
    }
    public Node(Rect r, string name) : this(name)
    {
        rectangle = r;
    }

    // This method should be called every frame to get the most recent value.
    public virtual void UpdateValue() { Debug.Log("Error, this should be over-written!"); }

}

public class AudioNode : Node
{
    public AudioNode()
    {
        nodeName = "Audio";
    }

    public AudioNode(string name)
    {
        nodeName = name;
    }

    public AudioNode(Rect rec)
    {
        rectangle = rec;
    }
    public AudioNode(Rect rec, string name) : this(rec)
    {
        nodeName = name;
    }

    // Update value from osc message.
    public override void UpdateValue()
    {
        value = 5;
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

   // public Node node = new Node();

    [MenuItem("Window/Node Editor")]
    static void ShowEditor()
    {
        NodeEditor editor = GetWindow<NodeEditor>();
    }

    // called when an right-click option is selected.
    void Callback(object obj)
    {
        string nodeRequested = obj.ToString();
        switch (nodeRequested)
        {
            /*
             * Audio Nodes.
             */
            case "Scale":
                windows.Add(new AudioNode(new Rect(10, 10, 100, 100), "Scale"));
                break;
            case "Volume":
                windows.Add(new AudioNode(new Rect(10, 10, 100, 100), "Volume"));
                break;
            case "Pitch":
                windows.Add(new AudioNode(new Rect(10, 10, 100, 100), "Pitch"));
                break;
        }
    }


    void OnGUI()
    {
        // If windowsToAttach is full, add to connected nodes and reset.
        if (windowsToAttach.Count >=2)
        {
            // Check if nodes are already connected first!!!

            attachedWindows.Add(windowsToAttach[0]);
            attachedWindows.Add(windowsToAttach[1]);
            // Reset windowsToAttach.
            windowsToAttach = new List<int>();
        }
        BeginWindows();
        // For each window, draw window.
        for (int i = 0; i < windows.Count; i++)
        {
            windows[i].rectangle = GUI.Window(i, windows[i].rectangle, DrawNodeWindow, windows[i].nodeName);
        }

        // Draw connected between connected nodes.
        if (attachedWindows.Count >= 2)
        {
            for (int i = 0; i < attachedWindows.Count; i += 2)
            {
                DrawNodeCurve(windows[attachedWindows[i]].rectangle, windows[attachedWindows[i + 1]].rectangle);              
            }
        }


        // Draw right click menu and populate list. Also check for right click event.
        Event currentEvent = Event.current;
        if (currentEvent.type == EventType.ContextClick)
        {
            Vector2 mousePos = currentEvent.mousePosition;
            // Now create the menu, add items and show it
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("VisualNodes/"), false, Callback, "V");
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Operators/"), false, Callback, "O");
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("AudioNodes/Scale"), false, Callback, "Scale");
            menu.AddItem(new GUIContent("AudioNodes/Pitch"), false, Callback, "Pitch");
            menu.AddItem(new GUIContent("AudioNodes/Volume"), false, Callback, "Volume");
            menu.ShowAsContext();
            currentEvent.Use();
        }

        EndWindows();
    }


    //void Update()
    //{
    //    if (EditorApplication.isPlaying && !EditorApplication.isPaused)
    //    {
        
    //    }
    //}


    // Draws the node window.
    void DrawNodeWindow(int id)
    {
        if (GUILayout.Button("Attach"))
        {
            if (windowsToAttach.Count < 2)
            {
                windowsToAttach.Add(id); 
            }
        }
        GUILayout.TextArea(windows[id].nodeName);
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
