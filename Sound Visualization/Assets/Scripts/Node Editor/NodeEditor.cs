using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

/* TODO LIST.
 * Operator nodes.
 * Apply osc manager onto this script.
 * Delete nodes, delete connections.
 * For visual nodes: break up vectors into seperate x y and z
 * Make sure when connecting nodes audio is on left side and visual on right side
 */



// Base node class.
public abstract class Node
{
    // Background rectangle, visually holds nodes info.
    public Rect rectangle = new Rect();
    // what type of node is this, scale, pitch, plus operator etc.
    public string nodeName;
    // Value that is being passed through system.
    public object value;
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
        value = 0;
    }

    public AudioNode(string name)
    {
        nodeName = name;
        value = 0;
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
        //value = (float)value+1;
        // Temporarily create a random num (for testing purposes)
        //value = Random.Range(0f, 1f);
    }
}

// Node for testing
public class RandomGeneratorNode : Node
{
    public RandomGeneratorNode()
    {
        nodeName = "Random";
        value = 0;
    }

    public RandomGeneratorNode(string name)
    {
        nodeName = name;
        value = 0;
    }

    public RandomGeneratorNode(Rect rec)
    {
        rectangle = rec;
    }
    public RandomGeneratorNode(Rect rec, string name) : this(rec)
    {
        nodeName = name;
    }

    // Update value randomly.
    public override void UpdateValues()
    {
        // Temporarily create a random num (for testing purposes)
        value = Random.Range(0f, 1f);
    }
}

public class MaxNode: Node
{
    // Port to listen on
    public int inPort = 8050;
    // Ip audio is coming from?
    public string incomingIp;
    // Sending to port? - not used by us.
    public int outPort = 9000;
    // Listens for messages - need a seperate one per node
    public Osc handler;
    // Value from maxMSP
    public float maxValue;

    public MaxNode()
    {
        UDPPacketIO udp = new UDPPacketIO();
        // Init the user datagram protocal.
        // Can change the listen port for each different input?
        udp.init(incomingIp, outPort, inPort);
        handler = new Osc();
        handler.init(udp);
        handler.SetAllMessageHandler(AllMessageHandler);
    }
    public MaxNode(Rect rec, string name) : this()
    {
        rectangle = rec;
        nodeName = name;

    }
    // msgAddress is a poor variable name. It is actually what musical parameter (e.g. pitch, frequency etc)
    public void AllMessageHandler(OscMessage oscMessage)
    {
        //Debug.Log(oscMessage.Address);
        maxValue = (float)oscMessage.Values[0];

    }
}

// Controller node, used to create visual nodes based on a given gameobject
public class ControllerNode : Node
{
    //public List<bool> componentsChecked = new List<bool>();
    public Object visual;
    public Dictionary<Component, bool> componentsDictionary = new Dictionary<Component, bool>();//public Component[] components;
    //private NoiseRingController controller;
    public string gameObjectTag = "GameObject Tag";

    public ControllerNode(Rect r, string name)
    {
        rectangle = r;
        //visual = GameObject.FindGameObjectWithTag(tag);
        //controller = visual.GetComponent<NoiseRingController>();
        this.nodeName = name;
    }

    public void Test()
    {
        if (visual != null)
        {
            GameObject temp = (GameObject)visual;
            //Debug.Log(temp.name);
            //transform = temp.GetComponent<Transform>();
            Component[] tempComponents = temp.GetComponents<Component>();
            foreach (Component component in tempComponents)
            {
                if (!(componentsDictionary.ContainsKey(component)))
                {
                    componentsDictionary.Add(component, false);

                }
            }
        }
    }

}

// Visual node, interacts with components of a gameobject provided by the controller node.
public class VisualNode : Node
{
    // Used to track down type of value
    public PropertyInfo propertyInfo;
    public FieldInfo fieldInfo;
    public System.Object compObj;

