using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    private Vector3 cameraOffset = new Vector3(15, 1, -30);
    private float cameraSpeed = 0.08f;
    private bool active = false;

    public static CameraManager instance;

    void Start()
    {
 
    }

    public void Initialize()
    {
        if (instance == null)
        {
            instance = this;
            active = true;
            targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
            return;
        }
        Destroy(gameObject);
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
