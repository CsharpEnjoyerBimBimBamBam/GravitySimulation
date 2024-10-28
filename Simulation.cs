using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Simulation
{
    public Simulation(SimulationUI UI) => _UI = UI;

    public float TimeScale
    {
        get => _TimeScale;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Time scale must be bigger than 0");

            _TimeScale = value;
        }
    }
    public bool IsPaused => _IsPaused;
    public bool IsInitialized => _IsInitialized;
    public IReadOnlyList<GameObject> SimulatedObjects => _SimulatedObjects;
    public SimulationUI UI => _UI;
    public event Action Paused;
    public event Action Unpaused;
    public event Action Initialized;
    protected List<GameObject> _SimulatedObjects = new List<GameObject>();
    private float _TimeScale = 1;
    private bool _IsPaused = false;
    private bool _IsInitialized = false;
    private SimulationUI _UI;

    public void Initialize()
    {
        if (_IsInitialized)
            throw new Exception("Simulation already initialized");

        InitializeSimulation();
        _IsInitialized = true;
        Initialized?.Invoke();
    }

    public void SetPause(bool pause)
    {
        if (pause == _IsPaused)
            return;

        if (pause)
        {
            Pause();
            _IsPaused = true;
            Paused?.Invoke();
            return;
        }
        Unpause();
        _IsPaused = false;
        Unpaused?.Invoke();
    }

    public void Destroy()
    {
        foreach (GameObject gameObject in SimulatedObjects)
            MonoBehaviour.Destroy(gameObject);

        MonoBehaviour.Destroy(UI.gameObject);
    }

    public abstract void SetUpdate(float DeltaTime);

    protected abstract void Pause();

    protected abstract void Unpause();

    protected abstract void InitializeSimulation();
}
