/* SmokeRingController.cs
 * Attach to GameObject with a particle system. 
 * Controls a ring shaped particle system */

using UnityEngine;
using System.Collections;

public class SmokeRingController : MonoBehaviour
{
    [Range(0, 5)]
    public float positionModulator;
    [Range(10, 50)]
    public float ringRadius;
    [Range(1, 10)]
    public float ringLifeTime;

    [Range(1, 10)]
    // Number of rings
    public int ringInstances;
    // The ring particle system prefab
    public GameObject ring;
    // The particle system attached to the GameObject
    private ParticleSystem[] ps;
    // The shape of the particle system
    private ParticleSystem.ShapeModule[] psShape;

    //private Transform[] transforms;

    
    //private GameObject ringInstance;
	// Use this for initialization
	void Start ()
    {
        // Initialize
        ps = GetComponentsInChildren<ParticleSystem>();
        //transforms = GetComponentsInChildren<Transform>();
        //ringInstance = GetComponentInChildren<GameObject>();
        //getcomponents
        //ringLifeTime = 1f;
        //ringRadius = 10f;
        //ringInstances = 1;
        //CreateRingSetup();

        for (int i = 0; i < ringInstances; i++)
        {
            //Instantiate(ring);
            GameObject childObject = Instantiate(ring) as GameObject;
            childObject.transform.parent = this.transform;
        }

    }
	
	// Update is called once per frame
	void Update ()
    {
        // Keep getting a reference to the particle systems of the children (TODO: fix, probably costly)
        ps = GetComponentsInChildren<ParticleSystem>();

        // Loop through all particle systems and adjust ring radius
        foreach (ParticleSystem psCurrent in ps)
        {
            var sh = psCurrent.shape;
            sh.radius = ringRadius;

            psCurrent.startLifetime = ringLifeTime;
        }

        // Add missing rings
        if (transform.childCount < ringInstances)
        {
            // Number of rings to be created
            int instancesToCreate = ringInstances - transform.childCount;
            // Create rings as children
            for (int i = 0; i < instancesToCreate; i++)
            {
                
                GameObject childObject = Instantiate(ring) as GameObject;
                childObject.transform.parent = this.transform;
                int displacement = 0;//5 * transform.childCount;
                childObject.transform.Translate(new Vector3(0f, displacement, 0f));
            }
        }

        // Destroy extra rings
        if (transform.childCount > ringInstances)
        {
            // Number of rings to be deleted
            int instancesToDelete = transform.childCount - ringInstances;
            
            foreach (Transform child in transform)
            {
                if (instancesToDelete == 0)
                {
                    continue;
                }
                else
                {
                    Destroy(child.gameObject);
                    instancesToDelete--;
                }

            }
        }


    }

    // Sets the particle system to emit as a ring, if not already set-up
    void CreateRingSetup()
    {
        
        //GetComponentInChildren<Transform>().
        int i = 0;
        // Loop through all particlesystems
        foreach (ParticleSystem psCurrent in ps)
        {
            //transforms[i].rotation = Quaternion.Euler(new Vector3(-90f, 0f, 0f));
            //psShape.SetValue(psCurrent.shape, i);
            var sh = psCurrent.shape;
            sh.enabled = true;
            sh.shapeType = ParticleSystemShapeType.ConeShell;
            sh.angle = 0f;
            psCurrent.startLifetime = 1f;
            var emission = psCurrent.emission;
            emission.rate = 100f;

            i++;
        }
    }
}
