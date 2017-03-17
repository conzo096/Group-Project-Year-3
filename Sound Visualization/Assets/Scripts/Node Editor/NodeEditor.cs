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
    public virtual void UpdateValues() { Debug.Log("Error, this should be over-written!"); }

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
    public override void UpdateValues()
    {
        value = 5;
    }
}

public class vsNode : Node
{
    public bool[] componentsChecked;
    public Object visual;
    public Component[] components;
    private NoiseRingController controller;
    public string gameObjectTag = "GameObject Tag";

    public vsNode(Rect r, string tag, string name)
    {
        rectangle = r;
        visual = GameObject.FindGameObjectWithTag(tag);
        //controller = visual.GetComponent<NoiseRingController>();
        this.nodeName = name;
    }

    public void UpdateValue()
    {
        switch (this.nodeName)
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
        //controller.yPositionModulator = this.value;   
    }

    public void Test()
    {
        if (visual != null)
        {
            GameObject temp = (GameObject)visual;
            //Debug.Log(temp.name);
            //transform = temp.GetComponent<Transform>();
            components = temp.GetComponents<Component>();
            foreach (Component component in components)
            {
                Debug.Log(component.GetType());
            }
            //Debug.Log(transform);
        }
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
  
    [MenuItem("Window/Node Editor")]
    static void ShowEditor()
    {
        NodeEditor editor = GetWindow<NodeEditor>();
    }

   // called when an right-click option is selected.
    void Callback(object obj)
    {
        //Event currentEvent = Event.current;
       // Debug.Log(currentEvent.mousePosition);
        Vector2 mousePos;// = currentEvent.mousePosition;
        mousePos = new Vector2(10, 10);
        string nodeRequested = obj.ToString();
        switch (nodeRequested)
        {
            /*
             * Audio Nodes.
             */
            case "Amplitude":
                windows.Add(new AudioNode(new Rect(mousePos.x, mousePos.y, 100, 100), "Amplitude"));
                break;
            case "Volume":
                windows.Add(new AudioNode(new Rect(mousePos.x, mousePos.y, 100, 100), "Volume"));
                break;
            case "Pitch":
                windows.Add(new AudioNode(new Rect(mousePos.x, mousePos.y, 100, 100), "Pitch"));
                break;
            case "yPosition":
                windows.Add(new vsNode(new Rect(mousePos.x, mousePos.y, 200, 300), "NoiseRing", "yPosition"));
                break;
            case "xRotation":
                windows.Add(new vsNode(new Rect(mousePos.x, mousePos.y, 100, 100), "NoiseRing", "xRotation"));
                break;
            case "yScale":
                windows.Add(new vsNode(new Rect(mousePos.x, mousePos.y, 100, 100), "NoiseRing", "yScale"));
                break;
        }
    }

    void Update()
    {
        
    }

    void OnGUI()
    {

        // If windowsToAttach is full, add to connected nodes and reset.
        if (windowsToAttach.Count >= 2)
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

        
        for (int i = 0; i < attachedWindows.Count; i += 2)
        {
            DrawNodeCurve(windows[attachedWindows[i]].rectangle, windows[attachedWindows[i + 1]].rectangle);
        }



        // Draw right click menu and populate list. Also check for right click event.
        Event currentEvent = Event.current;
        if (currentEvent.type == EventType.ContextClick)
        {
            Vector2 mousePos = currentEvent.mousePosition;
            // Now create the menu, add items and show it
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("VisualNodes/"), false, Callback, "V");
            menu.AddItem(new GUIContent("VisualNodes/yPosition"), false, Callback, "yPosition");
            menu.AddItem(new GUIContent("VisualNodes/xRotation"),false, Callback, "xRotation");
            menu.AddItem(new GUIContent("VisualNodes/yScale"), false, Callback, "yScale");
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Operators/"), false, Callback, "O");
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("AudioNodes/Amplitude"), false, Callback, "Amplitude");
            menu.AddItem(new GUIContent("AudioNodes/Pitch"), false, Callback, "Pitch");
            menu.AddItem(new GUIContent("AudioNodes/Volume"), false, Callback, "Volume");
           
            menu.ShowAsContext();
            currentEvent.Use();
        }
        EndWindows();
    }

    // Draws the node window.
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
        }
        windows[id].nodeName = GUILayout.TextArea(" ");


        // Check if types are of vsNode
        if (windows[id].GetType() == typeof(vsNode))
        {
            // Cast and add TextArea
            vsNode temp = (vsNode)windows[id];
            //temp.gameObjectTag = GUILayout.TextArea(temp.gameObjectTag);
            temp.visual = EditorGUILayout.ObjectField(temp.visual, typeof(Object), true);
            temp.Test();

            if (temp.visual != null)
            {
                int i = 0;
                foreach (Component component in temp.components)
                {
                    //Debug.Log(component.GetType());
                    //GUILayout.TextArea(component.GetType().ToString());
                    temp.componentsChecked[i] = EditorGUILayout.Toggle(component.GetType().ToString(), temp.componentsChecked[i]);
                    //if (temp.componentChecked)
                    //    GUILayout.TextArea(component.GetType().ToString());
                    i++;
                }
                //temp.FindGameObject();
            }
        }


        //stringToEdit = GUILayout.TextArea(stringToEdit);

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
