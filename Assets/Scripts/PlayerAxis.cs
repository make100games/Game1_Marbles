using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAxis : MonoBehaviour
{
    private float rotation = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        var RotationSpeed = 40f;
        rotation += RotationSpeed * Time.deltaTime;
        //Debug.Log("Player Axis rotation: " + rotation);
        //transform.Rotate(RotationSpeed * Time.deltaTime, 0, 0);
        transform.rotation = Quaternion.Euler(rotation, 0, 0);
    }
}
