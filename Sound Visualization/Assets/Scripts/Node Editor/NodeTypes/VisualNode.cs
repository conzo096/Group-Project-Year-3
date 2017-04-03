using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace NodeEditor
{   // Visual node, interacts with components of a gameobject provided by the controller node.
    [Serializable]
    public class VisualNode : Node
    {
        // Used to track down type of value
        public PropertyInfo propertyInfo;
        public FieldInfo fieldInfo;
        public object compObj;
        public string parent;
        // Tracks which properties of a Vector3 to change
        public bool[] Vectors = new bool[3];

        public VisualNode(Rect r, string name, string parent, object value, int index)
        {
            rectangle = r;
            this.nodeName = name;
            this.parent = parent;
            id = index;
            //this.value = value;
        }

        public VisualNode()
        {
        }

        // Updates the variables of a component
        public void UpdateVisual(Dictionary<CustomPropertyInfo, Component> propertyInfoDictionary)
        {

            // Temp value
            //this.value = Random.Range(0f, 1f);//new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            if (this.value != null)
            {

                List<CustomPropertyInfo> propertyInfo = new List<CustomPropertyInfo>(propertyInfoDictionary.Keys);
                for (int i = 0; i < propertyInfo.Count; i++)
                {

                    if (propertyInfo[i].propertyInfo.Name == this.nodeName && propertyInfo[i].parent == this.parent)
                    {
                        Component comp;
                        propertyInfoDictionary.TryGetValue(propertyInfo[i], out comp);
                        compObj = (System.Object)comp;

                        // Save propertyInfo to track down type later on
                        this.propertyInfo = propertyInfo[i].propertyInfo;

                        // Update as vector
                        if (propertyInfo[i].propertyInfo.PropertyType == typeof(Vector3))
                        {
                            // Cast
                            Vector3 vector3Value = (Vector3)propertyInfo[i].propertyInfo.GetValue(compObj, null);

                            // Loop through each vector attribute
                            for (int j = 0; j < 3; j++)
                            {
                                // If true, overwrite current value with the incoming value
                                if (Vectors[j])
                                    vector3Value[j] = (float)this.value;

                            }
                            // Set the value
                            propertyInfo[i].propertyInfo.SetValue(compObj, vector3Value, null);
                        }
                        // Update as int
                        else if (propertyInfo[i].propertyInfo.PropertyType == typeof(int))
                        {
                            // Cast
                            int intValue = (int)this.value;
                            // Set the value
                            propertyInfo[i].propertyInfo.SetValue(compObj, intValue, null);
                        }
                        // Default
                        else
                        {
                            propertyInfo[i].propertyInfo.SetValue(compObj, this.value, null);
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
                            int intValue = System.Convert.ToInt32(this.value);
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
}