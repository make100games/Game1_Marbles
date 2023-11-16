using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Spawner : MonoBehaviour
{
    public GameObject cylinder;
    public GameObject prefab;
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

        // Fire a ray into the cylinder and spawn an object there with its normal aligned
        // with that of the face it is sitting on
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            // Spawn object at hit location and align its Up vector with the surface normal of the collision
            //var obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var obstacle = Instantiate(prefab);
            
            // Give it some randomized scale
            obstacle.transform.localScale = new Vector3(Random.Range(1f, 2f), Random.Range(1f, 5f), Random.Range(1f, 2f));
            //obstacle.tag = Tags.Obstacle;
            obstacle.transform.position = hit.point;
            obstacle.transform.up = hit.normal;
            obstacle.transform.Translate(Vector3.up * (obstacle.GetComponent<MeshRenderer>().bounds.size.y / 2), Space.World);
            obstacle.transform.parent = cylinder.transform;
        }
        spawn = true;
    }
}
