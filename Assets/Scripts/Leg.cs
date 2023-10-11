using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg : MonoBehaviour
{
    private bool charging;
    Quaternion startingRot;
    Quaternion maxRot;
    Quaternion rotationTarget;
    private float maxRotX = 60;
    private float minRotX = 22;
    private float rotationSpeed;
    private float rotationSpeedCharge = 20f;
    private float rotationSpeedUncharge = 80f;
    
    void Start()
    {
        startingRot = Quaternion.Euler(minRotX, transform.rotation.y, transform.rotation.y);
        maxRot = Quaternion.Euler(maxRotX, transform.rotation.y, transform.rotation.y);
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

    void Update()
    {
        //rotate based on if its charging or not
        if (charging)
        {
            if (transform.localEulerAngles.x < maxRotX)
            {
                transform.Rotate(Time.deltaTime * rotationSpeed, 0, 0);
            }
        }
        else
        {
            if (transform.localEulerAngles.x > minRotX)
            {
                transform.Rotate(Time.deltaTime * -rotationSpeed, 0, 0);
            }
        }
    }
}
