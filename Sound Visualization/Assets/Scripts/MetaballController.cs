/* Controls the movement and spawning of metaball objects */
using UnityEngine;
using System.Collections;

public class MetaballController : MonoBehaviour {
    // Metaball prefab
    public GameObject metaball;
    // Metaball prefab that is at the center
    public GameObject metaballCenter;

    [Range(1, 100)]
    // Number of metaballs
    public int metaballInstances;
    [Range(-1, 1)]
    public float metaballSpeed;
    // Use this for initialization
    void Start ()
    {
        // Set to 1 if 0
        if (metaballInstances == 0)
            metaballInstances = 1;

        // Put the main metaball in the center
        GameObject center = Instantiate(metaballCenter) as GameObject;
        center.transform.parent = this.transform;
        center.transform.localPosition = new Vector3(0f, 0f, 0f);
        // Instantiate other metaballs
        for (int i = 0; i < metaballInstances; i++)
        {
            GameObject childObject = Instantiate(metaball) as GameObject;
            childObject.transform.parent = this.transform;
            childObject.transform.localPosition = new Vector3(0.5f, 0f, 0f);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        // Between 1 and 100 metaballs
        metaballInstances = Mathf.Clamp(metaballInstances, 1, 100);
        metaballSpeed = Mathf.Clamp(metaballSpeed, -1f, 1f);
        // Add missing metaballs
        if (transform.childCount < metaballInstances)
        {
            // Number of metaballs to be created
            int instancesToCreate = metaballInstances - transform.childCount;
            // Create metaballs as children
            for (int i = 0; i < instancesToCreate; i++)
            {
                GameObject childObject = Instantiate(metaball) as GameObject;
                childObject.transform.parent = this.transform;
                childObject.transform.localPosition = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
            }
        }

        // Destroy extra metaballs
        if (transform.childCount > metaballInstances)
        {
            // Number of metaballs to be deleted
            int instancesToDelete = transform.childCount - metaballInstances;

            foreach (Transform child in transform)
            {
                if (instancesToDelete == 0 || child.tag.Equals("MetaballCenter"))
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

        // Update each metaball's speed
        BouncingBall[] metaballs = GetComponentsInChildren<BouncingBall>();
        foreach (BouncingBall metaball in metaballs)
        {
            metaball.speed = metaballSpeed;
        }
    }

    
}
