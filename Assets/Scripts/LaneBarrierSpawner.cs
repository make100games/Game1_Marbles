using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class LaneBarrierSpawner : MonoBehaviour
{
    public GameObject cylinder;

    private Spawner barrierSpawner = new BarrierSpawner();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnBarrier(GameObject laneBarrierToSpawn)
    {
        // Fire a ray into the cylinder and spawn an object that will fall towards the cylinder
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity) && hit.collider.CompareTag(Tags.Ground))
        {
            if (laneBarrierToSpawn != null)
            {
                var obstacle = Instantiate(laneBarrierToSpawn);
                barrierSpawner.SpawnObject(cylinder, transform.position, hit, obstacle);
            }
        }
    }
}
