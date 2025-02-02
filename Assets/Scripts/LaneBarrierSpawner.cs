using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneBarrierSpawner : MonoBehaviour
{
    public GameObject cylinder;
    public float minTimeToSpawn = 1f;
    public float maxTimeToSpawn = 3.5f;

    private GameObject singleLaneBarrierPrefab;
    private GameObject doubleLaneBarrierPrefabVarian1;
    private GameObject doubleLaneBarrierPrefabVariant2;
    private GameObject tripleLaneBarrierPrefabVariant1;
    private GameObject tripleLaneBarrierPrefabVariant2;
    private ObjectToSpawn objectToSpawn;

    private Spawner barrierSpawner = new BarrierSpawner();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Invoke("SpawnBarrier", Random.Range(minTimeToSpawn, maxTimeToSpawn));
    }

    void SpawnBarrier()
    {
        // TODO Choose a barrier to spawn but keep in mind that not all barriers might be available
        // because the right most lane can only instatiate single lane barriers, for example


        // Fire a ray into the cylinder and spawn an object that will fall towards the cylinder
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity) && hit.collider.CompareTag(Tags.Ground))
        {
            if (singleLaneBarrierPrefab != null)
            {
                var obstacle = Instantiate(singleLaneBarrierPrefab);
                barrierSpawner.SpawnObject(cylinder, transform.position, hit, obstacle);
            }
        }
    }

    enum ObjectToSpawn
    {
        SingleLane,
        DoubleLaneVariant1,
        DoubleLaneVariant2,
        TripleLaneVariant1,
        TripleLaneVariant2
    }
}
