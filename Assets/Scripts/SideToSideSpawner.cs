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
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            if(spawnObstacle)
            {
                SpawnObstacle(hit);
            }
            else
            {
                SpawnCoin(hit);
            }
        }
        spawn = true;
    }

    private void SpawnObstacle(RaycastHit hit)
    {
        var gameObject = Instantiate(obstaclePrefab);

        // Give it some randomized scale
        gameObject.transform.localScale = new Vector3(Random.Range(3f, 6f), Random.Range(3f, 6f), Random.Range(3f, 6f));
        gameObject.transform.position = transform.position;
        gameObject.transform.up = hit.normal;
        gameObject.transform.parent = cylinder.transform;

        // Give the obstacle a bit of a spin
        var randomValue = Random.Range(1, 4);
        Vector3 spinDirection;
        switch (randomValue)
        {
            case 1:
                spinDirection = Vector3.left;
                break;
            case 2:
                spinDirection = Vector3.right;
                break;
            case 3:
                spinDirection = Vector3.forward;
                break;
            default:
                spinDirection = Vector3.back;
                break;
        }
        gameObject.GetComponent<Rigidbody>().AddTorque(spinDirection * 5f, ForceMode.Impulse);
    }

    private void SpawnCoin(RaycastHit hit)
    {
        var gameObject = Instantiate(coinPrefab);

        gameObject.transform.position = transform.position;
        gameObject.transform.up = hit.normal;
        gameObject.transform.parent = cylinder.transform;

        // Give the coin a slight nudge
        gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 5f, ForceMode.Impulse);
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
