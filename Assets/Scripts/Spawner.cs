using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Spawner : MonoBehaviour
{
    public GameObject cylinder;
    public GameObject prefab;
    public GameObject obstaclePrefab;
    public GameObject coinPrefab;
    private float cylinderWidth;

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
        if(spawn)
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

        // Fire a ray into the cylinder and spawn an object that will fall towards the cylinder
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            if(Random.Range(0, 2) == 0)
            {
                StartCoroutine(SpawnCoin(hit));
            }
            else
            {
                StartCoroutine(SpawnObstacle(hit));
            }
            
        }
        spawn = true;
    }

    
    private IEnumerator SpawnObstacle(RaycastHit hit)
    {
        var numberOfObstacles = Random.Range(2, 4);
        for(int i = 0; i < numberOfObstacles; i++)
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
            yield return null;
        }
    }

    private IEnumerator SpawnCoin(RaycastHit hit)
    {
        // Spawn a handful of coins
        var numberOfCoins = Random.Range(2, 5);
        for(int i = 0; i < numberOfCoins; i++)
        {
            var gameObject = Instantiate(coinPrefab);

            gameObject.transform.position = transform.position;
            gameObject.transform.up = hit.normal;
            gameObject.transform.parent = cylinder.transform;

            // Give the coin a slight nudge
            gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 5f, ForceMode.Impulse);
            yield return null;
        }
    }
}
