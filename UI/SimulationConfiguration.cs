using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SimulationConfiguration : MonoBehaviour
{
    public static bool IsGameStarted => _IsGameStarted;
    public static event Action GameStarted;
    private static bool _IsGameStarted = false;
    [SerializeField] private MainMenu _Menu;
    [SerializeField] private GameObject _MainMenu;
    [SerializeField] private GameObject _SimulationUI;
    [SerializeField] private Button _StartButton;

    private void Start()
    {
        _StartButton.onClick.AddListener(() =>
        {
            SimulationUpdater Updater = Camera.main.AddComponent<SimulationUpdater>();
            Updater.Initialize(_Menu.SelectedSimulationUI.SelectedSimulation);
            _MainMenu.SetActive(false);
            _SimulationUI.SetActive(true);
            GameStarted?.Invoke();
            _IsGameStarted = true;
        });
    }
}
