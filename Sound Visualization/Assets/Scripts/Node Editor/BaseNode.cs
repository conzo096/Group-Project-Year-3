using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseNode : MonoBehaviour
{
    // Name of node.
    private string nodeName = "Base";
    // Data value that will be passed to connected nodes.
    private object nodeValue;

    // Setters and Getters.
    public string NodeName
    {
        get
        {
            return nodeName;
        }

        set
        {
            nodeName = value;
        }
    }

    public object NodeValue
    {
        get
        {
            return nodeValue;
        }

        set
        {
            nodeValue = value;
        }
    }

    void Start()
    {
        gameObject.name = nodeName;
        // Create child that has text field.
        GameObject txtField = new GameObject();
        txtField.name = "Text Field";
        txtField.transform.SetParent(gameObject.transform);

        txtField.AddComponent<TextMesh>();    
        TextMesh meshName = txtField.GetComponent<TextMesh>();
        meshName.text = nodeName;
        meshName.color = Color.black;
        meshName.characterSize = 0.3f;
        meshName.anchor = TextAnchor.LowerCenter;

    }
}
