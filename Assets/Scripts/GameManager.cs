using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject spaceShip;
    private MapManager mapManager;

    public static GameManager instance;

    void Start()
    {
    }

    private void OnEnable()
    {
        Goal.goalReached += SpaceshipReachedGoal;
    }

    private void OnDisable()
    {
        Goal.goalReached -= SpaceshipReachedGoal;
    }

    void SpaceshipReachedGoal()
    {

    }

    public void Initialize()
    {
        if (instance == null)
        {
            instance = this;
            return;
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
