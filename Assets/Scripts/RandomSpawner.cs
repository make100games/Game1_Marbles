using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Moves around randomly and spawns a bunch of objects at a time. Can also spawn
// a surface-level object like a ramp
public class RandomSpawner : MonoBehaviour
{
    public GameObject cylinder;
    public GameObject rampPrefab;
    public GameObject cratePrefab;
    public GameObject roadClosedPrefab;
    public GameObject spoolPrefab;
    public GameObject barrelPrefab;
    public GameObject bombPrefab;
    public bool spawnGroups = true;    // If true, it will spawn multiple objects at once
    private float cylinderWidth;
    private Spawner coinSpawner = new CoinSpawner();
    private Spawner rampSpawner = new RampSpawner();
    private Spawner obstacleSpawner = new ObstacleSpawner();
    private Spawner barrelSpawner = new BarrelSpawner();
    private Spawner bombSpawner = new BombSpawner();
    private Spawner spoolSpawner = new SpoolSpawner();

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
            if(spawnGroups)
            {
                var numberOfObstacles = Random.Range(2, 4);
                for (int i = 0; i < numberOfObstacles; i++)
                {
                    var random = Random.Range(0, 5);
                    if (random == 0)
                    {
                        if (cratePrefab != null)
                        {
                            var obstacle = Instantiate(cratePrefab);
                            obstacleSpawner.SpawnObject(cylinder, transform.position, hit, obstacle, true, true, true);
                        }
                    }
                    else if (random == 1)
                    {
                        if (spoolPrefab != null)
                        {
                            var obstacle = Instantiate(spoolPrefab);
                            spoolSpawner.SpawnObject(cylinder, transform.position, hit, obstacle, true, true, true);
                        }
                    }
                    else if (random == 2)
                    {
                        if (barrelPrefab != null)
                        {
                            var obstacle = Instantiate(barrelPrefab);
                            barrelSpawner.SpawnObject(cylinder, transform.position, hit, obstacle, true, true, true);
                        }
                    }
                    else if (random == 3)
                    {
                        if (roadClosedPrefab != null)
                        {
                            var obstacle = Instantiate(roadClosedPrefab);
                            obstacleSpawner.SpawnObject(cylinder, transform.position, hit, obstacle, false, true, true);
                        }
                    }
                    else if (random == 4)
                    {
                        if (bombPrefab != null)
                        {
                            var obstacle = Instantiate(bombPrefab);
                            bombSpawner.SpawnObject(cylinder, transform.position, hit, obstacle, false, true, true);
                        }
                    }
                }
            }
            else
            {
                if(rampPrefab != null)
                {
                    var ramp = Instantiate(rampPrefab);
                    rampSpawner.SpawnObject(cylinder, transform.position, hit, ramp);
                }
            }
        }
        spawn = true;
    }
 }
