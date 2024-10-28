using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Extensions
{
    public static void Pause(this Rigidbody2D rigidbody)
    {
        if (!rigidbody.simulated)
            throw new Exception("Rigidbody already paused");
        rigidbody.simulated = false;
    }

    public static void Unpause(this Rigidbody2D rigidbody)
    {
        if (rigidbody.simulated)
            throw new Exception("Rigidbody not paused");
        rigidbody.simulated = true;
    }
}
