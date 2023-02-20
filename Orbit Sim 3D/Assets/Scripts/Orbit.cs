using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    protected double eng; // Enegery of orbit
    protected double a; // semi-major axis of orbit
    protected double e; // eccentricity of orbit
    public double rp; // periapsis of orbit
    public double ra; // apoapsis of orbit

    public double timeMultiplier = 1.0;

    public GameObject planet;
    protected Planet planetScript;

    protected double r; // radius from the center of the planet to the orbiting body
    protected double velo; // velocity of the orbiting body
    protected double true_anom; // true anomaly of the orbiting body

    public void SetRadius(float r) { this.r = r; }
    public void SetTrueAnomaly(float nu) { true_anom = nu; }
    public void SetVelocity(float velo) { this.velo = velo; }

    public double GetEnergy() { return eng; }
    public double GetSemiMajorAxis() { return a; }
    public double GetEccentricity() { return e; }
    public double GetRadius() { return r; }
    public double GetTrueAnomaly() { return true_anom; }
    public double GetVelocity() { return velo; }
}
