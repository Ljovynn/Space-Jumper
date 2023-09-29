using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    private Vector3 cameraOffset;
    private float cameraSpeed = 0.08f;
    private bool active = false;

    void Start()
    {
        cameraOffset = new Vector3(15, 1, -30);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (active)
        {
            Vector3 targetPosition = targetTransform.position + cameraOffset;
            Vector3 lerpPosition = Vector3.Lerp(transform.position, targetPosition, cameraSpeed);
            transform.position = lerpPosition;
        }
    }
}
