using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine;
using System.Security.Cryptography;

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
    private readonly float[] poisonDamage = {0.3f, 0.5f, 0.9f, 1.8f, 5 };

    private Rigidbody rigidbody;
    private float previousMagnitude = 0;
    private SpaceshipEmmissions spaceshipEmmissions;

    private Controls controls;
    private InputAction TurnInputAction;
    private InputAction JumpInputAction;

    private Color baseColor = Color.white;
    private Color poisonedColor = new Color(0.7f, 0.4f, 0.8f);
    private Color damageColor = new Color(0.8f, 0.3f, 0.35f);
    private Color targetColor;
    private float colorTimeLeft = 0;
    private float colorTransitionTime = 0.3f;

    private Renderer[] renderers;

    // Start is called before the first frame update

    private void Awake()
    {
        controls = new Controls();
        TurnInputAction = controls.Spaceship.Turn;
        JumpInputAction = controls.Spaceship.Jump;
    }
    void Start()
    {
        Health = maxHealth;
        rigidbody = GetComponent<Rigidbody>();
        spaceshipEmmissions = transform.GetChild(4).GetComponent<SpaceshipEmmissions>();
        renderers = GetComponentsInChildren<Renderer>();
        targetColor = baseColor;
        if (spaceshipEmmissions == null)
        {
            Debug.LogWarning("Could not find SpaceEmmissions gameobject");
        }
        GameObject LegHolder = transform.Find("Legs").gameObject;
        //gets all the "legs" of the spaceship
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
        UpdateColor();
        invulTimer -= Time.deltaTime;
        previousMagnitude = rigidbody.velocity.magnitude;
    }

    private void CheckMovement()
    {
        bool chargeInput = JumpInputAction.IsPressed();

        if (GameManager.Instance.state == GameManager.GameState.StartOfLevel)
        {
            chargeInput = false;
        }

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

        //turn spaceship
        float moveDir = TurnInputAction.ReadValue<float>();
        rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(new Vector3(0, 0, -moveDir * rotationMultiplier) * Time.deltaTime));
    }

    private void SetNewTargetColor(Color newColor)
    {
        if (newColor != targetColor)
        {
            colorTimeLeft = colorTransitionTime;
            targetColor = newColor;
        }
    }

    //smooth transition to target color
    private void UpdateColor()
    {
        if (colorTimeLeft <= Time.deltaTime)
        {
            ApplyColor(targetColor);
            if (targetColor != baseColor)
            {
                //switches to base color
                SetNewTargetColor(baseColor);
            }
        }
        else
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = Color.Lerp(renderers[i].material.color, targetColor, Time.deltaTime / colorTimeLeft);
            }

            colorTimeLeft -= Time.deltaTime;
        }
    }

    private void ApplyColor(Color color)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = color;
        }
    }

    public void ResetConditions()
    {
        targetColor = baseColor;
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
        if (invulTimer < 0)
        {
            invulTimer = invulTime;

            //more damage with more magnitude
            float damageTaken = previousMagnitude * multiplier;

            //less damage if the side of the ship is hit
            Vector3 forward = transform.TransformDirection(Vector3.up).normalized;
            float dot = Vector3.Dot(forward, otherPos);
            dot = System.Math.Max(dot, -dot);
            //have a minimum
            dot = System.Math.Max(dot, 0.2f);

            damageTaken = damageTaken * dot;

            ApplyDamage(damageTaken);
            SetNewTargetColor(damageColor);
            Debug.Log("Damage taken from crash: " + damageTaken);
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


    //all poison calculations here
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
                SetNewTargetColor(poisonedColor);
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
        ApplyColor(damageColor);
        GameManager.Instance.GameOver();
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
                GameManager.Instance.SpaceshipReachedGoal();
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
