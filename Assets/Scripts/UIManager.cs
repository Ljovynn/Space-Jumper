using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private GameObject pauseMenu;
    private GameObject gameOverMenu;
    private RectTransform healthBar;
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
        GameManager.instance.StartGame();
        SwitchToScene("Ingame");
    }

    public void ResumeButton()
    {
        GameManager.instance.Pause();
    }

    public void Pause(bool shouldPause)
    {
        pauseMenu.SetActive(shouldPause);
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GameManager.instance.state == GameManager.GameState.Menu)
        {

        }
        else
        {
            if (this == Instance)
            {
                Debug.Log("scene loaded");
                GameManager.instance.LoadGameStart();

                pauseMenu = GameObject.Find("PauseMenu");
                pauseMenu.SetActive(false);

                gameOverMenu = GameObject.Find("GameOverMenu");
                gameOverMenu.SetActive(false);

                healthBar = GameObject.Find("Health").GetComponent<RectTransform>();
                healthBardMaxWidth = healthBar.rect.width;
            }
        }
    }

    public void LoadMenu()
    {
        SwitchToScene("Main Menu");
        GameManager.instance.LoadMenu();
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
        
    }
}