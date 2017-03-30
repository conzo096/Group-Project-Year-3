using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

/* TODO LIST.
 *
 * Delete nodes, delete connections.
 *
 */


namespace Assets.Scripts.Node_Editor
{
    public class NodeEditor : EditorWindow
    {
        // Object field for controller node
        Object fromObjectField = new Object();
        Operators display = Operators.Multiply;
        //OSC variables
        public string RemoteIP = /*"146.176.164.4";*/ "127.0f.0.1f"; // signifies a local host (if testing locally
        public int SendToPort = 9000; //the port you will be sending from
        public int ListenerPort = 8050; //the port you will be listening on
        private Osc handler; // Handles listening to max messages.
                             // Id for the node.
        static private int uniqueNodeId = 0;

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
                    windows.Add(new RandomGeneratorNode(new Rect(mousePos.x, mousePos.y, 100, 100), "Random", uniqueNodeId));
                    uniqueNodeId++;

                    break;
                case "Amplitude":
                    windows.Add(new AudioNode(new Rect(mousePos.x, mousePos.y, 100, 100), "Amplitude", uniqueNodeId));
                    uniqueNodeId++;
                    break;
                case "Volume":
                    windows.Add(new AudioNode(new Rect(mousePos.x, mousePos.y, 100, 100), "Volume", uniqueNodeId));
                    uniqueNodeId++;
                    break;
                case "Pitch":
                    windows.Add(new AudioNode(new Rect(mousePos.x, mousePos.y, 100, 100), "Pitch", uniqueNodeId));
                    uniqueNodeId++;
                    break;
                case "GenericAudio":
                    windows.Add(new AudioNode(new Rect(mousePos.x, mousePos.y, 100, 100), "Insert Parameter", uniqueNodeId));
                    uniqueNodeId++;
                    break;
                case "MaxNode":
                    Debug.Log("Not working right now");
                    //windows.Add(new MaxNode(new Rect(mousePos.x, mousePos.y, 100, 100), "MaxNode"));
                    //uniqueNodeId++;
                    break;
                case "ControllerNode":
                    windows.Add(new ControllerNode(new Rect(mousePos.x, mousePos.y, 200, 400), "ControllerNode", uniqueNodeId));
                    uniqueNodeId++;
                    break;
                case "Operator":
                    windows.Add(new OperatorNode(new Rect(mousePos.x, mousePos.y, 200, 150), "Operator", uniqueNodeId));
                    uniqueNodeId++;
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
                            windows.Add(new VisualNode(new Rect(mousePos.x, mousePos.y, 150, 200), nodeRequested, currentPi.GetValue(compObj, null), uniqueNodeId));
                            uniqueNodeId++;
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
                            windows.Add(new VisualNode(new Rect(mousePos.x, mousePos.y, 150, 200), nodeRequested, currentFi.GetValue(compObj), uniqueNodeId));
                            uniqueNodeId++;
                        }
                    }
                    break;
            }

            // If node is requested to be deleted, find it and remove.
            if (nodeRequested.Contains("Delete"))
            {
                // Split string into two components.
                string[] values = nodeRequested.Split(':');
                // node index for windows is the second part of the array.
                int index = int.Parse(values[1]);
                // Next step. Find connections to node and remove them.

                // iterate backwards.
                for (int i = attachedWindows.Count - 1; i >= 0; i--)
                {
                    // if the index value matches id in attachedWindows...
                    // Remove it and the one it is connected too.
                    if (attachedWindows[i] == index)
                    {
                        attachedWindows.RemoveAt(i);
                        if (i % 2 == 0)
                        {
                            attachedWindows.RemoveAt(i);
                        }
                        else
                        {
                            attachedWindows.RemoveAt(i - 1);
                            i--;
                        }
                    }
                }
                // Remove node at location.
                for (int i = windows.Count - 1; i >= 0; i--)
                {
                    if (windows[i].id == index)
                    {
                        windows.RemoveAt(i);
                    }
                }
            }

        }

        void Update()
        {
            // For each window
            for (int i = 0; i < windows.Count; i++)
            {
                // Update audio nodes
                if (windows[i] is AudioNode)
                {
                    AudioNode temp = (AudioNode)windows[i];
                    temp.UpdateValues();

                }
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
            // Keep drawing a line from selected rectangle to the mouse position
            if (windowsToAttach.Count == 1)
            {
                // Repaint the GUI
                Repaint();
                // Draw curve between rect and mouse pos
                if (windowsToAttach.Count == 1)
                    for (int i = 0; i < windows.Count; i++)
                        if (windows[i].id == windowsToAttach[0])
                            DrawNodeCurve(windows[i].rectangle, (Vector3)Event.current.mousePosition);
            }

            // If windowsToAttach is full, add to connected nodes and reset.
            if (windowsToAttach.Count == 2)
            {
                for (int i = 0; i < windows.Count; i++)
                {
                    if(windows[i].id == windowsToAttach[1] )
                        if (!(windows[i] is AudioNode || windows[i] is RandomGeneratorNode))
                        {
                            // Add them to connection
                            attachedWindows.Add(windowsToAttach[0]);
                            attachedWindows.Add(windowsToAttach[1]);
                        }
                }
                windowsToAttach = new List<int>();
            }
            // Beginning area for popup windows.
            BeginWindows();

            // Draw the connection. - Remove abract on type Node.
            int firstNodeIndex = 0;
            int secondNodeIndex =0;
            // Connection        
            for (int x =0; x< attachedWindows.Count; x+=2)
            {
                // Search for first loc.
                for (int i = 0; i < windows.Count; i++)
                {
                    if (windows[i].id == attachedWindows[x])
                        firstNodeIndex = i;
                }

                // Now find the second.
                for (int i = 0; i < windows.Count; i++)
                {
                   
                    if (windows[i].id == attachedWindows[x+1])
                        secondNodeIndex = i;
                }

                DrawNodeCurve(windows[firstNodeIndex].rectangle, windows[secondNodeIndex].rectangle);
                if (windows[firstNodeIndex] is OperatorNode)
                {
                    // Cast to operator node
                    OperatorNode temp = (OperatorNode)windows[firstNodeIndex];
                    // Pass on output instead of value
                    windows[secondNodeIndex].value = temp.output;
                }
                else
                {
                    // Pass along the value for the connection, from left to right
                    windows[secondNodeIndex].value = windows[firstNodeIndex].value;
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
                        menu.AddItem(new GUIContent("Delete"), false, Callback, "Delete:" + windows[i].id);
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
            EndWindows();
        }


        // This will not work. Old code.
        void DrawMaxNodeWindow(int id)
        {
            if (GUILayout.Button("Attach"))
            {
                // If nothing is been attached, set it up.
                if (windowsToAttach.Count == 1)
                {
                    Debug.Log("Adding :" + windows[id].GetType());
                    windowsToAttach.Add(windows[id].id);
                }
                if (windowsToAttach.Count == 1)
                {
                    // Avoid duplicates
                    if (windowsToAttach.Contains(id))
                    {
                        // Do nothing
                    }
                    else
                    {
                        // Check what the first node was.

                        // Check conditions for audio node.
                        if (windows[windowsToAttach[0]] is AudioNode)
                        {
                            if (!(windows[id] is AudioNode))
                            {
                                windowsToAttach.Add(windows[id].id);
                            }
                        }
                    }
                }
            }
            windows[id].nodeName = GUILayout.TextArea(windows[id].nodeName);
            MaxNode temp = (MaxNode)windows[id];
            windows[id].nodeName = GUILayout.TextArea(temp.inPort.ToString());
            GUI.DragWindow(new Rect(0, 0, windows[id].rectangle.width, 20));
        }

        // Draws the node window.
        void DrawNodeWindow(int id)
        {
            // Controller node cannot attach to anything
            if (windows[id].GetType() != typeof(ControllerNode))
            {
                if (GUILayout.Button("Attach"))
                {
                    // If id is already selected, remove the connection.
                    if (windowsToAttach.Contains(windows[id].id))
                    {
                        windowsToAttach.Clear();
                    }
                    else
                    {
                        windowsToAttach.Add(windows[id].id);
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
                // Disable GUI for object field
                if (fromObjectField != null)
                    GUI.enabled = false;
                fromObjectField = EditorGUILayout.ObjectField(fromObjectField, typeof(Object), true);
                // Enable GUI for rest
                if (fromObjectField != null)
                    GUI.enabled = true;
                // Link visual object to given visual node, only once
                if (temp.visual == null)
                {
                    temp.visual = fromObjectField;
                    temp.Test();
                }

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
                                // Only use objects of type Vector3, float, int.
                                if (pi.PropertyType == typeof(Vector3) || pi.PropertyType == typeof(float) ||
                                    pi.PropertyType == typeof(int))
                                {
                                    // Add each property to list of properties
                                    if (!(propertyInfo.ContainsKey(pi)))
                                        propertyInfo.Add(pi, component);
                                }
                            }

                            // For scripts
                            foreach (FieldInfo fi in component.GetType().GetFields())
                            {
                                // Only use objects of type Vector3, float, int.
                                if (fi.FieldType == typeof(Vector3) || fi.FieldType == typeof(float) ||
                                    fi.FieldType == typeof(int))
                                {
                                    // Add each field to list of fields
                                    if (!(fieldInfo.ContainsKey(fi)))
                                        fieldInfo.Add(fi, component);
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
            // Area of rect to drag (initial, inital, width,height);
            GUI.DragWindow(new Rect(0, 0, windows[id].rectangle.width, 20));
        }

        // Between 2 rectangles
        void DrawNodeCurve(Rect start, Rect end)
        {
            Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;
            Color shadowCol = new Color(0, 0, 0, 0.06f);
            Vector3[] points = new Vector3[6];

            for (int i = 0; i < 3; i++)
            {
                // Draw a shadow
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
                Handles.color = new Color(0, 0, 0, 0.01f);
                // Shadow for arrow
                points[0] = endPos + new Vector3(1f + (i + 1), 0f, 0f);
                points[1] = endPos + new Vector3(-10f - (i + 1), -5f - (i + 1), 0f);
                points[2] = endPos + new Vector3(1f + (i + 1), 0f, 0f);
                points[3] = endPos + new Vector3(-10f - (i + 1), 5f + (i + 1), 0f);
                points[4] = endPos + new Vector3(-10f - (i + 1), -5f - (i + 1), 0f);
                points[5] = endPos + new Vector3(-10f - (i + 1), 5f + (i + 1), 0f);

                Handles.DrawAAConvexPolygon(points);
            }
            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
            Handles.color = Color.black;

            // Draw arrow
            points[0] = endPos;
            points[1] = endPos + new Vector3(-10f, -5f, 0f);
            points[2] = endPos;
            points[3] = endPos + new Vector3(-10f, 5f, 0f);
            points[4] = endPos + new Vector3(-10f, -5f, 0f);
            points[5] = endPos + new Vector3(-10f, 5f, 0f);

            Handles.DrawAAConvexPolygon(points);
        }

        // Between rectangle and vector3
        void DrawNodeCurve(Rect start, Vector3 end)
        {
            Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3(end.x, end.y, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;
            Color shadowCol = new Color(0, 0, 0, 0.06f);

            Vector3[] points = new Vector3[6];

            for (int i = 0; i < 3; i++)
            {
                // Draw a shadow
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
                Handles.color = new Color(0, 0, 0, 0.01f);
                // Shadow for arrow
                points[0] = endPos + new Vector3(1f + (i + 1), 0f, 0f);
                points[1] = endPos + new Vector3(-10f - (i + 1), -5f - (i + 1), 0f);
                points[2] = endPos + new Vector3(1f + (i + 1), 0f, 0f);
                points[3] = endPos + new Vector3(-10f - (i + 1), 5f + (i + 1), 0f);
                points[4] = endPos + new Vector3(-10f - (i + 1), -5f - (i + 1), 0f);
                points[5] = endPos + new Vector3(-10f - (i + 1), 5f + (i + 1), 0f);

                Handles.DrawAAConvexPolygon(points);
            }

            // Draw shadow for arrow
            //Handles.color = shadowCol;

            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);

            Handles.color = Color.black;
            // Draw arrow
            points[0] = endPos;
            points[1] = endPos + new Vector3(-10f, -5f, 0f);
            points[2] = endPos;
            points[3] = endPos + new Vector3(-10f, 5f, 0f);
            points[4] = endPos + new Vector3(-10f, -5f, 0f);
            points[5] = endPos + new Vector3(-10f, 5f, 0f);

            Handles.DrawAAConvexPolygon(points);
        }


        // msgAddress is a poor variable name. It is actually what musical parameter (e.g. pitch, frequency etc)
        public void AllMessageHandler(OscMessage oscMessage)
        {
            //Debug.Log(oscMessage.Address);
            // Where do send it too.
            string[] incAddress = oscMessage.Address.Split('/');
            // Value it contains.
            object incValue = oscMessage.Values[0];

            // Search each audio node to find where to send it.
            for (int i = 0; i < windows.Count; i++)
            {
                // First check if correct class type.
                if (windows[i] is AudioNode)
                {
                    // If windows contains name to this address. - Make it more accurate.
                    if (incAddress[1].ToLower().Equals(windows[i].nodeName.ToLower()))
                    {
                        //Debug.Log("Found");
                        windows[i].value = incValue;
                        //Debug.Log(windows[i].value);

                    }

                }
            }
        }
    }
}

