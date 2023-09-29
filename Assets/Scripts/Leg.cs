using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg : MonoBehaviour
{
    private bool charging;
    Quaternion startingRot;
    Quaternion maxRot;
    Quaternion rotationTarget;
    private float maxRotX = 40;
    private float minRotX = 22;
    private float rotationSpeed;
    private float rotationSpeedCharge = 10f;
    private float rotationSpeedUncharge = 100f;
    
    void Start()
    {
        startingRot = transform.rotation;
        maxRot = new Quaternion(maxRotX, 180, 0, 0);// transform.rotation.z, transform.rotation.w);
        rotationTarget = startingRot;
    }

    public void StartCharging()
    {
        charging = true;
        rotationTarget = maxRot;
        rotationSpeed = rotationSpeedCharge;
    }

    public void StopCharging() 
    { 
        charging = false;
        rotationTarget = startingRot;
        rotationSpeed = rotationSpeedUncharge;
    }

    // Update is called once per frame
    void Update()
    {
        if (charging)
        {
            if (transform.rotation.x < maxRotX)
            {
                transform.Rotate(Time.deltaTime * rotationSpeed, 0, 0);
            }
        }
        else
        {
            if (transform.rotation.x > minRotX)
            {
                transform.Rotate(Time.deltaTime * -rotationSpeed, 0, 0);
            }
        }
    }
}
