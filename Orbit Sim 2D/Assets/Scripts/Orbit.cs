using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Orbit : MonoBehaviour {
    enum OrbitType { Elliptical, Parabolic, Hyperbolic };
    enum SolveType { RpRa, VeloPos }

    // Unity Inputs
    [SerializeField] private GameObject planet;
    [SerializeField] private float tolerance = 0.00001f;
    [SerializeField] private int numTries = 10;
    [SerializeField] private int resolution = 10000;
    [SerializeField] private float littleOmega = 0.0f;
    [SerializeField] private float startTrueAnom = 0.0f;
    [SerializeField] private SolveType solveType;
    [SerializeField] private float ra = 2000;
    [SerializeField] private float rp = 1000;
    [SerializeField] private float r = 2000.0f;
    [SerializeField] private float velo = 5.0f;

    // Variables used for all orbits
    private Planet planetScript;
    private OrbitType orbitType;
    private float a;
    private float energy;
    private float trueAnom = 0.0f;
    private float e = -1.0f;
    private float theta;
    private float eccAnom;
    private float startEccAnom;
    private float timeOffset;
    private Vector2 posFromPlanet = new Vector2(0.0f, 0.0f);
    private Vector3[] positions;
    public LineRenderer orbitLine;

    // Variables used for elliptical orbits
    private float b;
    private float n;
    private float period;
    private float calcTime = 0.0f;

    // Variables used for hyperbolic orbits
    private float veloInf;
    private float totalDeflection;
    

    // Start is called before the first frame update
    protected void Start()
    {
        ra *= Globals.KM_TO_SCALE;
        rp *= Globals.KM_TO_SCALE;
        r *= Globals.KM_TO_SCALE;
        velo *= Globals.KM_TO_SCALE;
        planetScript = planet.GetComponent<Planet>();
        startTrueAnom *= Globals.DEG_TO_RAD;
        trueAnom = startTrueAnom;
        littleOmega *= Globals.DEG_TO_RAD;
        theta = littleOmega + trueAnom;
        if (solveType == SolveType.RpRa)
            SolveRaRp();
        else if (solveType == SolveType.VeloPos)
            SolvePosVelo();

        orbitLine = transform.Find("OrbitLineRenderer").GetComponent<LineRenderer>();
        UpdateOrbitLine();
        OrbitManager.instance.AddOrbit(this);
    }

    // Update is called once per frame
    protected void Update()
    {
        if (orbitType == OrbitType.Elliptical)
            SolveKeplerElliptical();
        else
            SolveKeplerHyperbolic();
        CalculateR();
        CalculateVelo();
        posFromPlanet.x = r * Mathf.Cos(theta);
        posFromPlanet.y = r * Mathf.Sin(theta);
        transform.position = new Vector3(posFromPlanet.x + planet.transform.position.x, posFromPlanet.y + planet.transform.position.y, 0.0f);
        UpdateOrbitLine();
    }

    public void PlanetUpdate() {
        if (solveType == SolveType.RpRa)
            SolveRaRp();
        else if (solveType == SolveType.VeloPos)
            SolvePosVelo();
        UpdateOrbitLine();
    }

    private void UpdateOrbitLine() {
        positions = null;
        if (orbitType == OrbitType.Elliptical)
            positions = CreateEllipse(resolution);
        else if (orbitType == OrbitType.Hyperbolic)
            positions = CreateHyperbola(resolution);
        orbitLine.positionCount = resolution + 1;
        for (int i = 0; i <= resolution; i++) {
            orbitLine.SetPosition(i, positions[i]);
        }
    }

    public void GivenRpRaSolve(float r_p, float r_a, float omega) {
        orbitType = OrbitType.Elliptical;
        rp = r_p;
        ra = r_a;
        littleOmega = omega;

        SolveRaRp();
        UpdateOrbitLine();
    }

    private void SolveRaRp() {
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

    public void GivenPosVeloSolve(float true_anomaly, float little_omega, float radius, float velocity) {
        trueAnom = true_anomaly;
        littleOmega = little_omega;
        theta = littleOmega + trueAnom;
        r = radius;
        velo = velocity;

        SolvePosVelo();
    }

    private void SolvePosVelo() {
        energy = ((float)Math.Pow(velo, 2.0f) / 2.0f) - (planetScript.gravParameter / r);
        if (energy < 0.0f) {
            orbitType = OrbitType.Elliptical;
        } else if (energy > 0.0f) {
            orbitType = OrbitType.Hyperbolic;
        } else {
            orbitType = OrbitType.Parabolic;
        }
        a = -planetScript.gravParameter / (2.0f * energy);
        int i = 0;
        while (e < 0 && i < 3600) {
            e = EccentricityQuadraticFormula();
            if (e < 0) {
                trueAnom += 0.1f * Globals.DEG_TO_RAD;
                theta = littleOmega + trueAnom;
                startTrueAnom = trueAnom;
            }
            i++;
        }
        if (orbitType == OrbitType.Elliptical)
            SolveEllipticalPosVelo();
        else if (orbitType == OrbitType.Hyperbolic)
            SolveHyperbolicPosVelo();
    }

    private void SolveEllipticalPosVelo() {
        n = (float)Math.Sqrt(planetScript.gravParameter / (float)Math.Pow(a, 3.0f));
        period = (float)(2.0 * Math.PI) / n;

        b = a * (float)Math.Sqrt(1 - Math.Pow(e, 2.0f));

        rp = a * (1.0f - e);
        ra = a * (1.0f + e);

        startEccAnom = TrueAnomToEccentricAnomElliptical(startTrueAnom);
        timeOffset = (startEccAnom - e * Mathf.Sin(startEccAnom)) / n;
    }

    private void SolveHyperbolicPosVelo() {
        veloInf = (float)Math.Sqrt(-planetScript.gravParameter/a);

        rp = a * (1.0f - e);
        totalDeflection = 2.0f * (float)Math.Asin(1.0f/e);

        n = Mathf.Sqrt(planetScript.gravParameter / Mathf.Pow(-1.0f*a, 3.0f));

        startEccAnom = TrueAnomToEccentricAnomHyperbolic(startTrueAnom);
        timeOffset = (e * (float)Math.Sinh(startEccAnom) - startEccAnom) / n;
    }

    private float EccentricityQuadraticFormula() {
        float a_q = a;
        float b_q = r * (float)Math.Cos(trueAnom);
        float c_q = r - a;
        float e1, e2;
        e1 = (-b_q + (float)Math.Sqrt(Math.Pow(b_q, 2) - 4 * a_q * c_q)) / (2 * a_q);
        e2 = (-b_q - (float)Math.Sqrt(Math.Pow(b_q, 2) - 4 * a_q * c_q)) / (2 * a_q);

        // TODO: this is not the best, if e1 is exactly 1, then it will return e1
        if (orbitType == OrbitType.Elliptical && e2 >= 0.0f && e2 < 1.0f)
            return e2;
        else if (orbitType == OrbitType.Elliptical && e1 >= 0.0f && e1 < 1.0f)
            return e1;
        else if (orbitType == OrbitType.Hyperbolic && e2 > 1.0f)
            return e2;
        else if (orbitType == OrbitType.Hyperbolic && e1 > 1.0f)
            return e1;
        else
            return -1.0f;
    }

    private float EccentricAnomToTrueAnomElliptical(float eAnom) {
        // Ambiguous quadrant, needs quad check
        // float ta = Mathf.Asin(Mathf.Sqrt(1.0f - Mathf.Pow(e, 2.0f)) * Mathf.Sin(tAnom) / (1.0f - e * Mathf.Cos(tAnom)));
        float ta = 2.0f * Mathf.Atan(Mathf.Sqrt((1.0f + e) / (1.0f - e)) * Mathf.Tan(eAnom / 2.0f));
        return ta;
    }

    private float EccentricAnomToTrueAnomHyperbolic(float eAnom) {
        float ta = 2.0f * Mathf.Atan((float)Math.Tanh(eAnom/2.0f)/Mathf.Sqrt((e-1.0f)/(e+1.0f)));
        return ta;
    }

    private float TrueAnomToEccentricAnomElliptical(float tAnom) {
        float ea = Mathf.Asin(Mathf.Sqrt(1.0f - Mathf.Pow(e, 2.0f)) * Mathf.Sin(tAnom) / (1.0f + e * Mathf.Cos(tAnom)));
        if (Mathf.Abs(EccentricAnomToTrueAnomElliptical(ea) - tAnom) > 0.0001)
            ea = Mathf.PI - ea;
        return ea;
    }

    private float TrueAnomToEccentricAnomHyperbolic(float tAnom) {
        float ea = (float)Math.Acosh((Mathf.Cos(tAnom) + e) / (1.0f + e*Mathf.Cos(tAnom)));
        if (Mathf.Abs(EccentricAnomToTrueAnomHyperbolic(ea) - tAnom) > 0.0001)
            ea = -1.0f * ea;
        return ea;
    }

    private void SolveKeplerElliptical() {
        // time = periods % time; // oops...
        calcTime = ((float)TimeKeeper.instance.time + timeOffset) % period;
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
        trueAnom = EccentricAnomToTrueAnomElliptical(eccAnom);
        theta = littleOmega + trueAnom;
    }

    private void SolveKeplerHyperbolic() {
        calcTime = (float)TimeKeeper.instance.time + timeOffset;
        float answer = n * calcTime;
        float check = e * (float)Math.Sinh(eccAnom) - eccAnom;
        int i = 0;
        while (Mathf.Abs(answer - check) > tolerance && i < numTries) {
            eccAnom = eccAnom - (e * (float)Math.Sinh(eccAnom) - eccAnom - n * calcTime) / (e * (float)Math.Cosh(eccAnom) - 1.0f);
            check = e * (float)Math.Sinh(eccAnom) - eccAnom;
            i++;
        }
        if (eccAnom > 2.0f * Mathf.PI) {
            eccAnom -= 2.0f * Mathf.PI;
        }
        trueAnom = EccentricAnomToTrueAnomHyperbolic(eccAnom);
        theta = littleOmega + trueAnom;
    }

    private void CalculateR() {
        r = a * (1.0f - Mathf.Pow(e, 2.0f)) / (1.0f + e * Mathf.Cos(trueAnom));
    }

    private void CalculateVelo() {
        velo = Mathf.Sqrt(2.0f * energy + (2.0f * planetScript.gravParameter) / r);
    }

    Vector3[] CreateEllipse(int resolution) {

        Vector3[] pos = new Vector3[resolution + 1];

        for (int i = 0; i <= resolution; i++) {
            float angle = (float)i / resolution * 2.0f * Mathf.PI;
            float rEllipse = a * (1.0f - Mathf.Pow(e, 2.0f)) / (1.0f + e * Mathf.Cos(angle));
            float xEllipse = rEllipse * Mathf.Cos(angle + littleOmega);
            float yEllipse = rEllipse * Mathf.Sin(angle + littleOmega);
            pos[i] = new Vector3(xEllipse + planet.transform.position.x, yEllipse + planet.transform.position.y, 0.0f);
        }

        return pos;
    }

    Vector3[] CreateHyperbola(int resolution) {
        Vector3[] pos = new Vector3[resolution + 1];

        for (int i = 0; i <= resolution; i++) {
            float angle = (i / (float)resolution) * Mathf.PI - (Mathf.PI / 2.0f);
            float rHype = a * (1.0f - Mathf.Pow(e, 2.0f)) / (1.0f + e * Mathf.Cos(angle));
            float xHype = rHype * Mathf.Cos(angle + littleOmega);
            float yHypee = rHype * Mathf.Sin(angle + littleOmega);
            pos[i] = new Vector3(xHype + planet.transform.position.x, yHypee + planet.transform.position.y, 0.0f);
        }

        return pos;
    }

    public float GetTrueAnom() { return trueAnom; }
    public float GetVelo() { return velo; }
    public float GetR() { return r; }
    public Planet GetPlanetScript() { return planetScript; }
}
