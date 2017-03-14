using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioNode : BaseNode
{
    // List of connections that the value will get sent to.
    private ArrayList toNodes;

    void Start()
    {
        NodeName = "Audio";
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

        // Add output object.
        GameObject outConnector = GameObject.CreatePrimitive(PrimitiveType.Quad);
        outConnector.name = "Out Connection";
        outConnector.AddComponent<Collider2D>();
        outConnector.transform.SetParent(gameObject.transform);
        outConnector.transform.Translate(3, 2, 0);

    }
}
