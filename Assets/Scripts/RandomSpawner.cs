using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Moves around randomly and spawns a bunch of objects at a time. Can also spawn
// a surface-level object like a ramp
public class RandomSpawner : MonoBehaviour
{
    public GameObject cylinder;
    public GameObject rampPrefab;
    public GameObject obstaclePrefab;
    public GameObject coinPrefab;
    private float cylinderWidth;
    private Spawner coinSpawner = new CoinSpawner();
    private Spawner obstacleSpawner = new ObstacleSpawner();
    private Spawner rampSpawner = new RampSpawner();

    private bool spawn = true;

    // Start is called before the first frame update
    void Start()
    {
        //InvokeRepeating("SpawnObject", 2f, 2f);
        var renderer = cylinder.GetComponentInChildren<MeshRenderer>();
        cylinderWidth = renderer.bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawn)
        {
            spawn = false;
            Invoke("SpawnObject", Random.Range(0.25f, 1f));
        }
    }

    void SpawnObject()
    {
        // Spawn objects by moving left or right a random distance (but not so far
        // as to go beyond the left or right edge of the Cylinder), looking at the
        // cylinder, firing a ray at it and spawning an object where the ray hit
        var cylinderX = cylinder.transform.position.x;
        var amountToMove = Random.Range(0f, (cylinderWidth / 2));
        var signOfMove = Random.Range(0f, 1f) >= 0.5 ? -1 : 1;
        var randomPositionOfSpawner = cylinderX + (amountToMove * signOfMove);
        transform.position = new Vector3(randomPositionOfSpawner, transform.position.y, transform.position.z);

        // Fire a ray into the cylinder and spawn an object that will fall towards the cylinder.
        // Make sure to only spawn an object if our raycast hit the ground and not another object
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity) && hit.collider.CompareTag(Tags.Ground))
        {
            var randomValue = Random.Range(1, 10);
            if(randomValue < 3)
            {
                if(rampPrefab != null)
                {
                    var ramp = Instantiate(rampPrefab);
                    rampSpawner.SpawnObject(cylinder, transform.position, hit, ramp);
                }
                
            }
            else if(randomValue < 7)
            {
                if(obstaclePrefab != null)
                {
                    var numberOfObstacles = Random.Range(2, 4);
                    for (int i = 0; i < numberOfObstacles; i++)
                    {
                        var obstacle = Instantiate(obstaclePrefab);
                        obstacleSpawner.SpawnObject(cylinder, transform.position, hit, obstacle);
                    }
                }
            }
            else
            {
                if(coinPrefab != null)
                {
                    var coin = Instantiate(coinPrefab);
                    coinSpawner.SpawnObject(cylinder, transform.position, hit, coin);
                }
            }
        }
        spawn = true;
    }
}
