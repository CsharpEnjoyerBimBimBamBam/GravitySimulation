using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BodyCollection : List<Body>, IReadOnlyBodyCollection
{
    public BodyCollection(IEnumerable<Body> Bodies) : base(Bodies) { }

    public BodyCollection() : base() { }

    public Vector2 CenterOfMass
    {
        get
        {
            Vector2 centerOfMass = Vector2.zero;
            float MassSum = 0;
            ForEach(X =>
            {
                centerOfMass += X.Rigidbody.position * X.Rigidbody.mass;
                MassSum += X.Rigidbody.mass;
            });
            return centerOfMass / MassSum;
        }
    }

    public Vector2 GeometryCenter
    {
        get
        {
            Vector2 Center = Vector2.zero;
            ForEach(X => Center += X.Rigidbody.position);
            return Center;
        }
    }

    public event Action<int> CountChanged;

    new public void Add(Body body)
    {
        (this as List<Body>).Add(body);
        CountChanged?.Invoke(Count);
    }

    new public void Remove(Body body)
    {
        (this as List<Body>).Remove(body);
        CountChanged?.Invoke(Count);
    }
}
