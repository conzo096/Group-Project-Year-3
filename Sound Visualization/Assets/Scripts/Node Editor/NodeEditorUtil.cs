using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System;

namespace NodeEditor
{
    // Utility class for Node Editor
    public class NodeEditorUtil
    {

        // Save the current node editor. 
        public static void SaveFile(NodeManager saveData)
        {
            // Convert manager to json format.
            string json = EditorJsonUtility.ToJson(saveData);
            // Get save path from user.
            string filePath = EditorUtility.SaveFilePanel("Saving editor", "..//Assets//Saves", "Graph", "Sav");
            if (filePath.Length != 0)
            {
                // Write file to file path.
                File.WriteAllText(filePath, json);
            }
        }

        // Load Node editor from file.
        public static NodeManager LoadFile()
        {
            // Get file path from user.
            string filePath = EditorUtility.OpenFilePanel("Saves", ".//Assets//Saves", "Sav");
            if (filePath.Length != 0)
            {
                // Read contents from file path.
                string json = File.ReadAllText(filePath);
                // COnvert json message into NodeManager class.
                NodeManager load = new NodeManager();
                EditorJsonUtility.FromJsonOverwrite(json, load);
                return load;
            }
            return null;
        }
    }
}

