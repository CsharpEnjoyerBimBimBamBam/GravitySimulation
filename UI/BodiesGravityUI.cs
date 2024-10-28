using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BodiesGravityUI : SimulationUI
{
    public BodiesGravityUI() => _SelectedSimulation = new BodiesGravitySimulation(this);

    public override Simulation SelectedSimulation => _SelectedSimulation;
    [SerializeField] private Button _AddBodyButton;
    [SerializeField] private Body _BodyPrefab;
    [SerializeField] private DistanceLine _DistanceLinePrefab;
    [SerializeField] private GameObject _CenterOfMassPrefab;
    [SerializeField] private GameObject _BodyParameters;
    [SerializeField] private UnitsNumeric _SpeedNumeric;
    [SerializeField] private Button _SetToOrbitSpeedButton;
    [SerializeField] private UnitsNumeric _MassNumeric;
    [SerializeField] private UnitsNumeric _RadiusNumeric;
    [SerializeField] private TextNumeric _OrbitSpeedNumeric;
    [SerializeField] private Toggle _ShowTrailToggle;
    [SerializeField] private UnitsNumeric _TrailLifeTime;
    [SerializeField] private Button _RemoveBodyButton;
    private BodiesGravitySimulation _SelectedSimulation;
    private GameObject _CenterOfMass;
    private DistanceLine _DistanceLine;
    private Body _SelectedBody;
    private bool _IsBodyParametersChange;
    private List<UnitsNumeric> _Numerics = new List<UnitsNumeric>();
    private event Action<Body> SelectedBodyChanged;

    private void Start()
    {
        _Numerics = new List<UnitsNumeric> { _SpeedNumeric, _MassNumeric };
        _BodyParameters.SetActive(false);
        CameraMovment Movement = Camera.main.GetComponent<CameraMovment>();
        _AddBodyButton.onClick.AddListener(() =>
        {
            if (SimulationConfiguration.IsGameStarted && !_SelectedSimulation.IsPaused)
                return;

            Body _Body = Instantiate(_BodyPrefab, transform);
            Vector3 CameraPosition = Camera.main.transform.position;
            _Body.transform.position = new Vector3(CameraPosition.x, CameraPosition.y, 0);
            _Body.ItemMovementStart += () => Movement.IsMovable = false;
            _Body.ItemMovementEnd += () => Movement.IsMovable = true;
            _Body.VelocityMovementStart += () => Movement.IsMovable = false;
            _Body.VelocityMovementEnd += () => Movement.IsMovable = true;
            _Body.Trail.gameObject.SetActive(false);

            _SelectedSimulation.AddBody(_Body);
        });

        _SetToOrbitSpeedButton.onClick.AddListener(() =>
        {
            float OrbitSpeed = CalculateOrbitSpeed();
            Vector2 CenterOfMassLocal = _SelectedSimulation.Bodies.CenterOfMass - _SelectedBody.Rigidbody.position;
            Vector2 ReferenceVelocity = _SelectedBody.Rigidbody.velocity;
            Vector2 TargetVelocity = Vector2.Perpendicular(CenterOfMassLocal).normalized * OrbitSpeed;
            if (Vector2.Angle(ReferenceVelocity, TargetVelocity) > 90)
                TargetVelocity = -TargetVelocity;
            _SelectedBody.Rigidbody.velocity = TargetVelocity;
        });

        _SelectedSimulation.Paused += OnPause;
        _SelectedSimulation.Unpaused += OnUnpause;
        _SelectedSimulation.Initialized += OnInitialized;

        _CenterOfMass = Instantiate(_CenterOfMassPrefab);
        _CenterOfMass.SetActive(false);
        _DistanceLine = Instantiate(_DistanceLinePrefab);
        _DistanceLine.gameObject.SetActive(false);

        Body.SelectedBodies.CountChanged += (count) =>
        {
            if (count <= 1)
            {
                _CenterOfMass.SetActive(false);
                return;
            }

            _CenterOfMass.SetActive(true);
        };

        Body.SelectedBodies.CountChanged += (count) =>
        {
            if (count != 1)
            {
                _BodyParameters.SetActive(false);
                _SelectedBody = null;
                SelectedBodyChanged?.Invoke(null);
                return;
            }

            _BodyParameters.SetActive(true);
            _SelectedBody = Body.SelectedBodies[0];
            SelectedBodyChanged?.Invoke(_SelectedBody);
            _ShowTrailToggle.isOn = _SelectedBody.Trail.gameObject.activeInHierarchy;
        };

        Body.SelectedBodies.CountChanged += (count) =>
        {
            if (count != 2)
            {
                _DistanceLine.gameObject.SetActive(false);
                return;
            }

            _DistanceLine.gameObject.SetActive(true);
            _DistanceLine.LockToTransforms(Body.SelectedBodies[0].transform, Body.SelectedBodies[1].transform);
        };

        SelectedBodyChanged += (Body) =>
        {
            if (Body == null)
                return;

            _ShowTrailToggle.isOn = Body.Trail.gameObject.activeInHierarchy;
            _MassNumeric.Value = Body.Rigidbody.mass;
            _TrailLifeTime.Value = Body.Trail.time;
            _RadiusNumeric.Value = Body.Radius;
        };

        _Numerics.ForEach(X =>
        {
            X.InputFieldSelect += () => _IsBodyParametersChange = true;
            X.InputFieldDeselect += () => _IsBodyParametersChange = false;
        });

        _SpeedNumeric.ValueChanged += (e) => _SelectedBody.Speed = _SpeedNumeric.UnitsValue;
        _MassNumeric.ValueChanged += (e) => _SelectedBody.Rigidbody.mass = _MassNumeric.UnitsValue;
        _TrailLifeTime.ValueChanged += (e) => _SelectedBody.Trail.time = e;
        _RadiusNumeric.ValueChanged += (e) => _SelectedBody.Radius = e;

        _ShowTrailToggle.onValueChanged.AddListener((e) =>
        {
            _TrailLifeTime.gameObject.SetActive(e);
            _SelectedBody.Trail.gameObject.SetActive(e);
        });

        _RemoveBodyButton.onClick.AddListener(() =>
        {
            _SelectedSimulation.RemoveBody(_SelectedBody);
            Destroy(_SelectedBody.gameObject);
        });
    }

    private void Update()
    {
        if (_SelectedBody == null || _IsBodyParametersChange)
            return;

        _SpeedNumeric.Value = _SelectedBody.Rigidbody.velocity.magnitude;
        _OrbitSpeedNumeric.Value = CalculateOrbitSpeed();
    }

    private void OnDisable()
    {
        _SelectedSimulation.Paused -= OnPause;
        _SelectedSimulation.Unpaused -= OnUnpause;
        _SelectedSimulation.Initialized -= OnInitialized;
    }

    private void OnDestroy()
    {
        if (_CenterOfMass != null)
            Destroy(_CenterOfMass);
    }

    private float CalculateOrbitSpeed()
    {
        float MassSum = _SelectedSimulation.Bodies.Sum(X => X != _SelectedBody ? X.Rigidbody.mass : 0);
        float Distance = Vector2.Distance(_SelectedBody.Rigidbody.position, _SelectedSimulation.Bodies.CenterOfMass);
        float OrbitSpeed = Mathf.Sqrt(_SelectedSimulation.G * MassSum / Distance);
        return !float.IsNaN(OrbitSpeed) ? OrbitSpeed : 0;
    }

    private Vector2 RotateTowards(Vector2 From, Vector2 To, float Angle)
    {
        bool IsClockWise = Vector2.SignedAngle(From, To) < 0;
        return Rotate(From, Angle, IsClockWise);
    }

    private Vector2 Rotate(Vector2 Vector, float Angle, bool ClockWise)
    {
        float SinAngle = Mathf.Sin(Angle);
        float CosAngle = Mathf.Cos(Angle);

        Vector2 RotatedVector = new Vector2(
            Vector.x * CosAngle - Vector.y * SinAngle, 
            Vector.x * SinAngle + Vector.y * CosAngle);

       if (ClockWise)
            RotatedVector = new Vector2(
            Vector.x * CosAngle + Vector.y * SinAngle,
            Vector.x * SinAngle - Vector.y * CosAngle);

       return RotatedVector;
    }

    private void OnPause() => _AddBodyButton.interactable = true;

    private void OnUnpause() => _AddBodyButton.interactable = false;

    private void OnInitialized() => _AddBodyButton.interactable = false;
}
