using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializationManager : MonoBehaviour
{
    [SerializeField] private MapManager mapManagerPrefab;
    [SerializeField] private CameraManager cameraManagerPrefab;
    [SerializeField] private GameManager gameManagerPrefab;
    [SerializeField] private UIManager uiManagerPrefab;
    void Awake()
    {
        LoadMapManager();
        LoadCameraManager();
        LoadGameManager();
        LoadUIManager();
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
