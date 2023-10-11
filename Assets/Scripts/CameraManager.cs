using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    private Vector3 cameraOffset = new Vector3(18, 1, -30);
    private Vector3 cameraChargeOffset = new Vector3(0, 0, 2);
    private float cameraSpeed = 0.08f;
    private float cameraMaxYPos = 36;
    public bool active = false;

    Vector3 inactivePos = new Vector3(0, 1, -10);

    public static CameraManager Instance;

    void Start()
    {
    }

    public void Initialize()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        Destroy(gameObject);
    }

    public void SetInactive()
    {
        active = false;
        transform.position = inactivePos;
    }

    public void SetActive()
    {
        active = true;
        targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void SnapToTarget()
    {
        transform.position = targetTransform.position + cameraOffset;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (active)
        {
            Vector3 targetPosition;
            if (GameManager.Instance.spaceship.IsCharging)
            {
                targetPosition = targetTransform.position + cameraOffset + cameraChargeOffset;
            }
            else
            {
                targetPosition = targetTransform.position + cameraOffset;
            }
            targetPosition = new Vector3(targetPosition.x, System.Math.Min(targetPosition.y, cameraMaxYPos), targetPosition.z);
            //smooth transition to target position
            Vector3 lerpPosition = Vector3.Lerp(transform.position, targetPosition, cameraSpeed);
            transform.position = lerpPosition;
        }
    }
}
