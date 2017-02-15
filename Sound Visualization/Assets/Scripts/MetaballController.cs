using UnityEngine;
using System.Collections;

public class MetaballController : MonoBehaviour {
    [Range(1, 100)]
    // Number of rings
    public int metaballInstances;
    public GameObject metaball;
    // Use this for initialization
    void Start ()
    {
        // Set to 1 if 0
        if (metaballInstances == 0)
            metaballInstances = 1;

        for (int i = 0; i < metaballInstances; i++)
        {
            GameObject childObject = Instantiate(metaball) as GameObject;
            childObject.transform.parent = this.transform;
            childObject.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
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
                childObject.transform.localPosition = new Vector3(0f, 0f, 0f);
            }
        }

        // Destroy extra metaballs
        if (transform.childCount > metaballInstances)
        {
            // Number of metaballs to be deleted
            int instancesToDelete = transform.childCount - metaballInstances;

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
}
