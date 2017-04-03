/* Controller node, used to generate visual nodes based on provided gameobjects. */
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NodeEditor
{
    
    [Serializable]
    public class ControllerNode : Node
    {
        //public List<bool> componentsChecked = new List<bool>();
        public UnityEngine.Object visual;
        public Dictionary<Component, bool> componentsDictionary = new Dictionary<Component, bool>();//public Component[] components;
                                                                                                    //private NoiseRingController controller;
        public string gameObjectTag = "GameObject Tag";

        public ControllerNode(Rect r, string name, int index)
        {
            rectangle = r;
            id = index;
            nodeName = name;
        }

        public ControllerNode()
        {
        }

        public void LoadComponents()
        {

            if (visual != null)
            {
                GameObject temp = (GameObject)visual;

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
}
