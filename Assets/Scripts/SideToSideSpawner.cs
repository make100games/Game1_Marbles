using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Moves from side to side spawning either coins or obstacles
public class SideToSideSpawner : MonoBehaviour
{
    public GameObject cylinder;
    public GameObject obstaclePrefab;
    public GameObject coinPrefab;
    public GameObject bombPrefab;
    public GameObject spoolPrefab;
    public float movementSpeed = 30f; // Speed in units per second at which the spawner moves from side to side
    public bool spawnObstacle;
    public float intervalBetweenObjects = 0.25f;   // Amount of seconds between each object spawn
    public float minTimeToSpawn = 1f;
    public float maxTimeToSpawn = 3.5f;
    public bool randomizeScale = true;  // True if the scale of the spawned objects should be randomized. False if the objects should just be spawned at their true scale
    public bool addSpinToObstacle = true;   // If true, spawned obstacle will have a spin applied
    private float cylinderWidth;
    private float cylinderLeftEdge;
    private float cylinderRightEdge;
    private bool spawning = false;   // True if allowed to be spawning. False if currently not spawning
    private bool spawn = true;
    private bool toggle = true;
    private Spawner coinSpawner = new CoinSpawner();
    private Spawner obstacleSpawner = new ObstacleSpawner();
    private Spawner bombSpawner = new BombSpawner();
    private Spawner spoolSpawner = new SpoolSpawner();

    // Start is called before the first frame update
    void Start()
    {
        bombSpawner.Initialize();
        var renderer = cylinder.GetComponentInChildren<MeshRenderer>();
        cylinderWidth = renderer.bounds.size.x;
        cylinderLeftEdge = cylinder.transform.position.x - (cylinderWidth / 2);
        cylinderRightEdge = cylinder.transform.position.x + (cylinderWidth / 2);

        StartCoroutine(MoveFromSideToSide());
    }

    // Update is called once per frame
    void Update()
    {
        // This is pretty gross but can't think of a better way right now. It's late
        if(toggle)
        {
            toggle = false;
            Invoke("ToggleSpawning", Random.Range(minTimeToSpawn, maxTimeToSpawn));
        }
        
        if(spawning && spawn)
        {
            spawn = false;
            Invoke("SpawnObject", intervalBetweenObjects);
        }
        bombSpawner.Update();
    }

    void ToggleSpawning()
    {
        spawning = !spawning;
        toggle = true;
    }

    void SpawnObject()
    {
        // Fire a ray into the cylinder and spawn an object that will fall towards the cylinder
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity) && hit.collider.CompareTag(Tags.Ground))
        {
            if(spawnObstacle && obstaclePrefab != null)
            {
                var obstacle = Instantiate(obstaclePrefab);
                obstacleSpawner.SpawnObject(cylinder, transform.position, hit, obstacle, randomizeScale, addSpinToObstacle);
            }
            else if(coinPrefab != null)
            {
                var coin = Instantiate(coinPrefab);
                coinSpawner.SpawnObject(cylinder, transform.position, hit, coin);
            }
            else if(bombPrefab != null)
            {
                var bomb = Instantiate(bombPrefab);
                bombSpawner.SpawnObject(cylinder, transform.position, hit, bomb, false, true);
            }
            else if(spoolPrefab != null)
            {
                var spool = Instantiate(spoolPrefab);
                spoolSpawner.SpawnObject(cylinder, transform.position, hit, spool, false, true);
            }
        }
        spawn = true;
    }

    private IEnumerator MoveFromSideToSide()
    {
        while (true)
        {
            // Move left
            while (transform.position.x > cylinderLeftEdge)
            {
                // Calculate the movement for this frame
                float moveAmount = movementSpeed * Time.deltaTime;

                // Update the position, clamping to the cylinder's left edge
                transform.position = new Vector3(
                    Mathf.Max(transform.position.x - moveAmount, cylinderLeftEdge),
                    transform.position.y,
                    transform.position.z
                );

                yield return null;
            }

            // Move right
            while (transform.position.x < cylinderRightEdge)
            {
                // Calculate the movement for this frame
                float moveAmount = movementSpeed * Time.deltaTime;

                // Update the position, clamping to the cylinder's right edge
                transform.position = new Vector3(
                    Mathf.Min(transform.position.x + moveAmount, cylinderRightEdge),
                    transform.position.y,
                    transform.position.z
                );

                yield return null;
            }
        }
    }
}
