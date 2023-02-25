using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Globals {
    public const float KM_TO_SCALE = 1.0f / 10.0f; // kilometers to the desired scale
    public const float RAD_TO_DEG = 180.0f / (float)Math.PI;
    public const float DEG_TO_RAD = (float)Math.PI / 180.0f;

    public static float timeMultiplier = 1000.0f;
}
