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
            this.nodeName = name;
            id = index;
        }

        public void PassValueToMaterial()
        {
            if (material != null && methodToSearch != null && value != null)
                material.SetFloat("_" + methodToSearch, (float)value);
        }
    }
}
