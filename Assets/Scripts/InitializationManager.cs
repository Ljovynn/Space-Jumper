using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializationManager : MonoBehaviour
{
    [SerializeField] private MapManager mapManagerPrefab;
    [SerializeField] private CameraManager cameraManagerPrefab;
    [SerializeField] private GameManager gameManagerPrefab;
    [SerializeField] private UIManager uiManagerPrefab;
    [SerializeField] private bool loadMainMenuAtStart = true;
    private static bool initialized = false;
    void Awake()
    {
        if (!initialized)
        {
            LoadMapManager();
            LoadCameraManager();
            LoadGameManager();
            LoadUIManager();
            if (loadMainMenuAtStart)
            {
                UIManager.Instance.LoadMenu();
            }
            else
            {
                UIManager.Instance.StartGameButton();
            }
            initialized = false;
        }
    }

    void LoadMapManager()
    {
        var mapManager = Instantiate(mapManagerPrefab);
        mapManager.Initialize();
    }

    void LoadCameraManager()
    {
        var cameraManager = Instantiate(cameraManagerPrefab);
        cameraManager.Initialize();
    }

    void LoadGameManager()
    {
        var gameManager = Instantiate(gameManagerPrefab);
        gameManager.Initialize();
    }

    void LoadUIManager()
    {
        var uiManager = Instantiate(uiManagerPrefab);
        uiManager.Initialize();
    }
}
