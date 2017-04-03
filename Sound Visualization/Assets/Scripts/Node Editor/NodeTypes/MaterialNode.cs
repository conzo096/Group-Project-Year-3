/* A material node is similar to a visual node, it's necessary to exist as the visual node 
 * cannot easilly access the material of the renderer component without much modification. */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NodeEditor
{
    [Serializable]
    public class MaterialNode : Node
    {
        public Material material;
        public string methodToSearch;
        public MaterialNode(Rect r, string name, int index)
        {
            rectangle = r;
            nodeName = name;
            id = index;
        }

        public void PassValueToMaterial()
        {
            if (material != null && methodToSearch != null)
                material.SetFloat("_" + methodToSearch, value);
        }
    }
}
