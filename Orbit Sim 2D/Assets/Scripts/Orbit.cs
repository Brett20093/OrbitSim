using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    [SerializeField] protected GameObject planet;
    protected Planet planetScript;

    [SerializeField] private float tolerance = 0.00001f;
    [SerializeField] private int numTries = 10;
    [SerializeField] private int resolution = 10000;

    [SerializeField] public float littleOmega = 0.0f;

    public float ra = 2000;
    public float rp = 1000;

    private float a;
    private float b;
    protected float energy;
    private float e;
    protected float trueAnom;
    private float theta;
    private float eccAnom;
    protected float r;
    protected float velo;
    private float n;
    private float period;
    private float calcTime = 0.0f;
    private Vector2 posFromPlanet = new Vector2(0.0f, 0.0f);
    private Vector3[] ellipsePositions;

    public LineRenderer orbitLine;

    // Start is called before the first frame update
    protected void Start()
    {
        ra *= Globals.KM_TO_SCALE;
        rp *= Globals.KM_TO_SCALE;
        planetScript = planet.GetComponent<Planet>();
        trueAnom = 0.0f;
        littleOmega *= Globals.DEG_TO_RAD;
        theta = littleOmega + trueAnom;
        SolveRaRp();
        orbitLine = transform.Find("OrbitLineRenderer").GetComponent<LineRenderer>();
        UpdateOrbitLine();
        OrbitManager.instance.AddOrbit(this);
    }

    public void UpdateOrbitLine() {
        ellipsePositions = null;
        ellipsePositions = CreateEllipse(resolution);
        orbitLine.positionCount = resolution + 1;
        for (int i = 0; i <= resolution; i++) {
            orbitLine.SetPosition(i, ellipsePositions[i]);
        }
    }

    // Update is called once per frame
    protected void Update()
    {
        SolveKepler();
        CalculateR();
        CalculateVelo();
        posFromPlanet.x = r * Mathf.Cos(theta);
        posFromPlanet.y = r * Mathf.Sin(theta);
        transform.position = new Vector3(posFromPlanet.x + planet.transform.position.x, posFromPlanet.y + planet.transform.position.y, 0.0f);
        UpdateOrbitLine();
    }

    public void SolveRaRp() {
        a = (ra + rp) / 2.0f;
        energy = -1.0f * planetScript.gravParameter / (2.0f * a);
        e = (ra - rp) / (ra + rp);
        b = a * Mathf.Sqrt(1.0f - Mathf.Pow(e, 2.0f));
        CalculateR();
        CalculateVelo();
        n = Mathf.Sqrt(planetScript.gravParameter/Mathf.Pow(a, 3.0f));
        period = 2.0f * Mathf.PI * Mathf.Sqrt(Mathf.Pow(a, 3.0f) / planetScript.gravParameter);
        // period = 2.0f * Mathf.PI / Mathf.Sqrt(Mathf.Sqrt(planetScript.gravParameter))*Mathf.Sqrt(Mathf.Pow(r, 3.0f)); // big oops
    }

    private float EccentricAnomToTrueAnom(float eAnom) {
        // Ambiguous quadrant, needs quad check
        // float ta = Mathf.Asin(Mathf.Sqrt(1.0f - Mathf.Pow(e, 2.0f)) * Mathf.Sin(tAnom) / (1.0f - e * Mathf.Cos(tAnom)));
        float ta = 2.0f * Mathf.Atan(Mathf.Sqrt((1.0f + e) / (1.0f - e)) * Mathf.Tan(eAnom / 2.0f));
        return ta;
    }

    private float TrueAnomToEccentricAnom(float tAnom) {
        float ea = Mathf.Asin(Mathf.Sqrt(1.0f - Mathf.Pow(e, 2.0f)) * Mathf.Sin(tAnom) / (1.0f + e * Mathf.Cos(tAnom)));
        return ea;
    }

    private void SolveKepler() {
        // time = periods % time; // oops...
        calcTime = (float)TimeKeeper.instance.time % period;
        float answer = n * calcTime;
        float check = eccAnom - e * Mathf.Sin(eccAnom);
        int i = 0;
        while (Mathf.Abs(answer-check) > tolerance && i < numTries) {
            eccAnom = eccAnom - (eccAnom - e * Mathf.Sin(eccAnom) - n * calcTime) / (1 - e * Mathf.Cos(eccAnom));
            check = eccAnom - e * Mathf.Sin(eccAnom);
            i++;
        }
        if (eccAnom > 2.0f * Mathf.PI) {
            eccAnom -= 2.0f * Mathf.PI;
        }
        trueAnom = EccentricAnomToTrueAnom(eccAnom);
        theta = littleOmega + trueAnom;
    }

    private void CalculateR() {
        r = a * (1.0f - Mathf.Pow(e, 2.0f)) / (1.0f + e * Mathf.Cos(trueAnom));
    }

    private void CalculateVelo() {
        velo = Mathf.Sqrt(2.0f * energy + (2.0f * planetScript.gravParameter) / r);
    }

    Vector3[] CreateEllipse(int resolution) {

        Vector3[] positions = new Vector3[resolution + 1];
        Vector3 center = new Vector3(planet.transform.position.x, planet.transform.position.y, 0.0f);

        for (int i = 0; i <= resolution; i++) {
            float angle = (float)i / resolution * 2.0f * Mathf.PI;
            float rEllipse = a * (1.0f - Mathf.Pow(e, 2.0f)) / (1.0f + e * Mathf.Cos(angle));
            float xEllipse = rEllipse * Mathf.Cos(angle + littleOmega);
            float yEllipse = rEllipse * Mathf.Sin(angle + littleOmega);
            positions[i] = new Vector3(xEllipse + planet.transform.position.x, yEllipse + planet.transform.position.y, 0.0f);
        }

        return positions;
    }

    public float GetTrueAnom() { return trueAnom; }
    public float GetVelo() { return velo; }
    public float GetR() { return r; }
    public Planet GetPlanetScript() { return planetScript; }
}