    // Tracks which properties of a Vector3 to change
    public bool[] Vectors = new bool[3];

    public VisualNode(Rect r, string name, object value)
    {
        rectangle = r;
        this.nodeName = name;
        //this.value = value;
    }

    // Updates the variables of a component
    public void UpdateVisual(Dictionary<PropertyInfo, Component> propertyInfoDictionary)
    {
        
        // Temp value
        //this.value = Random.Range(0f, 1f);//new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        if (this.value != null)
        {

            List<PropertyInfo> propertyInfo = new List<PropertyInfo>(propertyInfoDictionary.Keys);
            for (int i = 0; i < propertyInfo.Count; i++)
            {

                if (propertyInfo[i].Name == this.nodeName)
                {
                    Component comp;
                    propertyInfoDictionary.TryGetValue(propertyInfo[i], out comp);
                    compObj = (System.Object)comp;

                    // Save propertyInfo to track down type later on
                    this.propertyInfo = propertyInfo[i];

                    // Update as vector
                    if (propertyInfo[i].PropertyType == typeof(Vector3))
                    {
                        float theValue = (float)this.value;

                        // Cast
                        Vector3 vector3Value = (Vector3)propertyInfo[i].GetValue(compObj, null);

                        // Loop through each vector attribute
                        for (int j = 0; j < 3; j++)
                        {
                            // If true, overwrite current value with the incoming value
                            if (Vectors[j])
                                vector3Value[j] = (float)this.value;

                        }
                        // Set the value
                        propertyInfo[i].SetValue(compObj, vector3Value, null);
                    }
                    // Update as int
                    else if (propertyInfo[i].PropertyType == typeof(int))
                    {
                        // Cast
                        int intValue = (int)this.value;
                        // Set the value
                        propertyInfo[i].SetValue(compObj, intValue, null);
                    }
                    // Default
                    else
                    {
                        propertyInfo[i].SetValue(compObj, this.value, null);
                    }
                }
            }
        }
    }

    // Updates the variables of a script
    public void UpdateVisual(Dictionary<FieldInfo, Component> fieldInfoDictionary)
    {
        // Temp value
        //this.value = Random.Range(0f, 1f);

        List<FieldInfo> fieldInfo = new List<FieldInfo>(fieldInfoDictionary.Keys);
        if (this.value != null)
        {
            // Loop through all the variables of the script
            for (int i = 0; i < fieldInfo.Count; i++)
            {
                // If the variable is related to this node
                if (fieldInfo[i].Name == this.nodeName)
                {
                    Component comp;
                    fieldInfoDictionary.TryGetValue(fieldInfo[i], out comp);
                    compObj = (System.Object)comp;

                    // Save fieldInfo to track down type later on
                    this.fieldInfo = fieldInfo[i];

                    // Update as vector
                    if (fieldInfo[i].FieldType == typeof(Vector3))
                    {
                        float theValue = (float)this.value;
                        
                        // Cast
                        Vector3 vector3Value = (Vector3)fieldInfo[i].GetValue(compObj);//new Vector3((float)this.value, (float)this.value, (float)this.value);

                        // Loop through each vector attribute
                        for (int j = 0; j < 3; j++)
                        {
                            // If true, overwrite current value with the incoming value
                            if (Vectors[i])
                                vector3Value[i] = (float)this.value;

                        }

                        // Set the value
                        fieldInfo[i].SetValue(compObj, vector3Value);
                    }
                    // Update as int
                    else if (fieldInfo[i].FieldType == typeof(int))
                    {
                        // Cast
                        int intValue = (int)this.value;
                        // Set the value
                        fieldInfo[i].SetValue(compObj, intValue);
                    }
                    // Default
                    else
                    {
                        fieldInfo[i].SetValue(compObj, this.value);
                    }
                }
            }
        }
    }
}

public enum Operators
{
    Add = 0,
    Subtract = 1,
    Divide = 2,
    Multiply = 3,
    Power = 4
}

