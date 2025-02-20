using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Moves from side to side spawning either coins or obstacles
public class SideToSideSpawner : MonoBehaviour
{
    public GameObject cylinder;
    public GameObject cratePrefab;
    public GameObject barrierPrefab;
    public GameObject spoolPrefab;
    public GameObject barrelPrefab;
    public GameObject coinPrefab;
    public GameObject bombPrefab;
    public bool randomizeObstacles = false;
    public float movementSpeed = 30f; // Speed in units per second at which the spawner moves from side to side
    public float intervalBetweenObjects = 0.25f;   // Amount of seconds between each object spawn
    public float minTimeToStartSpawning = 3f;
    public float maxTimeToStartSpawning = 6f;
    public float minTimeToStopSpawning = 0.75f;
    public float maxTimeToStopSpawning = 1.5f;
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
    private Spawner barrelSpawner = new BarrelSpawner();

    // Start is called before the first frame update
    void Start()
    {
        bombSpawner.Initialize();
        var renderer = cylinder.GetComponentInChildren<MeshRenderer>();
        cylinderWidth = renderer.bounds.size.x;
        cylinderLeftEdge = cylinder.transform.position.x - (cylinderWidth / 2);
        cylinderRightEdge = cylinder.transform.position.x + (cylinderWidth / 2);

        StartCoroutine(MoveFromSideToSide());

        Invoke("StartSpawning", Random.Range(minTimeToStartSpawning, maxTimeToStartSpawning));
    }

    // Update is called once per frame
    void Update()
    {
        if (spawning && spawn)
        {
            spawn = false;
            Invoke("SpawnObject", intervalBetweenObjects);
        }

        bombSpawner.Update();
    }

    void StopSpawning()
    {
        spawning = false;
        Invoke("StartSpawning", Random.Range(minTimeToStartSpawning, maxTimeToStartSpawning));
    }

    void StartSpawning()
    {
        spawning = true;
        Invoke("StopSpawning", Random.Range(minTimeToStopSpawning, maxTimeToStopSpawning));
    }

    void SpawnObject()
    {
        // Fire a ray into the cylinder and spawn an object that will fall towards the cylinder
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity) && hit.collider.CompareTag(Tags.Ground))
        {
            if(randomizeObstacles)
            {
                var randomValue = Random.Range(0, 4);
                if(randomValue == 0)
                {
                    if(cratePrefab != null)
                    {
                        var obstacle = Instantiate(cratePrefab);
                        obstacleSpawner.SpawnObject(cylinder, transform.position, hit, obstacle, randomizeScale, addSpinToObstacle);
                    }
                }
                else if(randomValue == 1)
                {
                    if(barrierPrefab != null)
                    {
                        var obstacle = Instantiate(barrierPrefab);
                        obstacleSpawner.SpawnObject(cylinder, transform.position, hit, obstacle, false, addSpinToObstacle);
                    }
                }
                else if(randomValue == 2)
                {
                    if(spoolPrefab != null)
                    {
                        var obstacle = Instantiate(spoolPrefab);
                        spoolSpawner.SpawnObject(cylinder, transform.position, hit, obstacle, randomizeScale, addSpinToObstacle);
                    }
                }
                else if(randomValue == 3)
                {
                    if(barrelPrefab != null)
                    {
                        var obstacle = Instantiate(barrelPrefab);
                        barrelSpawner.SpawnObject(cylinder, transform.position, hit, obstacle, randomizeScale, addSpinToObstacle);
                    }
                }
            }
            else
            {
                if (cratePrefab != null)
                {
                    var obstacle = Instantiate(cratePrefab);
                    obstacleSpawner.SpawnObject(cylinder, transform.position, hit, obstacle, randomizeScale, addSpinToObstacle);
                }
                else if (coinPrefab != null)
                {
                    var coin = Instantiate(coinPrefab);
                    coinSpawner.SpawnObject(cylinder, transform.position, hit, coin);
                }
                else if (bombPrefab != null)
                {
                    var bomb = Instantiate(bombPrefab);
                    bombSpawner.SpawnObject(cylinder, transform.position, hit, bomb, false, true);
                }
                else if (spoolPrefab != null)
                {
                    var spool = Instantiate(spoolPrefab);
                    spoolSpawner.SpawnObject(cylinder, transform.position, hit, spool, false, true);
                }
                else if(barrierPrefab != null)
                {
                    var obstacle = Instantiate(barrierPrefab);
                    obstacleSpawner.SpawnObject(cylinder, transform.position, hit, obstacle, randomizeScale, addSpinToObstacle);
                }
                else if (barrelPrefab != null)
                {
                    var obstacle = Instantiate(barrelPrefab);
                    barrelSpawner.SpawnObject(cylinder, transform.position, hit, obstacle, randomizeScale, addSpinToObstacle);
                }
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
