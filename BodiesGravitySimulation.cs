using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BodiesGravitySimulation : Simulation
{
    public BodiesGravitySimulation(IEnumerable<Body> bodies, BodiesGravityUI UI) : base(UI)
    {
        _Bodies = new BodyCollection(bodies);
        _Rigidbodies = _Bodies.Select(X => X.Rigidbody).ToList();
        _SimulatedObjects = _Bodies.Select(X => X.gameObject).ToList();
    }

    public BodiesGravitySimulation(BodiesGravityUI UI) : base(UI) => _SimulatedObjects = new List<GameObject>();

    public float ScaleFactor
    {
        get => _ScaleFactor;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Scale factor must be bigger than 0");

            _ScaleFactor = value;
        }
    }
    public float G => _G;
    public IReadOnlyBodyCollection Bodies => _Bodies;
    private float _G = 0;
    private float _ScaleFactor = 1000;
    private BodyCollection _Bodies = new BodyCollection();
    private List<Rigidbody2D> _Rigidbodies = new List<Rigidbody2D>();

    public void AddBody(Body body)
    {
        if (_Bodies.Contains(body))
            throw new Exception("This body already in simulation");

        if (!IsInitialized)
        {
            AddBody();
            return;
        }
        
        body.AllowChangePosition = IsPaused;
        body.Rigidbody.simulated = !IsPaused;

        AddBody();

        void AddBody()
        {
            _Bodies.Add(body);
            _Rigidbodies.Add(body.Rigidbody);
            _SimulatedObjects.Add(body.gameObject);
        }
    }

    public void RemoveBody(Body body)
    {
        if (!_Bodies.Contains(body))
            throw new Exception("Can not find body");

        if (!IsInitialized)
        {
            RemoveBody();
            return;
        }

        RemoveBody();

        void RemoveBody()
        {
            _Bodies.Remove(body);
            _Rigidbodies.Remove(body.Rigidbody);
            _SimulatedObjects.Remove(body.gameObject);
        }
    }

    protected override void InitializeSimulation()
    {
        _Bodies.ForEach(X =>
        {
            X.AllowChangePosition = false;
            X.Rigidbody.simulated = true;
            X.Trail.gameObject.SetActive(true);
            X.Rigidbody.simulated = true;
        });
    }

    protected override void Pause()
    {
        _Bodies.ForEach(X =>
        {
            X.Rigidbody.simulated = false;
            X.AllowChangePosition = true;
        });
    }

    protected override void Unpause()
    {
        _Bodies.ForEach(X =>
        {
            X.Rigidbody.simulated = true;
            X.AllowChangePosition = false;
        });
    }

    public override void SetUpdate(float DeltaTime)
    {
        if (IsPaused)
            return;

        if (_Rigidbodies.Count == 0)
            return;

        for (int i = 0; i < _Rigidbodies.Count; i++)
        {
            Rigidbody2D Current = _Rigidbodies[i];
            for (int j = 0; j < _Rigidbodies.Count; j++)
            {
                Rigidbody2D Other = _Rigidbodies[j];
                if (Current == Other)
                    continue;
                
                Vector3 CurrentToOther = Current.position - Other.position;
                float Distance = CurrentToOther.magnitude;
                if (Distance == 0)
                    return;
                _G = _ScaleFactor * DeltaTime * TimeScale;
                float GravityForce = Current.mass * Other.mass / Mathf.Pow(Distance, 2) * DeltaTime * TimeScale * _ScaleFactor;
                Vector2 GravityVector = CurrentToOther.normalized * GravityForce;
                Other.AddForce(GravityVector);
            }
        }
    }
}
