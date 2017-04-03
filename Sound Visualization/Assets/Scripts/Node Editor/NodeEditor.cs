using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System;

namespace NodeEditor
{
    [Serializable]
    public class NodeEditor : EditorWindow
    {
        public static UDPPacketIO udp;
        // Object field for controller node
        UnityEngine.Object fromObjectField = new UnityEngine.Object();
        //Operators display = Operators.Multiply;
        //OSC variables
        public string RemoteIP = /*"146.176.164.4";*/ "127.0f.0.1f"; // signifies a local host (if testing locally
        public int SendToPort = 9000; //the port you will be sending from
        public int ListenerPort = 8050; //the port you will be listening on
        private Osc handler; // Handles listening to max messages.
        // Id for the node.
        public int uniqueNodeId = 0;
        // List of properties from components
        Dictionary<CustomPropertyInfo, Component> propertyInfo = new Dictionary<CustomPropertyInfo, Component>();
        // List of members from components
        Dictionary<FieldInfo, Component> fieldInfo = new Dictionary<FieldInfo, Component>();
        // List of rectangle nodes.
        public List<Node> windows = new List<Node>();
        // Temp List which holds which two nodes are to be connected.
        public List<int> windowsToAttach = new List<int>();
        // IDS of connected nodes.
        public List<int> attachedWindows = new List<int>();
        bool draggingLeft = false;
        bool draggingRight = false;
        bool draggingUp = false;
        bool draggingDown = false;


        [MenuItem("Window/Node Editor")]
        static void Init()
        {
            NodeEditor window = (NodeEditor)GetWindow(typeof(NodeEditor));
            udp = new UDPPacketIO();
            // Init the user datagram protocal.
            // Can change the listen port for each different input?
            udp.init(window.RemoteIP, window.SendToPort, window.ListenerPort);
            window.handler = new Osc();
            window.handler.init(udp);
            window.handler.SetAllMessageHandler(window.AllMessageHandler);

            window.Show();

        }

        void OnDestroy()
        {
            udp.Close();
        }

        // called when a right-click option is selected.
        void Callback(object obj)
        {
            Vector2 mousePos = new Vector2(10, 10);
            string nodeRequested = obj.ToString();
            if (obj is CallBackObject)
            {
                CallBackObject temp = obj as CallBackObject;
                mousePos = temp.mousePosition;
                nodeRequested = temp.callBackName;
            }
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
                case "ControllerNode":
                    windows.Add(new ControllerNode(new Rect(mousePos.x, mousePos.y, 200, 400), "ControllerNode", uniqueNodeId));
                    uniqueNodeId++;
                    break;
                case "Operator":
                    windows.Add(new OperatorNode(new Rect(mousePos.x, mousePos.y, 200, 150), "Operator", uniqueNodeId));
                    uniqueNodeId++;
                    break;
                case "Material":
                    windows.Add(new MaterialNode(new Rect(mousePos.x, mousePos.y, 100, 150), "Material", uniqueNodeId));
                    uniqueNodeId++;
                    break;
                default:
                    // Do this for all of components other than scripts
                    List<CustomPropertyInfo> pi = new List<CustomPropertyInfo>(propertyInfo.Keys);
                    foreach (CustomPropertyInfo currentPi in pi)
                    {
                        if (currentPi.propertyInfo.Name + currentPi.parent == nodeRequested)
                        {
                            Component comp;
                            propertyInfo.TryGetValue(currentPi, out comp);
                            System.Object compObj = (System.Object)comp;
                            windows.Add(new VisualNode(new Rect(mousePos.x, mousePos.y, 200, 100), currentPi.propertyInfo.Name, currentPi.parent, currentPi.propertyInfo.GetValue(compObj, null), uniqueNodeId));
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
                            windows.Add(new VisualNode(new Rect(mousePos.x, mousePos.y, 200, 100), nodeRequested, "", currentFi.GetValue(compObj), uniqueNodeId));
                            uniqueNodeId++;
                        }
                    }
                    break;
            }
        }

