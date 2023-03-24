using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Globals {
    public const float KM_TO_SCALE = 1.0f / 10.0f; // kilometers to the desired scale
    public const float KM_TO_M = 1000.0f; // kilometers to meters
    public static float PG_TO_KG = Mathf.Pow(10, 12); // petagrams to kilograms
    public const float RAD_TO_DEG = 180.0f / (float)Math.PI;
    public const float DEG_TO_RAD = (float)Math.PI / 180.0f;
    public static float GRAV_CONST = (float)6.6743 * Mathf.Pow(10, -11); // m^3 * kg^-1 * s^-2

    public static float timeMultiplier = 1000.0f;
}
