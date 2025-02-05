using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class LaneBarrierSpawnerSystem : MonoBehaviour
{
    public List<GameObject> laneBarrierSpawners = new List<GameObject>();
    public float minTimeToSpawn = 1f;
    public float maxTimeToSpawn = 3.5f;
    public GameObject singleLaneBarrierPrefab;
    public GameObject doubleLaneBarrierPrefabVariant1;
    public GameObject doubleLaneBarrierPrefabVariant2;
    public GameObject tripleLaneBarrierPrefabVariant1;
    public GameObject tripleLaneBarrierPrefabVariant2;

    /** The config is to be read as follows:
     * The first entry represents the barrier to be spawned on the left most lane.
     * The next entry represents the barrier to be spawned on the lane to the right of that and
     * so on.
     */
    private LaneBarrierSet[] barrierConfigs = new LaneBarrierSet[]
    {
        new LaneBarrierSet(new LaneBarrierType[] { LaneBarrierType.SingleLane, LaneBarrierType.Nothing, LaneBarrierType.Nothing, LaneBarrierType.Nothing, LaneBarrierType.SingleLane }),
        new LaneBarrierSet(new LaneBarrierType[] { LaneBarrierType.Nothing, LaneBarrierType.Nothing, LaneBarrierType.SingleLane, LaneBarrierType.Nothing, LaneBarrierType.SingleLane }),
        new LaneBarrierSet(new LaneBarrierType[] { LaneBarrierType.Nothing, LaneBarrierType.Nothing, LaneBarrierType.Nothing, LaneBarrierType.Nothing, LaneBarrierType.SingleLane }),
        new LaneBarrierSet(new LaneBarrierType[] { LaneBarrierType.SingleLane, LaneBarrierType.Nothing, LaneBarrierType.Nothing, LaneBarrierType.Nothing, LaneBarrierType.Nothing })
    };

    // Start is called before the first frame update
    void Start()
    {
        Invoke("SpawnBarriers", UnityEngine.Random.Range(minTimeToSpawn, maxTimeToSpawn));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnBarriers()
    {
        // Grab a random barrier configuration
        var indexOfBarrierSet = UnityEngine.Random.Range(0, barrierConfigs.Length);
        var barrierSet = barrierConfigs[indexOfBarrierSet];

        // The number of spawners must match the number of LaneBarrierTypes in each row of the barrierConfigs
        if(barrierSet.config.Length != laneBarrierSpawners.Count)
        {
            throw new InvalidOperationException("The number of barrier spawners (" + laneBarrierSpawners.Count + ") must match the number of LaneBarrierTypes (" + barrierSet.config.Length + ") in each row of the barrierConfigs");
        }

        // Tell the spawners to spawn the object defined in the barrier set
        var index = 0;
        foreach (LaneBarrierType barrier in barrierSet.config)
        {
            var barrierPrefab = BarrierPrefabFor(barrier);
            if(barrierPrefab != null)
            {
                laneBarrierSpawners[index].GetComponent<LaneBarrierSpawner>().SpawnBarrier(barrierPrefab);
            }
            index++;
        }

        Invoke("SpawnBarriers", UnityEngine.Random.Range(minTimeToSpawn, maxTimeToSpawn));
    }

    private GameObject BarrierPrefabFor(LaneBarrierType barrierType)
    {
        switch(barrierType)
        {
            case LaneBarrierType.SingleLane:
                return singleLaneBarrierPrefab;
            case LaneBarrierType.DoubleLaneVariant1:
                return doubleLaneBarrierPrefabVariant1;
            case LaneBarrierType.DoubleLaneVariant2:
                return doubleLaneBarrierPrefabVariant2;
            case LaneBarrierType.TripleLaneVariant1:
                return tripleLaneBarrierPrefabVariant1;
            case LaneBarrierType.TripleLaneVariant2:
                return tripleLaneBarrierPrefabVariant2;
            case LaneBarrierType.Nothing:
                return null;
        }
        return null;
    }

    enum LaneBarrierType
    {
        Nothing,
        SingleLane,
        DoubleLaneVariant1,
        DoubleLaneVariant2,
        TripleLaneVariant1,
        TripleLaneVariant2
    }

    struct LaneBarrierSet
    {
        public LaneBarrierType[] config;

        public LaneBarrierSet(LaneBarrierType[] config)
        {
            this.config = config;
        }
    }
}
