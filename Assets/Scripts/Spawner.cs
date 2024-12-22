using UnityEngine;
using System.Collections;

public interface Spawner
{
    /// <summary>
    /// Spawns an object on the parent object at the given RaycastHit.
    /// </summary>
    /// <param name="parentObject">The object on which to spawn the object</param>
    /// <param name="sourcePosition">The position of the spawner</param>
    /// <param name="hit">The position on the parent object where to spawn the object (only used if object should be spawned on surface)</param>
    /// <param name="gameObject">The object to spawn</param>
    /// <param name="randomizeScale">True if the scale of the object to be spawned should be randomized a bit</param>
	void SpawnObject(GameObject parentObject, Vector3 sourcePosition, RaycastHit hit, GameObject gameObject, bool randomizeScale = false, bool addSpin = true);

    void Initialize();

    void Update();
}

public class CoinSpawner : Spawner
{
    public void SpawnObject(GameObject parentObject, Vector3 sourcePosition, RaycastHit hit, GameObject gameObject, bool randomizeScale = false, bool addSpin = true)
    {
        gameObject.transform.position = hit.point;
        gameObject.transform.up = hit.normal;

        // Get the coin to face the right direction. Pretty hacky, I know.
        gameObject.transform.Rotate(new Vector3(0, 0, 270));
        gameObject.transform.Translate(Vector3.up * ((gameObject.GetComponent<MeshRenderer>().bounds.size.y / 2) + 1.75f), Space.World);
        gameObject.transform.parent = parentObject.transform;       
    }

    public void Initialize()
    {
        // No op
    }

    public void Update()
    {
        // No op
    }
}

public class ObstacleSpawner : Spawner
{
    public void SpawnObject(GameObject parentObject, Vector3 sourcePosition, RaycastHit hit, GameObject gameObject, bool randomizeScale = false, bool addSpin = true)
    {
        // Give it some randomized scale
        if(randomizeScale)
        {
            var randomScale = Random.Range(2f, 3f);
            gameObject.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        }
        gameObject.transform.position = sourcePosition;
        gameObject.transform.up = hit.normal;
        gameObject.transform.parent = parentObject.transform;
        gameObject.transform.Rotate(Vector3.up, 90, Space.Self);

        // Give the obstacle a bit of a spin
        if (addSpin)
        {
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
    }

    public void Initialize()
    {
        // No op
    }

    public void Update()
    {
        // No op
    }
}

public class RampSpawner : Spawner
{
    public void SpawnObject(GameObject parentObject, Vector3 sourcePosition, RaycastHit hit, GameObject gameObject, bool randomizeScale = false, bool addSpin = true)
    {
        gameObject.transform.position = hit.point;
        gameObject.transform.up = hit.normal;

        // Get the ramp to face the right direction. Pretty hacky, I know.
        gameObject.transform.Rotate(new Vector3(90, 90, 0));
        gameObject.transform.Rotate(new Vector3(0, 180, 0));
        gameObject.transform.Translate(Vector3.up * ((gameObject.GetComponent<MeshRenderer>().bounds.size.y / 2) - 2f), Space.World);   // Nudging it down a smidge so that it does not hover above the ground
        gameObject.transform.parent = parentObject.transform;
    }

    public void Initialize()
    {
        // No op
    }

    public void Update()
    {
        // No op
    }
}

public class SpoolSpawner : Spawner
{
    public void Initialize()
    {
        // No op
    }

    public void SpawnObject(GameObject parentObject, Vector3 sourcePosition, RaycastHit hit, GameObject gameObject, bool randomizeScale = false, bool addSpin = true)
    {
        gameObject.transform.position = sourcePosition;
        gameObject.transform.parent = parentObject.transform;
        gameObject.transform.Rotate(Vector3.forward, 90, Space.Self);

        // Give the obstacle a bit of a spin
        if (addSpin)
        {
            Vector3 spinDirection = Vector3.right;
            gameObject.GetComponent<Rigidbody>().AddTorque(spinDirection * 165f, ForceMode.Impulse);
        }
    }

    public void Update()
    {
        // No op
    }
}

public class BombSpawner : Spawner
{
    private float timeTillDetonationFloor = 1f;
    private float timeTillDetonationCeiling = 3f;
    private float creationTime;

    public void SpawnObject(GameObject parentObject, Vector3 sourcePosition, RaycastHit hit, GameObject gameObject, bool randomizeScale = false, bool addSpin = true)
    {
        gameObject.transform.position = sourcePosition;
        gameObject.transform.up = hit.normal;
        gameObject.transform.parent = parentObject.transform;
        gameObject.transform.Rotate(Vector3.up, 90, Space.Self);
        gameObject.GetComponent<Bomb>().timeTillDetonation = Random.Range(timeTillDetonationFloor, timeTillDetonationCeiling);

        // Give the obstacle a bit of a spin
        if (addSpin)
        {
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
    }

    public void Initialize()
    {
        creationTime = Time.time;
    }

    public void Update()
    {
        // Decrease time till detonation as time goes on to account for ever increasing speed of
        // ship
        float timeAlive = Time.time - creationTime;
        if(timeAlive >= 5f)
        {
            timeTillDetonationFloor = 0.15f;
            timeTillDetonationCeiling = 2.15f;
        }
        if(timeAlive >= 10f)
        {
            timeTillDetonationFloor = 0.1f;
            timeTillDetonationCeiling = 2.00f;
        }
        if(timeAlive >= 20f)
        {
            timeTillDetonationFloor = 0.15f;
            timeTillDetonationCeiling = 1.8f;
        }
        if(timeAlive >= 45f)
        {
            timeTillDetonationFloor = 0.10f;
            timeTillDetonationCeiling = 1.5f;
        }
        if(timeAlive >= 70f)
        {
            timeTillDetonationFloor = 0.05f;
            timeTillDetonationCeiling = 1f;
        }
        // TODO add more time frames at which to decrease bomb creation time
    }
}

