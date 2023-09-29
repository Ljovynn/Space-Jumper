using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingPoison : MonoBehaviour
{
    public Transform spaceShip;
    public float yPos;
    public float playerXOffset = 20;
    private ParticleSystem particleSystem;
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        spaceShip = GameObject.FindGameObjectWithTag("Player").transform;
        //set position
    }

    // Update is called once per frame
    void Update()
    {
        //particleSystem.shape.position = new Vector3(spaceShip.position.x + playerXOffset, yPos, 0);
        transform.position = new Vector3(spaceShip.position.x + playerXOffset, yPos, 0);
    }
}
