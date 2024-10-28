using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{   
    public SimulationUI SelectedSimulationUI { get; private set; }
    [SerializeField] private TMP_Dropdown _SimulationVariantsDropdown;
    [SerializeField] private Button _SetUpSimulationButton;
    [SerializeField] private SimulationConfiguration _Configuration;
    [SerializeField] private BodiesGravityUI _BodiesGravityUI;

    private void Start()
    {
        _SetUpSimulationButton.onClick.AddListener(() =>
        {
            switch (_SimulationVariantsDropdown.options[_SimulationVariantsDropdown.value].text)
            {
                case "Gravity":
                    SelectedSimulationUI = Instantiate(_BodiesGravityUI);
                    break;
            }

            _Configuration.gameObject.SetActive(true);
            gameObject.SetActive(false);
        });
    }
}
