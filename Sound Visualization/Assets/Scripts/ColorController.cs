/* Used to control color information to the shader.*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ColorController : MonoBehaviour
{
    // The material of connected object.
    private Material mat;
    private Color setCol;
    private Color setEmissionCol;

    // Basic colour.
    public float colX;
    public float colY;
    public float colZ;
    public float colW;
    // Emission colour.
    public float emissionColX;
    public float emissionColY;
    public float emissionColZ;
    public float emissionColW;

    
    // Use this for initialization
    void Start ()
    {
        // initilize values.
        mat = GetComponent<Renderer>().material;
        if (mat != null)
        {
            Color col = mat.GetColor("_Color");
            colX = col.r;
            colY = col.g;
            colZ = col.b;
            colW = col.a;

            Color emissionCol = mat.GetColor("_EmissionColor");
            emissionColX = emissionCol.r;
            emissionColY = emissionCol.g;
            emissionColZ = emissionCol.b;
            emissionColW = emissionCol.a;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        // Update colors.
        setCol = new Color(colX, colY, colZ, colW);
        setEmissionCol = new Color(emissionColX, emissionColY, emissionColZ, emissionColW);

        // Send color to info.
        GetComponent<Renderer>().material.SetColor("_Color", setCol);
        GetComponent<Renderer>().material.SetColor("_EmissionColor", setEmissionCol);
    }
}