// Operator node, receives a value, modifies it and outputs it to other nodes.
public class OperatorNode : Node
{

    public float modifier;
    public float output;
    public Operators currentOperator;
    public OperatorNode(Rect r, string name)
    {
        this.rectangle = r;
        this.nodeName = name;
    }
    public void CalculateOutput()
    {
        if (this.value != null)
        {
            switch (currentOperator)
            {
                case Operators.Add:
                    this.output = (float)this.value + this.modifier;
                    break;
                case Operators.Divide:
                    if (modifier != 0)
                        this.output = (float)this.value / this.modifier;
                    break;
                case Operators.Multiply:
                    this.output = (float)this.value * this.modifier;
                    break;
                case Operators.Subtract:
                    this.output = (float)this.value - this.modifier;
                    break;
                case Operators.Power:
                    this.output = Mathf.Pow((float)this.value, this.modifier);
                    break;
                default:
                    this.output = (float)this.value * this.modifier;
                    break;

            }
            
            //Debug.Log(this.output);
        }
    }

    public void UpdateState(Operators currentOperator)
    {
        this.currentOperator = currentOperator;
    }
}

public class NodeEditor : EditorWindow
{
    Operators display = Operators.Multiply;
    //OSC variables
    public string RemoteIP = /*"146.176.164.4";*/ "127.0f.0.1f"; // signifies a local host (if testing locally
    public int SendToPort = 9000; //the port you will be sending from
    public int ListenerPort = 8050; //the port you will be listening on
    private Osc handler;

    // List of properties from components
    Dictionary<PropertyInfo, Component> propertyInfo = new Dictionary<PropertyInfo, Component>();

    // List of members from components
    Dictionary<FieldInfo, Component> fieldInfo = new Dictionary<FieldInfo, Component>();

    // List of rectangle nodes.
    List<Node> windows = new List<Node>();

    // Temp List which holds which two nodes are to be connected.
    List<int> windowsToAttach = new List<int>();

    // IDS of connected nodes.
    List<int> attachedWindows = new List<int>();
  
    [MenuItem("Window/Node Editor")]
    static void Init()
    {
        NodeEditor window = (NodeEditor)GetWindow(typeof(NodeEditor));
        UDPPacketIO udp = new UDPPacketIO();
        //// Init the user datagram protocal.
        //// Can change the listen port for each different input?
        udp.init(window.RemoteIP, window.SendToPort, window.ListenerPort);
        window.handler = new Osc();
        window.handler.init(udp);
        window.handler.SetAllMessageHandler(window.AllMessageHandler);
        
        window.Show();

    }

