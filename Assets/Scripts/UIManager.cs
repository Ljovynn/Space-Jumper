using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private GameObject pauseMenu;
    private GameObject gameOverMenu;
    private GameObject winMenu;

    private RectTransform healthBar;
    private TextMeshProUGUI gameTimerText;
    private TextMeshProUGUI levelText;
    private TextMeshProUGUI levelStartText;
    private TextMeshProUGUI bestTimeText;

    private float healthBardMaxWidth;

    public void Initialize()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        Destroy(gameObject);
    }

    public void StartGameButton()
    {
        GameManager.Instance.StartGame();
        SwitchToScene("Ingame");
    }

    public void ResumeButton()
    {
        GameManager.Instance.Pause();
    }

    public void RestartButton()
    {
        Instance.Restart();
    }

    public void Restart()
    {
        GameManager.Instance.RestartGame();
        gameTimerText.text = "0:00:00";
        UpdateLevelText(1);
        gameOverMenu.SetActive(false);
    }

    public void Pause(bool shouldPause)
    {
        pauseMenu.SetActive(shouldPause);
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GameManager.Instance.state == GameManager.GameState.Menu)
        {

        }
        else
        {
            if (this == Instance)
            {
                Debug.Log("scene loaded");
                GameManager.Instance.LoadGameStart();

                pauseMenu = GameObject.Find("PauseMenu");
                pauseMenu.SetActive(false);

                gameOverMenu = GameObject.Find("GameOverMenu");
                gameOverMenu.SetActive(false);

                bestTimeText = GameObject.Find("BestTimeText").GetComponent<TextMeshProUGUI>();

                winMenu = GameObject.Find("WinMenu");
                winMenu.SetActive(false);

                healthBar = GameObject.Find("Health").GetComponent<RectTransform>();
                healthBardMaxWidth = healthBar.rect.width;

                gameTimerText = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
                gameTimerText.text = "0:00:00";

                levelText = GameObject.Find("LevelIndicator").GetComponent<TextMeshProUGUI>();
                UpdateLevelText(1);

                levelStartText = GameObject.Find("LevelStartText").GetComponent<TextMeshProUGUI>();
                levelStartText.text = "3";
            }
        }
    }

    public void WinGame()
    {
        winMenu.SetActive(true);

        TimeSpan time = TimeSpan.FromSeconds(GameManager.Instance.BestTime);
        bestTimeText.text = "Best time: " + time.ToString("m':'ss':'ff");
    }

    public void UpdateLevelText(int level)
    {
        levelText.text = "Level " + level;
    }

    public void RemoveLevelStartText()
    {
        levelStartText.text = "";
    }

    public void LoadMenu()
    {
        SwitchToScene("Main Menu");
        GameManager.Instance.LoadMenu();
    }

    public void DisplayHealth(float healthPercent)
    {
        healthBar.sizeDelta = new Vector2(healthBardMaxWidth * healthPercent, healthBar.rect.height);
    }

    public void GameOver()
    {
        gameOverMenu.SetActive(true);
    }

    private bool SwitchToScene(string name)
    {
        if (SceneManager.GetActiveScene().name != name)
        {
            SceneManager.LoadScene(name);
            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state == GameManager.GameState.Ingame)
        {
            TimeSpan time = TimeSpan.FromSeconds(GameManager.Instance.GameTimer);
            gameTimerText.text = time.ToString("m':'ss':'ff");
        } else if (GameManager.Instance.state == GameManager.GameState.StartOfLevel)
        {
            levelStartText.text = GameManager.Instance.LevelStartTimer.ToString("0");
        }
    }
}