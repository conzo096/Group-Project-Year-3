﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Node_Editor
{
    // Controller node, used to create visual nodes based on a given gameobject
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
}
