using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    private float maxChargeTime = 2f;
    private float minChargeForce = 0.6f;
    private float maxChargeForce = 6f;
    private float forceMultiplier = 4f;

    private float normalDrag = 0.15f;
    private float chargeDrag = 0.93f;

    private float rotationMultiplier = 50f;
    private List<Leg> legs = new List<Leg>();
    bool isCharging = false;
    private float chargeTimer = 0;

    private Rigidbody rigidbody;

    private Controls controls;
    private InputAction TurnInputAction;
    private InputAction JumpInputAction;

    // Start is called before the first frame update

    private void Awake()
    {
        controls = new Controls();
        TurnInputAction = controls.Spaceship.Turn;
        JumpInputAction = controls.Spaceship.Jump;
    }
    void Start()
    {
        //gets all the legs of the spaceship
        rigidbody = GetComponent<Rigidbody>();
        GameObject LegHolder = transform.Find("Legs").gameObject;
        for (int i = 0; i < LegHolder.transform.childCount; i++)
        {
            legs.Add(LegHolder.transform.GetChild(i).gameObject.GetComponent<Leg>());
        }

        rigidbody.drag = normalDrag;
    }

    private void OnEnable()
    {
        controls.Spaceship.Enable();
    }

    private void OnDisable()
    {
        controls.Spaceship.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        CheckMovement();
        rigidbody.AddForce(Vector3.down * 0.06f);
    }

    private void CheckMovement()
    {
        bool chargeInput = JumpInputAction.IsPressed();

        if (chargeInput)
        {
            if (!isCharging)
            {
                isCharging = true;
                rigidbody.drag = chargeDrag;
                foreach (Leg leg in legs)
                {
                    leg.StartCharging();
                }
            }
            chargeTimer += Time.deltaTime;
        }
        else
        {
            if (isCharging)
            {
                Jump();
            }
        }

        //cnange to angular velocity?
        float moveDir = TurnInputAction.ReadValue<float>();
        rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(new Vector3(0, 0, -moveDir * rotationMultiplier) * Time.deltaTime));
    }

    private void Jump()
    {
        isCharging = false;
        foreach (Leg leg in legs)
        {
            leg.StopCharging();
        }

        rigidbody.drag = normalDrag;
        float charge = System.Math.Min(chargeTimer, maxChargeTime);

        float chargePercent = charge / maxChargeTime;
        float chargeForceApplier = (maxChargeForce - minChargeForce) * chargePercent;
        float force = (minChargeForce + chargeForceApplier) * forceMultiplier;

        rigidbody.AddRelativeForce(Vector3.up * force, mode:ForceMode.VelocityChange);

        chargeTimer = 0;
    }
}
