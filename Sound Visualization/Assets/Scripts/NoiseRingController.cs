/* NoiseRingController.cs
 * Attach to ring (or any other object really) with procedural material 
 * Allows for editing the parameters of the procedural material, but also the transforms of the object */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum OscType
{
    Sin, Cos
}

public class NoiseRingController : MonoBehaviour {
    /* Position */
    [Header("Position modulators")]
    // X Axis
    [Range(0, 1)]
    public float xPositionModulator;
    // Y Axis
    [Range(0, 1)]
    public float yPositionModulator;
    // Z Axis
    [Range(0, 1)]
    public float zPositionModulator;
    [Range(1, 100)]
    public float positionAmplitude;

    public OscType positionOscillationType;

    /* Scale */
    [Header("Scale modulators")]
    // X Axis
    [Range(0, 1)]
    public float xScaleModulator;
    // Y Axis
    [Range(0, 1)]
    public float yScaleModulator;
    // Z Axis
    [Range(0, 1)]
    public float zScaleModulator;
    [Range(1, 100)]
    public float scaleAmplitude;

    public OscType scaleOscillationType;

    /* Rotation */
    [Header("Rotation modulators")]
    // X Axis
    [Range(0, 1)]
    public float xRotationModulator;
    // Y Axis
    [Range(0, 1)]
    public float yRotationModulator;
    // Z Axis
    [Range(0, 1)]
    public float zRotationModulator;
    [Range(1, 100)]
    public float rotationAmplitude;

    public OscType rotationOscillationType;
    // The children of the current object
    private Transform[] childTransform;
    // Use this for initialization
    void Start ()
    {
        // Grab all of the children
        childTransform = GetComponentsInChildren<Transform>();

        // Avoid division by 0
        if (positionAmplitude == 0f)
            positionAmplitude = 1f;
        if (scaleAmplitude == 0f)
            scaleAmplitude = 1f;
        if (rotationAmplitude == 0f)
            rotationAmplitude = 1f;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Apply all transformations
        UpdatePosition();
        UpdateScale();
        UpdateRotation();
    }

    public void UpdatePosition()
    {
        float oscillation = 0;

        switch (positionOscillationType)
        {
            case OscType.Cos:
                oscillation = (Mathf.Cos(Time.time) / positionAmplitude);
                break;
            case OscType.Sin:
                oscillation = (Mathf.Sin(Time.time) / positionAmplitude);
                break;
            default:
                break;
        }

        float xPosition = oscillation * xPositionModulator / 100f;
        float yPosition = oscillation * yPositionModulator / 100f;
        float zPosition = oscillation * zPositionModulator / 100f;

        // Apply translation
        transform.Translate(new Vector3(xPosition, yPosition, zPosition));
    }

    public void UpdateScale()
    {
        float oscillation = 0;

        switch (scaleOscillationType)
        {
            case OscType.Cos:
                oscillation = (Mathf.Cos(Time.time) / scaleAmplitude);
                break;
            case OscType.Sin:
                oscillation = (Mathf.Sin(Time.time) / scaleAmplitude);
                break;
            default:
                break;
        }

        float xScale = (oscillation * xScaleModulator) / 100f;
        float yScale = (oscillation * yScaleModulator) / 100f;
        float zScale = (oscillation * zScaleModulator) / 100f;
        // Apply scale
        transform.localScale += new Vector3(xScale, yScale, zScale);
    }   
        
    public void UpdateRotation()
    {
        float oscillation = 0;

        switch (rotationOscillationType)
        {
            case OscType.Cos:
                oscillation = (Mathf.Cos(Time.time / rotationAmplitude));
                break;
            case OscType.Sin:
                oscillation = (Mathf.Sin(Time.time / rotationAmplitude));
                break;
            default:
                break;
        }
        float xRotation = (oscillation * xRotationModulator) * Mathf.PI;
        float yRotation = (oscillation * yRotationModulator) * Mathf.PI;
        float zRotation = (oscillation * zRotationModulator) * Mathf.PI;
        // Apply rotation to 2nd child, because Unity counts the current object as a child for some reason...
        childTransform[1].Rotate(xRotation, yRotation, zRotation);
    }

}
