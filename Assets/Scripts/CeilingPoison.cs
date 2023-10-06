using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingPoison : MonoBehaviour
{
    private Transform spaceShip;
    public float yPos { get; set; }
    private float playerXOffset = 50;
    void Start()
    {
        spaceShip = GameObject.FindGameObjectWithTag("Player").transform;
        if (spaceShip == null)
        {
            Debug.LogWarning("Spaceship not found by poison ceiling");
            Destroy(gameObject);
            return;
        }
        transform.position = new Vector3(spaceShip.position.x + playerXOffset, yPos, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(spaceShip.position.x + playerXOffset, yPos, 0);
    }
}
