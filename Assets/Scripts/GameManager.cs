using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public Spaceship spaceship;
    private MapManager mapManager;

    public static GameManager instance;

    private Controls controls;
    private InputAction pauseInputAction;
    public bool Paused { get; private set; }

    public enum GameState
    {
        Menu,
        Ingame
    }

    public GameState state;

    void Start()
    {
    }

    public void GameOver()
    {

    }

    public void Pause()
    {
        if (state == GameState.Ingame)
        {
            if (Paused)
            {
                Paused = false;
                Time.timeScale = 1;
            }
            else
            {
                Paused = true;
                Time.timeScale = 0;
            }
            UIManager.Instance.Pause(Paused);
        }
    }
    private void OnDisable()
    {
        if (controls != null)
        {
            controls.Menu.Disable();
        }
    }

    public void SpaceshipReachedGoal()
    {
        MapManager.instance.GoalReached();
    }

    public void StartGame()
    {
        state = GameState.Ingame;
    }

    public void LoadGameStart()
    {
        Debug.Log("Loading game start from gamemanager");
        MapManager.instance.StartGame(1);
        CameraManager.instance.SetActive();
    }

    public void LoadMenu()
    {
        state = GameState.Menu;
        CameraManager.instance.SetInactive();
        Time.timeScale = 1;
        Paused = false;
    }

    public void Initialize()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            controls = new Controls();
            controls.Menu.Enable();
            pauseInputAction = controls.Menu.Pause;
            pauseInputAction.performed += ctx => Pause();
            return;
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