    // called when a right-click option is selected.
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
            case "Random":
                windows.Add(new RandomGeneratorNode(new Rect(mousePos.x, mousePos.y, 100, 100), "Random"));
                break;
            case "Amplitude":
                windows.Add(new AudioNode(new Rect(mousePos.x, mousePos.y, 100, 100), "Amplitude"));
                break;
            case "Volume":
                windows.Add(new AudioNode(new Rect(mousePos.x, mousePos.y, 100, 100), "Volume"));
                break;
            case "Pitch":
                windows.Add(new AudioNode(new Rect(mousePos.x, mousePos.y, 100, 100), "Pitch"));
                break;
            case "GenericAudio":
                windows.Add(new AudioNode(new Rect(mousePos.x,mousePos.y, 100,100), "Insert Parameter"));
                break;
            case "MaxNode":
                Debug.Log("Not working right now");
                //windows.Add(new MaxNode(new Rect(mousePos.x, mousePos.y, 100, 100), "MaxNode"));
                break;
            case "ControllerNode":
                windows.Add(new ControllerNode(new Rect(mousePos.x, mousePos.y, 200, 400), "ControllerNode"));
                break;
            case "Operator":
                windows.Add(new OperatorNode(new Rect(mousePos.x, mousePos.y, 200, 150), "Operator"));
                break;
            default:
                // Do this for all of components other than scripts
                List<PropertyInfo> pi = new List<PropertyInfo>(propertyInfo.Keys);
                foreach (PropertyInfo currentPi in pi)
                {
                    if (currentPi.Name == nodeRequested)
                    {
                        Component comp;
                        propertyInfo.TryGetValue(currentPi, out comp);
                        System.Object compObj = (System.Object)comp;
                        windows.Add(new VisualNode(new Rect(mousePos.x, mousePos.y, 150, 200), nodeRequested, currentPi.GetValue(compObj, null)));
                    }
                }
                // Do this for script components
                List<FieldInfo> fi = new List<FieldInfo>(fieldInfo.Keys);
                foreach (FieldInfo currentFi in fi)
                {
                    if (currentFi.Name == nodeRequested)
                    {
                        Component comp;
                        fieldInfo.TryGetValue(currentFi, out comp);
                        System.Object compObj = (System.Object)comp;
                        windows.Add(new VisualNode(new Rect(mousePos.x, mousePos.y, 150, 200), nodeRequested, currentFi.GetValue(compObj)));
                    }
                }
                break;
        }

        // If node is requested to be deleted, find it and remove.
        if (nodeRequested.Contains("Delete"))
        {
            string[] values = nodeRequested.Split(':');
            int index = int.Parse(values[1]);
            Debug.Log(index);
 
            // Remove node at location.
            windows.RemoveAt(index);
            // Next step. Find connections to node and remove them.
            
            //if ()
            // If index location is even remove next one. If odd remove one behind.
        }

    }

    void Update()
    {
        // For each window
        for (int i = 0; i < windows.Count; i++)
        {
            //// Update audio nodes
            //if (windows[i] is AudioNode)
            //{
            //    AudioNode temp = (AudioNode)windows[i];
            //    temp.UpdateValues();
            //    
            //}

            // Update visual nodes
            if (windows[i] is VisualNode)
            {
                // Cast and Update
                VisualNode temp = (VisualNode)windows[i];
                // Update variables for components
                temp.UpdateVisual(propertyInfo);
                // Update variables for scripts
                temp.UpdateVisual(fieldInfo);
            }

            // Update operator nodes
            if (windows[i] is OperatorNode)
            {
                // Cast and Update
                OperatorNode temp = (OperatorNode)windows[i];
                temp.CalculateOutput();
            }

            // Update rng nodes
            if (windows[i] is RandomGeneratorNode)
            {
                RandomGeneratorNode temp = (RandomGeneratorNode)windows[i];
                temp.UpdateValues();

            }

        }
        Repaint();

    }

    void OnGUI()
    {
        //for (int i = 0; i < windows.Count; i++)
        //{
        //    if (Event.current.type == EventType.MouseDrag)
        //    {
        //        // Debug.Log("Rec: " + windows[id].rectangle);
        //        //Debug.Log("Mou: " + Event.current.mousePosition);
        //        if (windows[i].rectangle.Contains(Event.current.mousePosition))
        //            windows[i].rectangle.position = Event.current.mousePosition;
        //    }
        //}


        // Keep drawing a line from selected rectangle to the mouse position
        if (windowsToAttach.Count == 1)
        {
            // Repaint the GUI
            Repaint();
            // Draw curve between rect and mouse pos
            DrawNodeCurve(windows[windowsToAttach[0]].rectangle, (Vector3)Event.current.mousePosition);
        }

        // If windowsToAttach is full, add to connected nodes and reset.
        if (windowsToAttach.Count >= 2)
        {
            // Add them to connection
            attachedWindows.Add(windowsToAttach[0]);
            attachedWindows.Add(windowsToAttach[1]);
            // Reset windowsToAttach.
            windowsToAttach = new List<int>();
        }
        // Beginning area for popup windows.
        BeginWindows();
        
        // Connection
        for (int i = 0; i < attachedWindows.Count; i += 2)
        {
            // Draw the connection
            DrawNodeCurve(windows[attachedWindows[i]].rectangle, windows[attachedWindows[i + 1]].rectangle);

            if (windows[attachedWindows[i]] is OperatorNode)
            {
                // Cast to operator node
                OperatorNode temp = (OperatorNode)windows[attachedWindows[i]];
                // Pass on output instead of value
                windows[attachedWindows[i + 1]].value = temp.output;
            }
            else
            {
                // Pass along the value for the connection, from left to right
                windows[attachedWindows[i + 1]].value = windows[attachedWindows[i]].value;
            }

        }

        // Draw right click menu and populate list. Also check for right click event.
        Event currentEvent = Event.current;
        // Create generic menu.
        GenericMenu menu = new GenericMenu();
        // Capture current mouse position.
        Vector2 mousePos = currentEvent.mousePosition;

        // If right click, generate window.

        if (currentEvent.type == EventType.ContextClick)
        { 
            // Add delete option.
            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].rectangle.Contains(mousePos))
                {
                    menu.AddItem(new GUIContent("Delete"), false, Callback, "Delete:" + i);
                    menu.AddSeparator("");
                }
            }
            // If right click was pressed, stop trying to create a connection
            windowsToAttach.Clear();

            // Now create the menu, add items and show it
            menu.AddItem(new GUIContent("ControllerNode"), false, Callback, "ControllerNode");
            menu.AddSeparator("");
            //menu.AddItem(new GUIContent("VisualNodes/"), false, Callback, "V");
            //menu.AddSeparator("");
            menu.AddItem(new GUIContent("Operator"), false, Callback, "Operator");
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("AudioNodes/Random"), false, Callback, "Random");
            menu.AddItem(new GUIContent("AudioNodes/Amplitude"), false, Callback, "Amplitude");
            menu.AddItem(new GUIContent("AudioNodes/Pitch"), false, Callback, "Pitch");
            menu.AddItem(new GUIContent("AudioNodes/Volume"), false, Callback, "Volume");
            menu.AddItem(new GUIContent("AudioNodes/GenericAudio"), false, Callback, "GenericAudio");
         //   menu.AddSeparator("");
         //   menu.AddItem(new GUIContent("MaxMSP/MaxMSP"), false, Callback, "MaxNode");
            // What does this part do?
            List<PropertyInfo> pi = new List<PropertyInfo>(propertyInfo.Keys);

            foreach (PropertyInfo currentPi in pi)
            {
                menu.AddItem(new GUIContent("VisualNodes/" + currentPi.DeclaringType + "/" + currentPi.Name), false, Callback, currentPi.Name);
            }

            List<FieldInfo> fi = new List<FieldInfo>(fieldInfo.Keys);

            foreach (FieldInfo currentFi in fi)
            {
                menu.AddItem(new GUIContent("VisualNodes/" + currentFi.DeclaringType + "/" + currentFi.Name), false, Callback, currentFi.Name);
            }

            menu.ShowAsContext();
            currentEvent.Use();
        }

        // For each window, draw window.
        for (int i = 0; i < windows.Count; i++)
        {
            if (windows[i] is AudioNode)
            {
                string displayName = windows[i].nodeName + " (Audio)";
                windows[i].rectangle = GUI.Window(i, windows[i].rectangle, DrawNodeWindow, displayName);

            }
            else if (windows[i] is MaxNode)
            {
                windows[i].rectangle = GUI.Window(i, windows[i].rectangle, DrawMaxNodeWindow, windows[i].nodeName);

            }
            else
                windows[i].rectangle = GUI.Window(i, windows[i].rectangle, DrawNodeWindow, windows[i].nodeName);
        }
        
        //// Connection
        //for (int i = 0; i < attachedWindows.Count; i += 2)
        //{
        //    // Draw the connection
        //    DrawNodeCurve(windows[attachedWindows[i]].rectangle, windows[attachedWindows[i + 1]].rectangle);
        //
        //    // Pass along the value for the connection, from left to right
        //    windows[attachedWindows[i + 1]].value = windows[attachedWindows[i]].value;
        //}

        
        EndWindows();
    }


    void DrawMaxNodeWindow(int id)
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
        windows[id].nodeName = GUILayout.TextArea(windows[id].nodeName);
        MaxNode temp = (MaxNode)windows[id];
        windows[id].nodeName = GUILayout.TextArea(temp.inPort.ToString());
        GUI.DragWindow(new Rect(0, 0, 100, 20));
    }

    // Draws the node window.
    void DrawNodeWindow(int id)
    {
    
        // Controller node cannot attach to anything
        if (windows[id].GetType() != typeof(ControllerNode))
        {
            if (GUILayout.Button("Attach"))
            {
                if (windowsToAttach.Count < 2)
                {
                    // Avoid duplicates
                    if (windowsToAttach.Contains(id))
                    {
                        // If attach was pressed twice on the same node, stop trying to create a connection
                        windowsToAttach.Clear();
                    }
                    else
                    {
                        windowsToAttach.Add(id);
                    }
                }
            }
        }
        windows[id].nodeName = GUILayout.TextArea(windows[id].nodeName);
        if (windows[id] is AudioNode)
        {
            // It is stored to a temp variable to prevent user from messing with the audio parameters. Change
            string t = "No Value...";
            //Debug.Log(windows[id].value);
            if (windows[id].value != null)
               t = windows[id].value.ToString();
            GUILayout.Label(t);
        }

        // If controller node
        if (windows[id] is ControllerNode)
        {
            // Cast and add TextArea
            ControllerNode temp = (ControllerNode)windows[id];
            //temp.gameObjectTag = GUILayout.TextArea(temp.gameObjectTag);
            temp.visual = EditorGUILayout.ObjectField(temp.visual, typeof(Object), true);
            temp.Test();

            if (temp.visual != null)
            {
                List<Component> keys = new List<Component>(temp.componentsDictionary.Keys);
                foreach (Component component in keys)
                {
                    // Get value from check box
                    bool value = false;
                    temp.componentsDictionary.TryGetValue(component, out value);
                    value = GUILayout.Toggle(value, component.GetType().ToString());

                    // Replace value in dictionary at specified key
                    temp.componentsDictionary[component] = value;

                    if (value)
                    {
                        // For most components
                        foreach (PropertyInfo pi in component.GetType().GetProperties())
                        {
                            System.Object obj = (System.Object)component;
                            // Only use objects of type Vector3, float, int.
                            if (pi.PropertyType == typeof(Vector3) || pi.PropertyType == typeof(float) ||
                                pi.PropertyType == typeof(int))
                            {

                                // Add each property to list of properties
                                if (!(propertyInfo.ContainsKey(pi)))
                                    propertyInfo.Add(pi, component);

                                // Set up GUI on controller
                                // GUILayout.Toggle(value, pi.Name);
                                // GUILayout.TextField(pi.GetValue(obj, null).ToString());
                            }
                        }

                        // For scripts
                        foreach (FieldInfo fi in component.GetType().GetFields())
                        {
                            // Only use objects of type Vector3, float, int.
                            if (fi.FieldType == typeof(Vector3) || fi.FieldType == typeof(float) ||
                                fi.FieldType == typeof(int))
                            {
                                System.Object obj = (System.Object)component;

                                // Add each field to list of fields
                                if (!(fieldInfo.ContainsKey(fi)))
                                    fieldInfo.Add(fi, component);
                                // Set up GUI on controller
                                // GUILayout.Toggle(value, fi.Name);
                                // GUILayout.TextField(fi.GetValue(obj).ToString());
                            }
                        }
                    }
                }
            }
        }
        else if (windows[id] is VisualNode)
        {
            // Cast and add TextField for value
            VisualNode temp = (VisualNode)windows[id];
            //temp.UpdateVisual(propertyInfo);

            if (temp.value != null)
            {
                
                //Debug.Log(temp.value.GetType());
                // Vectors are displayed differently than floats and ints
                if (temp.propertyInfo != null)
                {
                    if (temp.propertyInfo.PropertyType == typeof(Vector3))
                    {

                        // Cast object to Vector3
                        Vector3 vector3Value = (Vector3)temp.propertyInfo.GetValue(temp.compObj, null);

                        // Display Vector3
                        EditorGUILayout.Vector3Field("", vector3Value);

                        // Toggle boxes
                        EditorGUILayout.BeginHorizontal();
                        temp.Vectors[0] = EditorGUILayout.Toggle(temp.Vectors[0]);
                        temp.Vectors[1] = EditorGUILayout.Toggle(temp.Vectors[1]);
                        temp.Vectors[2] = EditorGUILayout.Toggle(temp.Vectors[2]);
                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {
                        EditorGUILayout.FloatField((float)temp.value);
                    }
                }
                if (temp.fieldInfo != null)
                {
                    if (temp.fieldInfo.FieldType == typeof(Vector3))
                    {
                        // Cast object to Vector3
                        Vector3 vector3Value = (Vector3)temp.fieldInfo.GetValue(temp.compObj);

                        // Display Vector3
                        EditorGUILayout.Vector3Field("", vector3Value);

                        // Toggle boxes
                        EditorGUILayout.BeginHorizontal();
                        temp.Vectors[0] = EditorGUILayout.Toggle(temp.Vectors[0]);
                        temp.Vectors[1] = EditorGUILayout.Toggle(temp.Vectors[1]);
                        temp.Vectors[2] = EditorGUILayout.Toggle(temp.Vectors[2]);
                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {
                        EditorGUILayout.FloatField((float)temp.value);
                    }
                }
            }

        }
        // YOu can drag the window if it is along the header - Visual que? 
        

        else if (windows[id] is OperatorNode)
        {
            OperatorNode temp = (OperatorNode)windows[id];

            //float.TryParse(GUILayout.TextField(temp.modifier.ToString()), out temp.modifier);
            //temp.modifier = EditorGUI.FloatField(new Rect(windows[id].rectangle.width - 60, windows[id].rectangle.height - 25, 50, 20), temp.modifier);
            temp.modifier = EditorGUILayout.FloatField("Modifier:", temp.modifier);

            //Transform selectedObj = Selection.activeTransform;

            display = (Operators)EditorGUILayout.EnumPopup(
                "Type",
                display);

            temp.UpdateState(display);

            // Show output
            EditorGUILayout.FloatField("Output:", temp.output);

        }
        GUI.DragWindow();
        //GUI.DragWindow(new Rect(0, 0, 100, 20));
    }

    // Between 2 rectangles
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

    // Between rectangle and vector3
    void DrawNodeCurve(Rect start, Vector3 end)
    {
        Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
        Vector3 endPos = new Vector3(end.x, end.y, 0);
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


    // msgAddress is a poor variable name. It is actually what musical parameter (e.g. pitch, frequency etc)
    public void AllMessageHandler(OscMessage oscMessage)
    {
        //Debug.Log(oscMessage.Address);
        // Where do send it too.
        string incAddress = oscMessage.Address;
        // Value it contains.
        object incValue = oscMessage.Values[0];

        // Search each audio node to find where to send it.
        for (int i = 0; i < windows.Count; i++)
        {
            // First check if correct class type.
            if (windows[i] is AudioNode)
            {
                // If windows contains name to this address.
                if (incAddress.Contains(windows[i].nodeName))
                {
                    //Debug.Log("Found");
                    windows[i].value = incValue;
                    Debug.Log(windows[i].value);
                    
                }

            }
        }
    }
}


