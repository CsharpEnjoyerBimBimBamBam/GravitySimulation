using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SimulationUpdater : MonoBehaviour
{
    public Simulation Simulation { get; private set; }
    private bool _IsInitialized = false;

    private void FixedUpdate()
    {
        if (Simulation == null)
            return;

        Simulation.SetUpdate(Time.fixedDeltaTime);
    }

    public void Initialize(Simulation simulation)
    {
        if (_IsInitialized)
            throw new Exception("SimulationUpdater already initialized");
        Simulation = simulation;
        Simulation.Initialize();
        _IsInitialized = true;
    }
}
