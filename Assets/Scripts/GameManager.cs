using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public Spaceship spaceship;

    public float GameTimer { get; private set; } = 0;

    public static GameManager Instance;

    public int Level { get; set; } = 1;
    public readonly int Levels = 3;

    private Controls controls;
    private InputAction pauseInputAction;
    public bool Paused { get; private set; }

    public enum GameState
    {
        Menu,
        StartOfLevel,
        Ingame,
        GameOver,
        Win
    }

    public GameState state;

    private int levelStartMaxTimer = 3;
    public float LevelStartTimer { get; private set; }
    public float BestTime { get; private set; }

    void Update()
    {
        if (state == GameState.Ingame)
        {
            GameTimer += Time.deltaTime;
        } else if (state == GameState.StartOfLevel)
        {
            LevelStartTimer -= Time.deltaTime;
            if (LevelStartTimer <= 0)
            {
                state = GameState.Ingame;
                UIManager.Instance.RemoveLevelStartText();
            }
        }
    }

    public void GameOver()
    {
        state = GameState.GameOver;
        UIManager.Instance.GameOver();
        Time.timeScale = 0;
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
        } else if (state == GameState.Menu)
        {
            Application.Quit();
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
        Level += 1;
        if (Level > Levels)
        {
            WinGame();
            return;
        }
        LevelStartTimer = levelStartMaxTimer;
        state = GameState.StartOfLevel;
        UIManager.Instance.UpdateLevelText(Level);
        MapManager.Instance.Reset();
        CameraManager.Instance.SnapToTarget();
    }

    public void WinGame()
    {
        state = GameState.Win;
        Time.timeScale = 0;

        //save best time
        if (GameTimer < BestTime)
        {
            BestTime = GameTimer;
            PlayerPrefs.SetFloat("BestTime", BestTime);
        }

        UIManager.Instance.WinGame();
    }

    public void StartGame()
    {
        state = GameState.StartOfLevel;
        GameTimer = 0;
        Time.timeScale = 1;
        Level = 1;
        LevelStartTimer = levelStartMaxTimer;
    }

    public void RestartGame()
    {
        StartGame();
        MapManager.Instance.Reset();
        CameraManager.Instance.SnapToTarget();
        spaceship.ResetConditions();
    }

    //this is after scene loads
    public void LoadGameStart()
    {
        MapManager.Instance.StartGame();
        CameraManager.Instance.SetActive();
    }

    public void LoadMenu()
    {
        state = GameState.Menu;
        CameraManager.Instance.SetInactive();
        Time.timeScale = 1;
        Paused = false;
    }

    public void Initialize()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            controls = new Controls();
            controls.Menu.Enable();
            pauseInputAction = controls.Menu.Pause;
            pauseInputAction.performed += ctx => Pause();
            BestTime = PlayerPrefs.GetFloat("BestTime", 599);
            return;
        }
        Destroy(gameObject);
    }
}
