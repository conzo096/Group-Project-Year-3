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
        public static Color col = new Color();
        public static Color emissionCol = new Color();
        public Material material;
        public string methodToSearch;
        // Tracks which properties of Color/Vector4 to change (if needed).
        public bool[] Vectors = new bool[4];
        // Used to pass color infomation to the shader.
        public MaterialNode(Rect r, string name, int index)
        {
            rectangle = r;
            nodeName = name;
            id = index;
        }

        public void PassValueToMaterial()
        {
            // Send correct information.
            if (material != null && methodToSearch != null)
            {
                // All color methods contain the word color.
                if (methodToSearch.ToLower().Contains("color")) 
                {

                    if (methodToSearch.ToLower().Contains("emissioncolor"))
                    {
                        // Update selected options.
                        for (int i = 0; i < 4; i++)
                            if (Vectors[i] == true)
                            {
                                //emissionCol[i] = Mathf.Lerp(emissionCol[i],value, Time.deltaTime);
                                emissionCol[i] = value;
                            }
                        material.SetColor("_" + methodToSearch, emissionCol);
                    }
                    else
                    {
                        // Update selected options.
                        for (int i = 0; i < 4; i++)
                            if (Vectors[i] == true)
                            {
                                //col[i] = Mathf.Lerp(col[i], value, Time.deltaTime);
                                 col[i] = value;
                            }
                        material.SetColor("_" + methodToSearch, col);
                    }
                    
                }
                // Need to check if it is a vector value.

                // Now only option left for editor is float.
                else
                    material.SetFloat("_" + methodToSearch, value);
            }
        }
    }
}
