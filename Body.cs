using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Body : MonoBehaviour
{
    public Body() => _Radius = _StartRadius;

    public float Radius 
    {
        get => _Radius; 
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Radius must be bigger than 0");

            _Radius = value;
            float Scale = _StartScale * (_Radius / _StartRadius);
            transform.localScale = new Vector3(Scale, Scale, Scale);

            float TrailSize = _Radius * _TrailRadiusMultiplier;
            float VelocitySize = _Radius * _VelocityRadiusMultiplier;

            _Trail.startWidth = TrailSize;
            _Trail.endWidth = TrailSize * _TrailStartToEndMultiplier;

            _VelocityLineRenderer.startWidth = VelocitySize;
            _VelocityLineRenderer.endWidth = VelocitySize;
        }
    }
    public Vector2 VelocityPoint
    {
        get => Rigidbody.position + Rigidbody.velocity;
        set => Rigidbody.velocity = value - Rigidbody.position;
    }
    public float Speed
    {
        get => Rigidbody.velocity.magnitude;
        set => Rigidbody.velocity = Rigidbody.velocity.normalized * value;
    }
    public bool IsSelected
    {
        get => _IsSelected;
        set
        {
            if (_IsSelected == value)
                return;

            _IsSelected = value;
            if (value)
            {
                ItemSelected?.Invoke();
                return;
            }
            ItemDeselected?.Invoke();
        }
    }
    public Vector3 StartVelocity = Vector2.up;
    public bool AllowChangePosition = true;
    public bool AllowChangeVelocity = true;
    public static IReadOnlyBodyCollection SelectedBodies => _SelectedBodies;
    public event Action ItemSelected;
    public event Action ItemDeselected;
    public event Action Click;
    public event Action ItemMovementStart;
    public event Action ItemMovementEnd;
    public event Action VelocityMovementStart;
    public event Action VelocityMovementEnd;
    public Rigidbody2D Rigidbody => _Rigidbody;
    public TrailRenderer Trail => _Trail;
    [SerializeField] private Rigidbody2D _Rigidbody;
    [SerializeField] private TrailRenderer _Trail;
    [SerializeField] private float _StartRadius;
    [SerializeField] private float _TrailRadiusMultiplier = 0.5f;
    [SerializeField] private float _VelocityRadiusMultiplier = 0.075f;
    [SerializeField] private float _TrailStartToEndMultiplier = 0.5f;
    private float _StartScale;
    private bool _IsSelected = false;
    private static BodyCollection _SelectedBodies = new BodyCollection();
    private Vector2 _MouseWorldPosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);
    private bool _IsMouseOnItem => Vector2.Distance(_MouseWorldPosition, transform.position) <= _Radius;
    private bool _IsMouseOnVelocityPoint => Vector2.Distance(_MouseWorldPosition, VelocityPoint) <= _VelocityLineRenderer.startWidth;
    private bool _IsItemHeld;
    private bool _IsVelocityPointHeld;
    private float _Radius;
    private Vector2 _MouseOffset;
    private Vector2 _VelocityPoint;
    private LineRenderer _VelocityLineRenderer;
    private SpriteRenderer _SpriteRenderer;

    private void Start()
    {
        _VelocityLineRenderer = GetComponent<LineRenderer>();
        _SpriteRenderer = GetComponent<SpriteRenderer>();
        _Radius = _StartRadius;
        _StartScale = transform.localScale.x;
        Rigidbody.velocity = StartVelocity;
        
        ItemSelected += () => 
        { 
            _SpriteRenderer.color = Color.gray; 
            _SelectedBodies.Add(this);
        };

        ItemDeselected += () => 
        { 
            _SpriteRenderer.color = Color.white;
            _SelectedBodies.Remove(this);
        };

        Click += () =>
        {
            _MouseOffset = Rigidbody.position - _MouseWorldPosition;
            ItemMovementStart?.Invoke();
        };

        ItemMovementStart += () => _IsItemHeld = true;
        ItemMovementEnd += () => _IsItemHeld = false;
        VelocityMovementStart += () => _IsVelocityPointHeld = true;
        VelocityMovementEnd += () => _IsVelocityPointHeld = false;

        InputEvent.LeftDoubleClick += CheckForDeselection;
    }

    private void Update()
    {
        _VelocityLineRenderer.SetPositions(new Vector3[] { transform.position, VelocityPoint });

        bool IsMouseOnItem = _IsMouseOnItem;
        bool IsMouseOnVelocityPoint = _IsMouseOnVelocityPoint;

        if (IsMouseOnItem && Input.GetMouseButtonDown(0))
            Click?.Invoke();

        if (_IsItemHeld && Input.GetMouseButtonUp(0))
            ItemMovementEnd?.Invoke();

        if (IsMouseOnVelocityPoint && Input.GetMouseButtonDown(0))
            VelocityMovementStart?.Invoke();

        if (IsMouseOnVelocityPoint && Input.GetMouseButtonUp(0))
            VelocityMovementEnd?.Invoke();

        CheckForSelection();

        if (AllowChangePosition)
            MoveItemToMouse();

        if (AllowChangeVelocity)
            MoveVelocityPointToMouse();
    }

    private void CheckForSelection()
    {
        bool IsMouseClicked = Input.GetMouseButtonDown(0);

        if (!IsMouseClicked || _IsMouseOnVelocityPoint)
            return;

        bool IsMouseOnItem = _IsMouseOnItem;

        if (IsMouseOnItem && !IsSelected)
        {
            IsSelected = true;
            return;
        }

        if (IsMouseOnItem && Input.GetKey(KeyCode.LeftShift))
        {
            IsSelected = !IsSelected;
            return;
        }
    }

    private void CheckForDeselection()
    {
        if (!_IsMouseOnItem && IsSelected && !Input.GetKey(KeyCode.LeftShift))
        {
            IsSelected = false;
            return;
        }
    }

    private void MoveItemToMouse()
    {
        if (!_IsItemHeld || _IsMouseOnVelocityPoint)
            return;

        transform.position = _MouseWorldPosition + _MouseOffset;
    }

    private void MoveVelocityPointToMouse()
    {
        if (!_IsVelocityPointHeld)
            return;

        VelocityPoint = _MouseWorldPosition;
    }

    private void OnEnable()
    {
        if (Rigidbody != null)
            VelocityPoint = _VelocityPoint;
    }

    private void OnDisable()
    {
        _VelocityPoint = VelocityPoint;
        _SelectedBodies.Remove(this);
        InputEvent.LeftDoubleClick -= CheckForDeselection;
    }  
}
