using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EverythingSpawner : MonoBehaviour
{
    public GameObject cylinder;
    public GameObject cratePrefab;
    public GameObject spoolPrefab;
    public GameObject barrelPrefab;
    public GameObject roadClosedPrefab;
    public GameObject bombPrefab;

    public float movementSpeed = 30f; // Speed in units per second at which the spawner moves from side to side
    public bool spawnObstacle;
    public float intervalBetweenObjects = 0.25f;   // Amount of seconds between each object spawn
    public float minTimeToSpawn = 1f;
    public float maxTimeToSpawn = 3.5f;
    private float cylinderWidth;
    private float cylinderLeftEdge;
    private float cylinderRightEdge;
    private bool spawning = false;   // True if allowed to be spawning. False if currently not spawning
    private bool spawn = true;
    private bool toggle = true;
    private Spawner crateSpawner = new ObstacleSpawner();
    private Spawner spoolSpawner = new SpoolSpawner();
    private Spawner barrelSpawner = new BarrelSpawner();
    private Spawner roadClosedSpawner = new ObstacleSpawner();
    private Spawner bombSpawner = new BombSpawner();
    

    // Start is called before the first frame update
    void Start()
    {
        bombSpawner.Initialize();
        var renderer = cylinder.GetComponentInChildren<MeshRenderer>();
        cylinderWidth = renderer.bounds.size.x;
        cylinderLeftEdge = cylinder.transform.position.x - (cylinderWidth / 2);
        cylinderRightEdge = cylinder.transform.position.x + (cylinderWidth / 2);

        
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
            if (randomValue < 2)
            {
                if (cratePrefab != null)
                {
                    var crate = Instantiate(cratePrefab);
                    crateSpawner.SpawnObject(cylinder, transform.position, hit, crate, true, true);
                }

            }
            else if (randomValue < 4)
            {
                if (spoolPrefab != null)
                {
                    var spool = Instantiate(spoolPrefab);
                    spoolSpawner.SpawnObject(cylinder, transform.position, hit, spool);
                }
            }
            else if (randomValue < 6)
            {
                if (barrelPrefab != null)
                {
                    var barrel = Instantiate(barrelPrefab);
                    barrelSpawner.SpawnObject(cylinder, transform.position, hit, barrel);
                }
            }
            else if (randomValue < 8)
            {
                if (roadClosedPrefab != null)
                {
                    var roadClosed = Instantiate(roadClosedPrefab);
                    roadClosedSpawner.SpawnObject(cylinder, transform.position, hit, roadClosed, false, true);
                }
            }
            else
            {
                if (bombPrefab != null)
                {
                    var bomb = Instantiate(bombPrefab);
                    bombSpawner.SpawnObject(cylinder, transform.position, hit, bomb, false, true);
                }
            }
        }
        spawn = true;
    }
}
