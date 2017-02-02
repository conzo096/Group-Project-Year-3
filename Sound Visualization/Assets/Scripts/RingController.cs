using UnityEngine;
using System.Collections;
/* RingController.cs by Emmanuel Miras. Last edited 26/01/17
 * Attach to GameObject with a particle system. 
 * Controls a ring shaped particle system
*/

public class RingController : MonoBehaviour
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
        ringLifeTime = 1f;
        ringRadius = 10f;
        //ringInstances = 1;
        CreateRingSetup();

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
        // Loop through all particle systems and adjust ring radius
        foreach (ParticleSystem psCurrent in ps)
        {
            var sh = psCurrent.shape;
            sh.radius = ringRadius;

            psCurrent.startLifetime = ringLifeTime;
        }


        //for (int i = 0; i < psShape.Length; i++)
        //{
        //    Debug.Log(i);
        //    psShape[i].radius = ringRadius;
        //}

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
