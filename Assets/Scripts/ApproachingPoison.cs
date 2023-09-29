using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApproachingPoison : MonoBehaviour
{
    private float speed;
    void Start()
    {
        
    }

    public void Reset(Vector3 position, float speed)
    {
        this.speed = speed;
        //set position
    }

    // Update is called once per frame
    void Update()
    {
        float movementX = speed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x + movementX, transform.position.y, transform.position.z);
    }
}