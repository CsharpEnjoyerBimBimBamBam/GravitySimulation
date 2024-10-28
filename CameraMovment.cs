using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovment : MonoBehaviour
{
    public bool IsMovable = true;
    [SerializeField] private float _ZoomSensivity = 1;
    [SerializeField] private float _MinZoom = 1;
    private float _MovementSensivity = 25;
    private Camera _Camera;

    private void Start()
    {
        _Camera = GetComponent<Camera>();
    }

    private void Update()
    {
        UpdateZoom();
        if (Input.GetMouseButton(0) && IsMovable)
            UpdatePosition();
    }

    private void UpdateZoom()
    {
        float Size = _Camera.orthographicSize - Input.mouseScrollDelta.y * _ZoomSensivity;
        if (Size < _MinZoom)
            Size = _MinZoom;
        _Camera.orthographicSize = Size;
    }

    private void UpdatePosition()
    {
        float MouseX = -Input.GetAxis("Mouse X") * Time.deltaTime * _Camera.orthographicSize / Time.timeScale;
        float MouseY = -Input.GetAxis("Mouse Y") * Time.deltaTime * _Camera.orthographicSize / Time.timeScale;
        _Camera.transform.position += new Vector3(MouseX * _MovementSensivity, MouseY * _MovementSensivity);
    }
}
