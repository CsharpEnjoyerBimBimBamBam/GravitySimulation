using System;
using System.Collections.Generic;
using UnityEngine;

public interface IReadOnlyBodyCollection : IReadOnlyList<Body>
{
    public Vector2 CenterOfMass { get; }
    public Vector2 GeometryCenter { get; }
    public event Action<int> CountChanged;
}