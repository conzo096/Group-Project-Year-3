using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualNode : BaseNode
{
    // List of nodes that connect to this node.
    private ArrayList fromNodes;
    
    void Start()
    {
        NodeName = "Visual";
        gameObject.name = NodeName;
        // Create child that has text field.
        GameObject txtField = new GameObject();
        txtField.name = "Text Field";
        txtField.transform.SetParent(gameObject.transform);

        txtField.AddComponent<TextMesh>();
        TextMesh meshName = txtField.GetComponent<TextMesh>();
        meshName.text = NodeName;
        meshName.color = Color.black;
        meshName.characterSize = 0.3f;
        meshName.anchor = TextAnchor.LowerCenter;
    }
}
