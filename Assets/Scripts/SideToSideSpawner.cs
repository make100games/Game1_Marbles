using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Moves from side to side spawning either coins or obstacles
public class SideToSideSpawner : MonoBehaviour
{
    public GameObject cylinder;
    public GameObject obstaclePrefab;
    public GameObject coinPrefab;
    public bool spawnObstacle;
    public float intervalBetweenObjects = 0.25f;   // Amount of seconds between each object spawn
    public float minTimeToSpawn = 1f;
    public float maxTimeToSpawn = 3.5f;
    private float cylinderWidth;
    private float amountByWhichToMovePerFrame = 0.05f;
    private float cylinderLeftEdge;
    private float cylinderRightEdge;
    private bool spawning = false;   // True if allowed to be spawning. False if currently not spawning
    private bool spawn = true;
    private bool toggle = true;
    private Spawner coinSpawner = new CoinSpawner();
    private Spawner obstacleSpawner = new ObstacleSpawner();

    // Start is called before the first frame update
    void Start()
    {
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
                obstacleSpawner.SpawnObject(cylinder, transform.position, hit, obstacle);
            }
            else if(coinPrefab != null)
            {
                var coin = Instantiate(coinPrefab);
                coinSpawner.SpawnObject(cylinder, transform.position, hit, coin);
            }
        }
        spawn = true;
    }

    IEnumerator MoveFromSideToSide()
    {
        while(true)
        {
            // Move left
            while (transform.position.x > cylinderLeftEdge)
            {
                transform.position = new Vector3(transform.position.x - amountByWhichToMovePerFrame, transform.position.y, transform.position.z);

                // Make sure spawner does not go beyond bounds of cylinder
                if (transform.position.x < cylinderLeftEdge)
                {
                    transform.position = new Vector3(cylinderLeftEdge, transform.position.y, transform.position.z);
                }
                yield return null;
            }

            // Move right
            while (transform.position.x < cylinderRightEdge)
            {
                transform.position = new Vector3(transform.position.x + amountByWhichToMovePerFrame, transform.position.y, transform.position.z);

                // Make sure spawner does not go beyond bounds of cylinder
                if (transform.position.x > cylinderRightEdge)
                {
                    transform.position = new Vector3(cylinderRightEdge, transform.position.y, transform.position.z);
                }
                yield return null;
            }
        }
    }
}
