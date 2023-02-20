using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Globals {
    public static double scaleMeters = 1.0 / Math.Pow(10.0, 4.0);
    public static double scaleKM = 1.0 / 10.0;
    public static double earthRadius = 6563.0 * scaleKM;
    public static double earthMew = 3.986 * Math.Pow(10, 5) * Math.Pow(scaleKM, 3);
    public static double degToRad = Math.PI / 180.0;
}
