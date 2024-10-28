using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using System;

public class PauseMenu : MonoBehaviour
{
    public static event Action ExitToMainMenu;
    [SerializeField] private Button _MainMenuButton;
    [SerializeField] private GameObject _MainMenu;
    private SimulationUpdater _Updater;
    private bool _IsSimulationPaused;
    private const float _DefaultCameraSize = 5;

    private void Start()
    {
        _MainMenuButton.onClick.AddListener(() =>
        {
            ExitToMainMenu?.Invoke();
            _Updater.Simulation.Destroy();
            _MainMenu.SetActive(true);
            gameObject.SetActive(false);
            Camera.main.transform.position = Vector3.zero;
            Camera.main.orthographicSize = _DefaultCameraSize;
        });
    }

    private void OnEnable()
    {
        _Updater = Camera.main.GetComponent<SimulationUpdater>();
        _IsSimulationPaused = _Updater.Simulation.IsPaused;
        _Updater.Simulation.SetPause(true);
        foreach (GameObject gameObject in _Updater.Simulation.SimulatedObjects)
            gameObject.SetActive(false);
        InputEvent.GamePauseChanged += UnpauseGame;
    }

    private void OnDisable()
    {
        InputEvent.GamePauseChanged -= UnpauseGame;
    }

    private void UnpauseGame()
    {
        _Updater.Simulation.SetPause(_IsSimulationPaused);
        foreach (GameObject gameObject in _Updater.Simulation.SimulatedObjects)
            gameObject.SetActive(true);
        _Updater.Simulation.UI.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
