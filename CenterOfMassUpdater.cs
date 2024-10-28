using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOfMassUpdater : MonoBehaviour
{
    private void Update()
    {
        transform.position = Body.SelectedBodies.CenterOfMass;
    }
}
