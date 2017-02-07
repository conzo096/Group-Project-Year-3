/* Created by Conner Weatherston 40167111
 * Last edited: 06/02/2017
 * The purpose of this class is to store all the possible attributes of a mesh that can be changed.
 * This should be used with the GUI to determine what music parameter affects the mesh.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifiableAttributes
{
    // Scale value of the mesh.
    private Vector3 scale = new Vector3(0,0,0);
    // Rotation of the mesh.
    private Vector3 rotation = new Vector3(0,0,0);
    // Position of the mesh.
    private Vector3 position = new Vector3(0,0,0);

    // Attributes that can be changed for particle ring.
    // Speed particles move at.
    private float particleSpeed = 0;
    // Size of smoke ring.
    private float ringRadius = 0;
    // Instances of particles.
    private int particleInstances = 0;


    public ModifiableAttributes()
    {

    }

    // Accessors for the class variables.
    public Vector3 Scale
    {
        get
        {
            return scale;
        }
        set
        {
           
            scale = value;
        }
    }


    public Vector3 Rotation
    {
        get
        {
            return rotation;
        }
        set
        {
            // Do we want minus values to be inputted?
           rotation= value;
        }
    }

    public Vector3 Position
    {
        get
        {
            return position;
        }

        set
        {
            position = value;
        }
    }

    public float ParticleSpeed
    {
        get
        {
            return particleSpeed;
        }

        set
        {
            particleSpeed = value;
        }
    }

    public float RingRadius
    {
        get
        {
            return ringRadius;
        }

        set
        {
            ringRadius = value;
        }
    }

    public int ParticleInstances
    {
        get
        {
            return particleInstances;
        }

        set
        {
            particleInstances = value;
        }
    }
}
