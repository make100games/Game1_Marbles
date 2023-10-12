using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class TileSpawner : MonoBehaviour
{
    public GameObject cylinder;

    // Start is called before the first frame update
    void Start()
    {

        InvokeRepeating("SpawnObject", 2f, 2f);

        /*Vector3 position = hit.transform.position + hit.normal;

        // calculate the rotation to create the object aligned with the face normal:
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        // create the object at the face center, and perpendicular to it:
        GameObject Placement = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Placement.transform.position = position;
        Placement.transform.rotation = rotation;*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnObject()
    {
        // Spawn tiles by moving left or right a random distance (but not so far
        // as to go beyond the left or right edge of the Cylinder), looking at the
        // cylinder, firing a ray at it and spawning an object where the ray hit
        var cylinderX = cylinder.transform.position.x;
        var cylinderWidth = cylinder.GetComponent<MeshRenderer>().bounds.size.x;
        var amountToMove = Random.Range(0f, (cylinderWidth / 2));
        var signOfMove = Random.Range(0f, 1f) >= 0.5 ? -1 : 1;
        var randomPositionOfSpawner = cylinderX + (amountToMove * signOfMove);
        transform.position = new Vector3(randomPositionOfSpawner, transform.position.y, transform.position.y);
        transform.LookAt(cylinder.transform);

        // Fire a ray into the cylinder and spawn an object there with its normal aligned
        // with that of the face it is sitting on
    }
}
