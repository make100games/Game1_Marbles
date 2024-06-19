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
        transform.rotation = Quaternion.Euler(rotation, 0, 0);
    }
}
