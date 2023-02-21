using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthRotation : MonoBehaviour
{
    private float timeOfDay;

    // Update is called once per frame
    void Update()
    {
        timeOfDay = Globals.time % Globals.SEC_IN_DAY;
        float rotationRad = (timeOfDay * 360.0f) / Globals.SEC_IN_DAY;
        var rotation = Quaternion.AngleAxis(rotationRad, Vector3.forward);
        transform.rotation = rotation;
    }
}
