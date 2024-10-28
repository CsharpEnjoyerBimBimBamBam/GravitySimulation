using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public partial class BaseSimulationUI : MonoBehaviour
{
    [SerializeField] private Button _PauseButton;
    [SerializeField] private Sprite _PlaySprite;
    [SerializeField] private Sprite _PauseSprite;
    [SerializeField] private ScaleNumeric _TimeScaleNumeric;
    [SerializeField] private Button _FocusButton;
    [SerializeField] private GameObject _PauseMenu;
    private Simulation _Simulation;
    private SimulationUpdater _Updater;

    private void Start()
    {
        _PauseButton.onClick.AddListener(UpdateSimulationPause);

        _TimeScaleNumeric.ValueChanged += (value) => Time.timeScale = value;
        PauseMenu.ExitToMainMenu += () => _PauseButton.image.sprite = _PauseSprite;
        InputEvent.SimulationPauseChanged += UpdateSimulationPause;
        InputEvent.SimulationTimeUpscaled += () => _TimeScaleNumeric.IncreaseValue();
        InputEvent.SimulationTimeDownscaled += () => _TimeScaleNumeric.DecreaseValue();

        _FocusButton.onClick.AddListener(() =>
        {
            Vector2 Center = Vector2.zero;
            foreach (GameObject gameObject in _Simulation.SimulatedObjects)
            {
                Vector3 Position = gameObject.transform.position;
                Center += new Vector2(Position.x, Position.y);
            }
            Camera.main.transform.position = new Vector3(Center.x, Center.y, -10);
        });
    }

    private void OnEnable()
    {
        _Updater = Camera.main.GetComponent<SimulationUpdater>();
        _Simulation = _Updater.Simulation;
        InputEvent.GamePauseChanged += PauseGame;
    }

    private void OnDisable()
    {
        InputEvent.GamePauseChanged -= PauseGame;
    }

    private void PauseGame()
    {
        _PauseMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    private void UpdateSimulationPause()
    {
        bool IsPaused = _Simulation.IsPaused;

        _Simulation.SetPause(!IsPaused);
        if (IsPaused)
        {
            _PauseButton.image.sprite = _PauseSprite;
            return;
        }

        _PauseButton.image.sprite = _PlaySprite;

    }
}
