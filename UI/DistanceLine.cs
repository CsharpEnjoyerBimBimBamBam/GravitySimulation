using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using Unity.VisualScripting;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class DistanceLine : MonoBehaviour
{
    public float Magnitude => _Magnitude;
    [SerializeField] private GameObject _Canvas;
    [SerializeField] private TMP_Text _DistanceText;
    private float _Magnitude;
    private LineRenderer _LineRenderer;
    private Transform _Start;
    private Transform _End;

    private void Start()
    {
        _LineRenderer = GetComponent<LineRenderer>();
        _LineRenderer.positionCount = 2;
    }

    private void Update()
    {
        if (!TryMoveToTransorms())
            return;

        _DistanceText.text = _Magnitude.ToString();
    }

    public void SetPoints(Vector2 StartPoint, Vector2 EndPoint)
    {
        _Start = null;
        _End = null;

        UpdateLinePositions(StartPoint, EndPoint);
    }

    public void LockToTransforms(Transform Start, Transform End)
    {
        if (Start == null)
            throw new NullReferenceException(nameof(Start));

        if (End == null)
            throw new NullReferenceException(nameof(End));

        _Start = Start;
        _End = End;
    }

    private bool TryMoveToTransorms()
    {
        if (_Start == null || _End == null)
            return false;

        UpdateLinePositions(_Start.transform.position, _End.transform.position);
        return true;
    }

    private void UpdateLinePositions(Vector2 StartPoint, Vector2 EndPoint)
    {
        _LineRenderer.SetPositions(new Vector3[] { StartPoint, EndPoint });
        _Magnitude = Vector2.Distance(StartPoint, EndPoint);
        UpdateCanvas(StartPoint, EndPoint);
    }

    private void UpdateCanvas(Vector2 StartPoint, Vector2 EndPoint)
    {
        float Distance = Vector2.Distance(StartPoint, EndPoint);
        Vector2 Forward = EndPoint - StartPoint;

        Vector2 Upward = Vector2.Perpendicular(Forward);

        if (Upward.y < 0)
            Upward = -Upward;

        float RotationAngle = Vector2.Angle(Vector2.up, Upward);

        if (Vector2.Angle(Vector2.right, Upward) < 90)
            RotationAngle = -RotationAngle;

        _Canvas.transform.eulerAngles = new Vector3(0, 0, RotationAngle);

        _Canvas.transform.position = (Forward.normalized * (Distance / 2)) + StartPoint;
    }
}
