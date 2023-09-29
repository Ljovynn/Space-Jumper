using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private Transform spaceShip;
    void Start()
    {
    }

    public delegate void GoalDelegate();
    public static GoalDelegate goalReached;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("goal reached");
            goalReached?.Invoke();
        }
    }

    void Update()
    {
        
    }
}