        // Handles node deletion
        void NodeDeletion(object obj)
        {
            string nodeRequested = obj.ToString();
            if (nodeRequested == "DeleteAll")
            {
                attachedWindows = new List<int>();
                windows = new List<Node>();
                uniqueNodeId = 0;
            }
            else if (nodeRequested.Contains("Delete"))
            {
                // Split string into two components.
                string[] values = nodeRequested.Split(':');
                // node index for windows is the second part of the array.
                int index = int.Parse(values[1]);
                DeleteNode(index);

            }
        }

        // Handles any callback deletions.
        void DeleteConnection(object obj)
        {
            DeleteConnection temp = (DeleteConnection)obj;
            Node first = temp.firstNode;
            Node second = temp.secondNode;
            // If node is requested to be deleted, find it and remove.
            for (int i = 0; i < attachedWindows.Count; i += 2)
            {
                if (windows[attachedWindows[i]] == first && windows[attachedWindows[i + 1]] == second)
                {
                    attachedWindows.RemoveAt(i + 1);
                    attachedWindows.RemoveAt(i);
                }
            }           
        }

        // Deletes a node
        void DeleteNode(int index)
        {
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


        // Updates each frame.
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


                // Update material nodes
                if (windows[i] is MaterialNode)
                {
                    MaterialNode temp = (MaterialNode)windows[i];

                    List<Component> components = new List<Component>(propertyInfo.Values);
                    // Loop through all components
                    foreach (Component currentComponent in components)
                    {
                        // Debug.Log("trying to find renderer");
                        // Find the renderer to reference the material
                        if (currentComponent.GetType() == typeof(MeshRenderer))
                        {
                            // Debug.Log("Found renderer");
                            MeshRenderer rendererComponent = (MeshRenderer)currentComponent;
                            temp.material = rendererComponent.sharedMaterial;
                        }
                    }
                    // Make the material node pass its value to the material
                    temp.PassValueToMaterial();
                }

            }
            Repaint();

        }

        void OnGUI()
        {

            // Draw right click menu and populate list. Also check for right click event.
            Event currentEvent = Event.current;
            // Create generic menu.
            GenericMenu menu = new GenericMenu();
            // Capture current mouse position.
            Vector2 mousePos = currentEvent.mousePosition;

            // Keep drawing a line from selected rectangle to the mouse position
            if (windowsToAttach.Count == 1)
            {
                // Repaint the GUI
                Repaint();
                // Draw curve between rect and mouse pos
                if (windowsToAttach.Count == 1)
                    for (int i = 0; i < windows.Count; i++)
                        if (windows[i].id == windowsToAttach[0])
                            DrawNodeCurve(windows[i].rectangle, Event.current.mousePosition);
            }

            // If windowsToAttach is full, add to connected nodes and reset.
            if (windowsToAttach.Count == 2)
            {
                for (int i = 0; i < windows.Count; i++)
                {
                    if (windows[i].id == windowsToAttach[1])
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
            int secondNodeIndex = 0;
            // Connection        
            for (int x = 0; x < attachedWindows.Count; x += 2)
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

                    if (windows[i].id == attachedWindows[x + 1])
                        secondNodeIndex = i;
                }
                DrawNodeCurve(windows[firstNodeIndex].rectangle, windows[secondNodeIndex].rectangle);
                bool col = DetectCursorLineDetection(mousePos, windows[firstNodeIndex].rectangle, windows[secondNodeIndex].rectangle);
                if (col)
                {
                    menu.AddItem(new GUIContent("DeleteConnection"), false, DeleteConnection, new DeleteConnection(windows[firstNodeIndex], windows[secondNodeIndex]));
                    menu.AddSeparator("");
                }
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
                    if(windows[firstNodeIndex] != null)
                        windows[secondNodeIndex].value = windows[firstNodeIndex].value;
                }
            }
            // If right click, generate window.

            if (currentEvent.type == EventType.ContextClick)
            {
                // Add delete option.
                for (int i = 0; i < windows.Count; i++)
                {
                    if (windows[i].rectangle.Contains(mousePos))
                    {
                        menu.AddItem(new GUIContent("Delete"), false, NodeDeletion, "Delete:" + windows[i].id);
                        menu.AddSeparator("");
                    }
                }

                // If right click was pressed, stop trying to create a connection
                windowsToAttach.Clear();

                // Now create the menu, add items and show it
                menu.AddItem(new GUIContent("ControllerNode"), false, Callback, new CallBackObject("ControllerNode", mousePos));
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Operator"), false, Callback, new CallBackObject("Operator", mousePos));
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("AudioNodes/Random"), false, Callback, new CallBackObject("Random", mousePos));
                menu.AddItem(new GUIContent("AudioNodes/Amplitude"), false, Callback, new CallBackObject("Amplitude", mousePos));
                menu.AddItem(new GUIContent("AudioNodes/Pitch"), false, Callback, new CallBackObject("Pitch", mousePos));
                menu.AddItem(new GUIContent("AudioNodes/Volume"), false, Callback, new CallBackObject("Volume", mousePos));
                menu.AddItem(new GUIContent("AudioNodes/GenericAudio"), false, Callback, new CallBackObject("GenericAudio", mousePos));
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("File/Save"), false, SaveWindow, "Save");
                menu.AddItem(new GUIContent("File/Load"), false, LoadWindow, "Load");
                menu.AddItem(new GUIContent("File/DeleteAll"), false, NodeDeletion, "DeleteAll");
                menu.AddSeparator("");

                // What does this part do?
                List<CustomPropertyInfo> pi = new List<CustomPropertyInfo>(propertyInfo.Keys);

                foreach (CustomPropertyInfo currentPi in pi)
                {
                    Component theComponent = new Component();
                    propertyInfo.TryGetValue(currentPi, out theComponent);
                    menu.AddItem(new GUIContent("VisualNodes/" + theComponent.gameObject.name + "/" + currentPi.propertyInfo.DeclaringType + "/" + currentPi.propertyInfo.Name), false, Callback, currentPi.propertyInfo.Name + currentPi.parent);

                    // Special check for Renderer
                    if (currentPi.propertyInfo.DeclaringType == typeof(Renderer))
                    {
                        menu.AddItem(new GUIContent("VisualNodes/" + theComponent.gameObject.name + "/" + currentPi.propertyInfo.DeclaringType + "/Material"), false, Callback, "Material");

                    }
                }

                List<FieldInfo> fi = new List<FieldInfo>(fieldInfo.Keys);

                foreach (FieldInfo currentFi in fi)
                {
                    Component theComponent = new Component();
                    fieldInfo.TryGetValue(currentFi, out theComponent);
                    menu.AddItem(new GUIContent("VisualNodes/" + theComponent.gameObject.name + "/" + currentFi.DeclaringType + "/" + currentFi.Name), false, Callback, currentFi.Name);
                }

                menu.ShowAsContext();
                currentEvent.Use();
            }

            // For each window, draw window.
            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].rectangle.Contains(mousePos))
                {
                    windows[i].rectangle = HorizResizer(windows[i].rectangle); //right
                    windows[i].rectangle = HorizResizer(windows[i].rectangle, false); //left
                    windows[i].rectangle = VertResizer(windows[i].rectangle); //Up
                    windows[i].rectangle = VertResizer(windows[i].rectangle, false); //down
                }

                if (windows[i] is AudioNode)
                {
                    string displayName = windows[i].nodeName + " (Audio)";
                    windows[i].rectangle = GUI.Window(i, windows[i].rectangle, DrawNodeWindow, displayName);

                }
                else if (windows[i] is MaxNode)
                {
                    windows[i].rectangle = GUI.Window(i, windows[i].rectangle, DrawMaxNodeWindow, windows[i].nodeName);

                }
                else if (windows[i] is VisualNode)
                {
                    VisualNode visualNode = (VisualNode)windows[i];
                    string displayName = windows[i].nodeName + "(" + visualNode.parent + ")";
                    windows[i].rectangle = GUI.Window(i, windows[i].rectangle, DrawNodeWindow, displayName);
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
            // Only allow editing node name for audio and max nodes
            if (windows[id] is AudioNode || windows[id] is MaxNode)
            {
                windows[id].nodeName = GUILayout.TextArea(windows[id].nodeName);
            }

            if (windows[id] is AudioNode)
            {
                // It is stored to a temp variable to prevent user from messing with the audio parameters. Change
                string t = "No Value...";
                //Debug.Log(windows[id].value);
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
                //if (fromObjectField != null)
                //    GUI.enabled = false;
                fromObjectField = EditorGUILayout.ObjectField(fromObjectField, typeof(UnityEngine.Object), true);


                // Enable GUI for rest
                //if (fromObjectField != null)
                //    GUI.enabled = true;
                // Link visual object to given visual node, only once
                //if (temp.visual == null)
                //{
                temp.visual = fromObjectField;
                temp.LoadComponents();

                //}

                if (temp.visual != null)
                {
                    //VisualObject vo = new VisualObject(fromObjectField.name);
                    //
                    //if (!visualObjects.Contains(vo))
                    //{
                    //    visualObjects.Add(vo);
                    //    //Debug.Log("adding vo");
                    //}

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
                            ////TODO: loop through all visualobjects and access their info (will possibly remove this)
                            //foreach (VisualObject currentVisualObject in visualObjects)
                            //{

                            // For most components
                            foreach (PropertyInfo pi in component.GetType().GetProperties())
                            {
                                // Only use objects of type Vector3, float, int.
                                if (pi.PropertyType == typeof(Vector3) || pi.PropertyType == typeof(float) ||
                                    pi.PropertyType == typeof(int))
                                {
                                    // Create new custom property info object
                                    CustomPropertyInfo customPi = new CustomPropertyInfo(pi, component.gameObject.name);

                                    // Add each property to list of properties
                                    if ((propertyInfo.ContainsKey(customPi)))
                                        // Overwrite if already contained
                                        propertyInfo[customPi] = component;
                                    else
                                        // Add if not
                                        propertyInfo.Add(customPi, component);
                                    //Debug.Log(propertyInfo.Keys.Count);

                                    //if (!(currentVisualObject.propertyInfo.ContainsKey(pi)))
                                    //    currentVisualObject.propertyInfo.Add(pi, component);

                                    //Debug.Log(currentVisualObject.propertyInfo);
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
                                    if (fieldInfo.ContainsKey(fi))
                                        fieldInfo[fi] = component;
                                    else
                                        fieldInfo.Add(fi, component);

                                    //if (!(currentVisualObject.fieldInfo.ContainsKey(fi)))
                                    //    currentVisualObject.fieldInfo.Add(fi, component);
                                }
                            }
                            //}
                        }
                    }
                }
            }
            else if (windows[id] is VisualNode)
            {
                // Cast and add TextField for value
                VisualNode temp = (VisualNode)windows[id];
                //temp.UpdateVisual(propertyInfo);
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
            else if (windows[id] is OperatorNode)
            {
                OperatorNode temp = (OperatorNode)windows[id];

                //float.TryParse(GUILayout.TextField(temp.modifier.ToString()), out temp.modifier);
                //temp.modifier = EditorGUI.FloatField(new Rect(windows[id].rectangle.width - 60, windows[id].rectangle.height - 25, 50, 20), temp.modifier);
                temp.modifier = EditorGUILayout.FloatField("Modifier:", temp.modifier);

                //Transform selectedObj = Selection.activeTransform;

                temp.currentOperator = (Operators)EditorGUILayout.EnumPopup(
                    "Type",
                    temp.currentOperator);

                //temp.UpdateState(temp.currentOperator);

                // Show output
                EditorGUILayout.FloatField("Output:", temp.output);

            }
            else if (windows[id] is MaterialNode)
            {
                MaterialNode temp = (MaterialNode)windows[id];

                temp.methodToSearch = EditorGUILayout.TextField(temp.methodToSearch);
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

        // Detect if cursor is on line.
        bool DetectCursorLineDetection(Vector3 point, Rect start, Rect end)
        {
            Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;

            float res = HandleUtility.DistancePointBezier(point, startPos, endPos, startTan, endTan);
            if (res < 9)
                return true;

            return false;
        }    

        private Rect HorizResizer(Rect window, bool right = true, float detectionRange = 8f)
        {
            detectionRange *= 0.5f;
            Rect resizer = window;

            if (right)
            {
                resizer.xMin = resizer.xMax - detectionRange;
                resizer.xMax += detectionRange;
            }
            else
            {
                resizer.xMax = resizer.xMin + detectionRange;
                resizer.xMin -= detectionRange;
            }
            Event current = Event.current;
            EditorGUIUtility.AddCursorRect(resizer, MouseCursor.ResizeHorizontal);

            // if mouse is no longer dragging, stop tracking direction of drag
            if (current.type == EventType.MouseUp)
            {
                draggingLeft = false;
                draggingRight = false;
            }

            // resize window if mouse is being dragged within resizor bounds
            if (current.mousePosition.x > resizer.xMin &&
                current.mousePosition.x < resizer.xMax &&
                current.type == EventType.MouseDrag &&
                current.button == 0 ||
                draggingLeft ||
                draggingRight)
            {
                if (right == !draggingLeft)
                {
                    window.width = current.mousePosition.x + current.delta.x;
                    Repaint();
                    draggingRight = true;
                }
                else if (!right == !draggingRight)
                {
                    window.width = window.width - (current.mousePosition.x + current.delta.x);
                    Repaint();
                    draggingLeft = true;
                }

            }

            return window;
        }
        private Rect VertResizer(Rect window, bool up = true, float detectionRange = 8f)
        {
            detectionRange *= 0.5f;
            Rect resizer = window;

            if (up)
            {
                resizer.yMin = resizer.yMax - detectionRange;
                resizer.yMax += detectionRange;
            }
            else
            {
                resizer.yMax = resizer.yMin + detectionRange;
                resizer.yMin -= detectionRange;
            }

            Event current = Event.current;
            EditorGUIUtility.AddCursorRect(resizer, MouseCursor.ResizeVertical);

            // if mouse is no longer dragging, stop tracking direction of drag
            if (current.type == EventType.MouseUp)
            {
                draggingUp = false;
                draggingDown = false;
            }

            // resize window if mouse is being dragged within resizor bounds
            if (current.mousePosition.y > resizer.yMin &&
                current.mousePosition.y < resizer.yMax &&
                current.type == EventType.MouseDrag &&
                current.button == 0 ||
                draggingUp ||
                draggingDown)
            {
                if (up == !draggingUp)
                {
                    window.height = current.mousePosition.y + current.delta.y;
                    Repaint();
                    draggingUp = true;
                }
                else if (!up == !draggingDown)
                {
                    window.height = window.height - (current.mousePosition.y + current.delta.y);
                    Repaint();
                    draggingDown = true;
                }

            }

            return window;
        }

        // This handles the incoming max messages and sends it to the correct audio node.
        public void AllMessageHandler(OscMessage oscMessage)
        {
            // Where do send it too.
            string[] incAddress = oscMessage.Address.Split('/');
            // Value it contains.
            float incValue = (float)oscMessage.Values[0];
            // Search each audio node to find where to send it.
            for (int i = 0; i < windows.Count; i++)
                // First check if correct class type.
                if (windows[i] is AudioNode)
                    // If windows contains name to this address. - Make it more accurate.
                    if (incAddress[1].ToLower().Equals(windows[i].nodeName.ToLower()))
                        windows[i].value = (float)incValue;
        }


        // Serialize nodes and save to file - Add option to what file later.
        public void SaveWindow(object obj)
        {
            NodeManager saveData = new NodeManager(this);
            NodeEditorUtil.SaveFile(saveData);
            Debug.Log("File saved");
        }


        // Load serialized file - Add option what file later.
        public void LoadWindow(object obj)
        {
            // Load file
            NodeManager load = NodeEditorUtil.LoadFile();
            //if suitable file.
            if (load != null)
            {
                // Reset variables 
                attachedWindows = load.attachedWindows;
                windows = new List<Node>();
                foreach (AudioNode x in load.auNodes)
                    windows.Add(x);
                foreach (VisualNode x in load.viNodes)
                    windows.Add(x);
                foreach (OperatorNode x in load.oNodes)
                    windows.Add(x);
                foreach (ControllerNode x in load.cNodes)
                    windows.Add(x);
                foreach (RandomGeneratorNode x in load.rNodes)
                    windows.Add(x);
                foreach (MaxNode x in load.mNodes)
                    windows.Add(x);
                foreach (MaterialNode x in load.matNodes)
                    windows.Add(x);
                uniqueNodeId = load.uniqueNodeId;
                windowsToAttach = load.windowsToAttach;
                Debug.Log("File loaded!");
            }
        }
    }
}

