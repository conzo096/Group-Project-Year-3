﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
// Visual node, interacts with components of a gameobject provided by the controller node.
namespace Assets.Scripts.Node_Editor
{// Visual node, interacts with components of a gameobject provided by the controller node.
    public class VisualNode : Node
    {
        // Used to track down type of value
        public PropertyInfo propertyInfo;
        public FieldInfo fieldInfo;
        public System.Object compObj;

        // Tracks which properties of a Vector3 to change
        public bool[] Vectors = new bool[3];

        public VisualNode(Rect r, string name, object value, int index)
        {
            rectangle = r;
            this.nodeName = name;
            id = index;
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