using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    private readonly float maxChargeTime = 2f;
    private readonly float minChargeForce = 0.6f;
    private readonly float maxChargeForce = 6f;
    private readonly float forceMultiplier = 4f;

    private readonly float normalDrag = 0.15f;
    private readonly float chargeDrag = 0.93f;

    private readonly float rotationMultiplier = 55f;
    private List<Leg> legs = new List<Leg>();
    public bool IsCharging { get; private set; } = false;

    private float chargeTimer = 0;

    public readonly float maxHealth = 100;
    public float Health { get; private set; }

    private readonly float invulTime = 0.2f;
    private float invulTimer = 0;

    private readonly float poisonTime = 0.5f;
    private float poisonTimer = 0;
    private readonly float poisonDamageTime = 0.3f;
    private float poisonDamageTimer = 0;
    private readonly float poisonLevelTime = 0.6f;
    private float poisonLevelTimer = 0;
    private bool inPoison = false;
    private bool poisoned = false;
    private int poisonLevel = 0;
    private readonly float[] poisonDamage = {0.3f, 0.5f, 0.9f, 1.5f, 3 };

    private Rigidbody rigidbody;
    private float previousMagnitude = 0;
    private SpaceshipEmmissions spaceshipEmmissions;

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
        Health = maxHealth;
        rigidbody = GetComponent<Rigidbody>();
        spaceshipEmmissions = transform.GetChild(4).GetComponent<SpaceshipEmmissions>();
        if (spaceshipEmmissions == null)
        {
            Debug.LogWarning("Could not find SpaceEmmissions gameobject");
        }
        GameObject LegHolder = transform.Find("Legs").gameObject;
        for (int i = 0; i < LegHolder.transform.childCount; i++)
        {
            legs.Add(LegHolder.transform.GetChild(i).gameObject.GetComponent<Leg>());
        }

        rigidbody.drag = normalDrag;

        poisonLevelTimer = poisonLevelTime;
        poisonDamageTimer = poisonDamageTime;
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
        UpdatePoison();
        invulTimer -= Time.deltaTime;
        previousMagnitude = rigidbody.velocity.magnitude;
    }

    private void CheckMovement()
    {
        bool chargeInput = JumpInputAction.IsPressed();

        if (chargeInput)
        {
            if (!IsCharging)
            {
                IsCharging = true;
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
            if (IsCharging)
            {
                Jump();
            }
        }

        //cnange to angular velocity?
        float moveDir = TurnInputAction.ReadValue<float>();
        rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(new Vector3(0, 0, -moveDir * rotationMultiplier) * Time.deltaTime));
    }

    private void ResetConditions()
    {
        inPoison = false;
        poisonLevel = 0;
        poisonLevelTimer = poisonLevelTime;
        poisonDamageTimer = poisonDamageTime;
        poisoned = false;

        Health = maxHealth;
        UIManager.Instance.DisplayHealth(1);
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.identity;
        IsCharging = false;
        chargeTimer = 0;
        foreach (Leg leg in legs)
        {
            leg.StopCharging();
        }
    }

    private void TakeCrashDamage(float multiplier, Vector3 otherPos)
    {
        //apply angle multiplier
        if (invulTimer < 0)
        {
            invulTimer = invulTime;

            float damageTaken = previousMagnitude * multiplier;

            Vector3 forward = transform.TransformDirection(Vector3.up).normalized;
            float dot = Vector3.Dot(forward, otherPos);
            dot = System.Math.Max(dot, -dot);
            //have a minimum
            dot = System.Math.Max(dot, 0.2f);

            damageTaken = damageTaken * dot;

            ApplyDamage(damageTaken);
            Debug.Log("Damage taken from crash: " + damageTaken);
            //Debug.Log("Dot: " + Vector3.Dot(forward, otherPos));
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Poison")
        {
            inPoison = true;
            poisonTimer = poisonTime;
            poisoned = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Poison")
        {
            inPoison = false;
        }
    }

    private void UpdatePoison()
    {
        if (!poisoned)
        {
            return;
        }

        poisonTimer -= Time.deltaTime;

        if (poisonTimer < 0)
        {
            poisonLevel = 0;
            poisonLevelTimer = poisonLevelTime;
            poisonDamageTimer = poisonDamageTime;
            poisoned = false;
        }

        //poison gets stronger if you stay in it
        if (inPoison)
        {
            poisonLevelTimer -= Time.deltaTime;
            if (poisonLevelTimer < 0)
            {
                poisonLevelTimer = poisonLevelTime;
                if (poisonLevel < poisonDamage.Length)
                {
                    poisonLevel++;
                }
            }
        }

        //take poison damage
        if (poisonLevel > 0)
        {
            poisonDamageTimer -= Time.deltaTime;
            if (poisonDamageTimer < 0)
            {
                poisonDamageTimer = poisonDamageTime;
                ApplyDamage(poisonDamage[poisonLevel - 1]);
            }
        }
    }

    private void ApplyDamage(float damage)
    {
        Health -= damage;
        UIManager.Instance.DisplayHealth(Health / maxHealth);
        if (Health < 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.instance.GameOver();
    }

    private void Jump()
    {
        IsCharging = false;
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

        spaceshipEmmissions.Emit(chargePercent);
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Goal":
                Debug.Log("player collides with goal");
                GameManager.instance.SpaceshipReachedGoal();
                ResetConditions();
                break;
            case "Planet":
                TakeCrashDamage(2, collision.contacts[0].normal);
                break;
            case "Cone":
                TakeCrashDamage(4, collision.contacts[0].normal);
                break;
        }
    }
}
